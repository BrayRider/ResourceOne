using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Security.Cryptography;
using System.Web.Configuration;
using System.Configuration;
using RSM.Support;
using System.Text;
using System.Diagnostics;

namespace RSM.Models
{

	public sealed class RSMMembershipProvider : MembershipProvider
	{
		RSMDataModelDataContext context;

		private string eventSource = "RSMMembershipProvider";
		private string eventLog = "Application";
		
		//
		// Used when determining encryption key values.
		//

		private MachineKeySection machineKey;

		//
		// If false, exceptions are thrown to the caller. If true,
		// exceptions are written to the event log.
		//

		private bool pWriteExceptionsToEventLog;

		public bool WriteExceptionsToEventLog
		{
			get { return pWriteExceptionsToEventLog; }
			set { pWriteExceptionsToEventLog = value; }
		}


		//
		// System.Configuration.Provider.ProviderBase.Initialize Method
		//

		public override void Initialize(string name, NameValueCollection config)
		{
			//
			// Initialize values from web.config.
			//

			if (config == null)
				throw new ArgumentNullException("config");

			if (name == null || name.Length == 0)
				name = "RSMMembershipProvider";

			if (String.IsNullOrEmpty(config["description"]))
			{
				config.Remove("description");
				config.Add("description", "RSM Membership provider");
			}

			// Initialize the abstract base class.
			base.Initialize(name, config);

			pApplicationName = GetConfigValue(config["applicationName"],
											System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
			pMaxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
			pPasswordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
			pMinRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredNonAlphanumericCharacters"], "1"));
			pMinRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "7"));
			pPasswordStrengthRegularExpression = Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""));
			pEnablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
			pEnablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "true"));
			pRequiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));
			pRequiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "true"));
			pWriteExceptionsToEventLog = Convert.ToBoolean(GetConfigValue(config["writeExceptionsToEventLog"], "true"));

			string temp_format = config["passwordFormat"];
			if (temp_format == null)
			{
				temp_format = "Hashed";
			}

			switch (temp_format)
			{
				case "Hashed":
					pPasswordFormat = MembershipPasswordFormat.Hashed;
					break;
				case "Encrypted":
					pPasswordFormat = MembershipPasswordFormat.Encrypted;
					break;
				case "Clear":
					pPasswordFormat = MembershipPasswordFormat.Clear;
					break;
				default:
					throw new ProviderException("Password format not supported.");
			}

			context = new RSMDataModelDataContext();

			// Get encryption and decryption key information from the configuration.
			Configuration cfg =
			  WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
			machineKey = (MachineKeySection)cfg.GetSection("system.web/machineKey");

			if (machineKey.ValidationKey.Contains("AutoGenerate"))
				if (PasswordFormat != MembershipPasswordFormat.Clear)
					throw new ProviderException("Hashed or Encrypted passwords " +
												"are not supported with auto-generated keys.");



			context = new RSMDataModelDataContext();
		}


		//
		// A helper function to retrieve config values from the configuration file.
		//

		private string GetConfigValue(string configValue, string defaultValue)
		{
			if (String.IsNullOrEmpty(configValue))
				return defaultValue;

			return configValue;
		}


		//
		// System.Web.Security.MembershipProvider properties.
		//


		private string pApplicationName;
		private bool pEnablePasswordReset;
		private bool pEnablePasswordRetrieval;
		private bool pRequiresQuestionAndAnswer;
		private bool pRequiresUniqueEmail;
		private int pMaxInvalidPasswordAttempts;
		private int pPasswordAttemptWindow;
		private MembershipPasswordFormat pPasswordFormat;

		public override string ApplicationName
		{
			get { return pApplicationName; }
			set { pApplicationName = value; }
		}

		public override bool EnablePasswordReset
		{
			get { return pEnablePasswordReset; }
		}


		public override bool EnablePasswordRetrieval
		{
			get { return pEnablePasswordRetrieval; }
		}


		public override bool RequiresQuestionAndAnswer
		{
			get { return pRequiresQuestionAndAnswer; }
		}


		public override bool RequiresUniqueEmail
		{
			get { return pRequiresUniqueEmail; }
		}


		public override int MaxInvalidPasswordAttempts
		{
			get { return pMaxInvalidPasswordAttempts; }
		}


		public override int PasswordAttemptWindow
		{
			get { return pPasswordAttemptWindow; }
		}


		public override MembershipPasswordFormat PasswordFormat
		{
			get { return pPasswordFormat; }
		}

		private int pMinRequiredNonAlphanumericCharacters;

		public override int MinRequiredNonAlphanumericCharacters
		{
			get { return pMinRequiredNonAlphanumericCharacters; }
		}

		private int pMinRequiredPasswordLength;

		public override int MinRequiredPasswordLength
		{
			get { return pMinRequiredPasswordLength; }
		}

		private string pPasswordStrengthRegularExpression;

		public override string PasswordStrengthRegularExpression
		{
			get { return pPasswordStrengthRegularExpression; }
		}

		//
		// System.Web.Security.MembershipProvider methods.
		//

		//
		// MembershipProvider.ChangePassword
		//

		public override bool ChangePassword(string username, string oldPwd, string newPwd)
		{
			if (!ValidateUser(username, oldPwd))
				return false;


			ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, newPwd, true);

			OnValidatingPassword(args);
  
			if (args.Cancel)
				if (args.FailureInformation != null)
					throw args.FailureInformation;
				else
					throw new MembershipPasswordException("Change password canceled due to new password validation failure.");


			Person person = null;

			try
			{
				person = (from p in context.Persons
						  where p.username == username
						  select p).Single();
			}
			catch (Exception)
			{
				throw new MembershipPasswordException("Change password canceled due to error finding user.");
				//return false;
			}

			person.password = EncodePassword(newPwd);

			try
			{
				context.SubmitChanges();
			}
			catch (Exception)
			{
				throw new MembershipPasswordException("Change password canceled due to error saving password.");
			}

			 return true;
		}



		//
		// MembershipProvider.ChangePasswordQuestionAndAnswer
		//

		public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPwdQuestion, string newPwdAnswer)
		{
			return false;
		}



		//
		// MembershipProvider.CreateUser - Stubbed
		//

		public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion,
												  string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
		{
			status = MembershipCreateStatus.UserRejected;
			return null;
		 
		}



		//
		// MembershipProvider.DeleteUser
		//

		public override bool DeleteUser(string username, bool deleteAllRelatedData)
		{
			return false;
		}



		//
		// MembershipProvider.GetAllUsers
		//

		public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
		{
			totalRecords = 0;
			return null;
		}


		//
		// MembershipProvider.GetNumberOfUsersOnline
		//

		public override int GetNumberOfUsersOnline()
		{
			return 0;
		}



		//
		// MembershipProvider.GetPassword
		//

		public override string GetPassword(string username, string answer)
		{
		  throw new ProviderException("Password Retrieval Not Enabled.");
		  
		  //return null;
		}



		//
		// MembershipProvider.GetUser(string, bool)
		//

		public override MembershipUser GetUser(string username, bool userIsOnline)
		{

			Person person = null;

			try
			{
				person = (from p in context.Persons
						  where p.username == username
						  select p).Single();
			}
			catch (Exception)
			{
			   
				return null;
			}
			if (person == null)
				return null;

		  
			MembershipUser u = null;
			DateTime empty = new DateTime();
			bool locked = person.LockedOut;
			DateTime lastlogin = (person.LastLogin != null) ? (DateTime)person.LastLogin : empty;
			DateTime lastAct = (person.LastActivity != null) ? (DateTime)person.LastActivity : empty;


			u = new MembershipUser(this.Name, person.username, person.PersonID, "", "", "", true, locked, DateTime.Now, lastlogin, lastAct, empty, empty);
			return u;      
		}


		//
		// MembershipProvider.GetUser(object, bool)
		//

		public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
		{
			Person person = null;

			try
			{
				person = (from p in context.Persons
						where p.PersonID == (int)providerUserKey
						select p).Single();
			}
			catch (Exception)
			{

				return null;
			}
			if (person == null)
				return null;


			MembershipUser u = null;
			DateTime empty = new DateTime();
			u = new MembershipUser(this.Name, person.username, person.PersonID, "", "", "", true, false, DateTime.Now, DateTime.Now, DateTime.Now,  empty, empty);

			return u;
		}


   


		//
		// MembershipProvider.UnlockUser
		//

		public override bool UnlockUser(string username)
		{

			Person person = null;

			try
			{
				person = (from p in context.Persons
						  where p.username  == username
						  select p).Single();
			}
			catch (Exception)
			{

				return false;
			}
			if (person == null)
				return false;


			person.LockedOut = false;
			context.SubmitChanges();
		  return false;      
		}


		//
		// MembershipProvider.GetUserNameByEmail
		//

		public override string GetUserNameByEmail(string email)
		{
			return null;
		}




		//
		// MembershipProvider.ResetPassword
		//

		public override string ResetPassword(string username, string answer)
		{
			throw new NotSupportedException("Password reset is not enabled.");
		}


		//
		// MembershipProvider.UpdateUser - stubbed
		//

		public override void UpdateUser(MembershipUser user)
		{
			Person person = null;
			RSMDataModelDataContext ctx = new RSMDataModelDataContext();
			try
			{
				person = (from p in ctx.Persons
						  where p.PersonID == (int)user.ProviderUserKey
						  select p).Single();
			}
			catch (Exception e)
			{
				WriteToEventLog(e, "UpdateUser");
				return;
			}
			if (person == null)
				return;

			
			person.LastActivity = user.LastActivityDate;
			person.LastLogin = DateTime.Now ;
			person.LockedOut = user.IsLockedOut;
			ctx.SubmitChanges();
			return;      
		 
		}

		//
		// MembershipProvider.ValidateUser
		//
		public override bool ValidateUser(string username, string password)
		{
			var isValid = false;
			Person person;

			context = new RSMDataModelDataContext();

			try
			{
				person = context.Persons.FirstOrDefault(p => p.username == username);
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("Select from persons failed for {0}\n{1}", username, ex.Message), ex);
			}

			if (person == null)
				return false;

			if (CheckPassword(password, person.password))
			{
				if (person.Active && (person.LockedOut == false))
				{
					isValid = true;
					person.FailedPasswordAttempts = 0;
					person.LastLogin = DateTime.Now;
					person.LastActivity = DateTime.Now;

					context.SubmitChanges();
				}
			}
			else
			{
				UpdateFailureCount(username, "password");
			}

			return isValid;
		}


		//
		// UpdateFailureCount
		//   A helper method that performs the checks and updates associated with
		// password failure tracking.
		//

		private void UpdateFailureCount(string username, string failureType)
		{

			Person person = null;

			try
			{
				person = (from p in context.Persons
						  where p.username == username
						  select p).Single();
			}
			catch (Exception e)
			{
				WriteToEventLog(e, "UpdateFailureCount");
				return;
			}
			if (person == null)
				return;

			

			person.FailedPasswordAttempts++;
			if (person.FailedPasswordAttempts >= MaxInvalidPasswordAttempts)
			{
				person.LockedOut = true;
			}

			context.SubmitChanges();
		}


		//
		// CheckPassword
		//   Compares password values based on the MembershipPasswordFormat.
		//
		private bool CheckPassword(string password, string dbpassword)
		{
			var pass1 = password;
			var pass2 = dbpassword;

			switch (PasswordFormat)
			{
				case MembershipPasswordFormat.Encrypted:
					pass2 = UnEncodePassword(dbpassword);
					break;
				case MembershipPasswordFormat.Hashed:
					pass1 = EncodePassword(password);
					break;
			}

			return pass1 == pass2;
		}


		//
		// EncodePassword
		//   Encrypts, Hashes, or leaves the password clear based on the PasswordFormat.
		//

		private string EncodePassword(string password)
		{
		  string encodedPassword = password;

		  switch (PasswordFormat)
		  {
			case MembershipPasswordFormat.Clear:
			  break;
			case MembershipPasswordFormat.Encrypted:
			  encodedPassword = 
				Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
			  break;
			case MembershipPasswordFormat.Hashed:
			  HMACSHA512  hash = new HMACSHA512();
			  hash.Key = HexToByte(machineKey.ValidationKey);

			  string hash1 = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password + ".rSmSa1t" + password.Length.ToString())));
			  string hash2 = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(hash1 + "an0tH3r5alt!"  + password.Length.ToString())));
			  encodedPassword =
				Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(hash2)));
			  break;
			default:
			  throw new ProviderException("Unsupported password format.");
		  }

		  return encodedPassword;
		}


		//
		// UnEncodePassword
		//   Decrypts or leaves the password clear based on the PasswordFormat.
		//

		private string UnEncodePassword(string encodedPassword)
		{
		  string password = encodedPassword;

		  switch (PasswordFormat)
		  {
			case MembershipPasswordFormat.Clear:
			  break;
			case MembershipPasswordFormat.Encrypted:
			  password = 
				Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
			  break;
			case MembershipPasswordFormat.Hashed:
			  throw new ProviderException("Cannot unencode a hashed password.");
			default:
			  throw new ProviderException("Unsupported password format.");
		  }

		  return password;
		}

		//
		// HexToByte
		//   Converts a hexadecimal string to a byte array. Used to convert encryption
		// key values from the configuration.
		//

		private byte[] HexToByte(string hexString)
		{
		  byte[] returnBytes = new byte[hexString.Length / 2];
		  for (int i = 0; i < returnBytes.Length; i++)
			returnBytes[i] = Convert.ToByte(hexString.Substring(i*2, 2), 16);
		  return returnBytes;
		}


		//
		// MembershipProvider.FindUsersByName - stubbed
		//

		public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			totalRecords = 0;
			return null;
		}

		//
		// MembershipProvider.FindUsersByEmail
		//

		public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			totalRecords = 0;
			return null;
		}


		//
		// WriteToEventLog
		//   A helper function that writes exception detail to the event log. Exceptions
		// are written to the event log as a security measure to avoid private database
		// details from being returned to the browser. If a method does not return a status
		// or boolean indicating the action succeeded or failed, a generic exception is also 
		// thrown by the caller.
		//

		private void WriteToEventLog(Exception e, string action)
		{
			try
			{
				EventLog log = new EventLog();
				log.Source = eventSource;
				log.Log = eventLog;

				string message = "An exception occurred communicating with the data source.\n\n";
				message += "Action: " + action + "\n\n";
				message += "Exception: " + e.ToString();

				log.WriteEntry(message);
			}
			catch (Exception)
			{
			}
		}

	 
	
	
	}




	#region Models

	public class ChangePasswordModel
	{
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Current password")]
		public string OldPassword { get; set; }

		[Required]
		[ValidatePasswordLength]
		[DataType(DataType.Password)]
		[Display(Name = "New password")]
		public string NewPassword { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm new password")]
		[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
	}

	public class LogOnModel
	{
		[Required]
		[Display(Name = "User name")]
		public string UserName { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[Display(Name = "Remember me?")]
		public bool RememberMe { get; set; }
	}


	public class RegisterModel
	{
		[Required]
		[Display(Name = "User name")]
		public string UserName { get; set; }

		[Required]
		[DataType(DataType.EmailAddress)]
		[Display(Name = "Email address")]
		public string Email { get; set; }

		[Required]
		[ValidatePasswordLength]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
	}
	#endregion

	#region Services
	// The FormsAuthentication type is sealed and contains static members, so it is difficult to
	// unit test code that calls its members. The interface and helper class below demonstrate
	// how to create an abstract wrapper around such a type in order to make the AccountController
	// code unit testable.

	public interface IMembershipService
	{
		int MinPasswordLength { get; }

		bool ValidateUser(string userName, string password);
		MembershipCreateStatus CreateUser(string userName, string password, string email);
		bool ChangePassword(string userName, string oldPassword, string newPassword);
	}

	public class AccountMembershipService : IMembershipService
	{
		private readonly MembershipProvider _provider;

		public AccountMembershipService()
			: this(null)
		{
		}

		public AccountMembershipService(MembershipProvider provider)
		{
			_provider = provider ?? Membership.Provider;
			
		}

		public int MinPasswordLength
		{
			get
			{
				return _provider.MinRequiredPasswordLength;
			}
		}

		public bool ValidateUser(string userName, string password)
		{
			if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
			if (String.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty.", "password");

			return _provider.ValidateUser(userName, password);
		}

		public MembershipCreateStatus CreateUser(string userName, string password, string email)
		{
			if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
			if (String.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty.", "password");
			if (String.IsNullOrEmpty(email)) throw new ArgumentException("Value cannot be null or empty.", "email");

			MembershipCreateStatus status;
			_provider.CreateUser(userName, password, email, null, null, true, null, out status);
			return status;
		}

		public bool ChangePassword(string userName, string oldPassword, string newPassword)
		{
			if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
			if (String.IsNullOrEmpty(oldPassword)) throw new ArgumentException("Value cannot be null or empty.", "oldPassword");
			if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException("Value cannot be null or empty.", "newPassword");

			// The underlying ChangePassword() will throw an exception rather
			// than return false in certain failure scenarios.
			try
			{
				MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
				return currentUser.ChangePassword(oldPassword, newPassword);
			}
			catch (ArgumentException)
			{
				return false;
			}
			catch (MembershipPasswordException)
			{
				return false;
			}
		}
	}

	public interface IFormsAuthenticationService
	{
		void SignIn(string userName, bool createPersistentCookie);
		void SignOut();
	}

	public class FormsAuthenticationService : IFormsAuthenticationService
	{
		public void SignIn(string userName, bool createPersistentCookie)
		{
			if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");

			FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
		}

		public void SignOut()
		{
			FormsAuthentication.SignOut();
		}
	}
	#endregion

	#region Validation
	public static class AccountValidation
	{
		public static string ErrorCodeToString(MembershipCreateStatus createStatus)
		{
			// See http://go.microsoft.com/fwlink/?LinkID=177550 for
			// a full list of status codes.
			switch (createStatus)
			{
				case MembershipCreateStatus.DuplicateUserName:
					return "Username already exists. Please enter a different user name.";

				case MembershipCreateStatus.DuplicateEmail:
					return "A username for that e-mail address already exists. Please enter a different e-mail address.";

				case MembershipCreateStatus.InvalidPassword:
					return "The password provided is invalid. Please enter a valid password value.";

				case MembershipCreateStatus.InvalidEmail:
					return "The e-mail address provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidAnswer:
					return "The password retrieval answer provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidQuestion:
					return "The password retrieval question provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidUserName:
					return "The user name provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.ProviderError:
					return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

				case MembershipCreateStatus.UserRejected:
					return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

				default:
					return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
			}
		}
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class ValidatePasswordLengthAttribute : ValidationAttribute, IClientValidatable
	{
		private const string _defaultErrorMessage = "'{0}' must be at least {1} characters long.";
		private readonly int _minCharacters = Membership.Provider.MinRequiredPasswordLength;

		public ValidatePasswordLengthAttribute()
			: base(_defaultErrorMessage)
		{
		}

		public override string FormatErrorMessage(string name)
		{
			return String.Format(CultureInfo.CurrentCulture, ErrorMessageString,
				name, _minCharacters);
		}

		public override bool IsValid(object value)
		{
			string valueAsString = value as string;
			return (valueAsString != null && valueAsString.Length >= _minCharacters);
		}

		public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
		{
			return new[]{
				new ModelClientValidationStringLengthRule(FormatErrorMessage(metadata.GetDisplayName()), _minCharacters, int.MaxValue)
			};
		}
	}
	#endregion


	public class RSMRoleProvider : RoleProvider
	{
		public override string[] GetRolesForUser(string username)
		{
			List<string> roles = new List<string>();
			Person person;

			RSMDataModelDataContext context = new RSMDataModelDataContext();
			try
			{
				person = (from p in context.Persons
						  where p.username == username
						  select p).Single();
			}
			catch (Exception)
			{
				if (username.Equals("admin"))
					roles.Add("admin");
				return roles.ToArray();
			}

			roles.Add("user");
			if(person.IsAdmin)
				roles.Add("admin");


			return roles.ToArray();
		}

		public override void Initialize(string name, NameValueCollection config)
		{
			base.Initialize(name, config);
		}

		private string pApplicationName;


		public override string ApplicationName
		{
			get { return pApplicationName; }
			set { pApplicationName = value; }
		} 

		public override void AddUsersToRoles(string[]  usernames, string[] rolenames)
		{
			throw new NotImplementedException();
		}
		
		public override void CreateRole(string rolename)
		{
			throw new NotImplementedException();
		}
		
		public override bool DeleteRole(string rolename, bool throwOnPopulatedRole)
		{
			return false;
		}

		public override string[] GetAllRoles()
		{
			List<string> roles = new List<string>();
			roles.Add("user");
			roles.Add("admin");
			return roles.ToArray();
		}

		public override string[] GetUsersInRole(string rolename)
		{
			throw new NotImplementedException();
		}
		
		public override void RemoveUsersFromRoles(string[] usernames, string[] rolenames)
		{
			throw new NotImplementedException();
		}

		public override bool IsUserInRole(string username, string rolename)
		{
			var context = new RSMDataModelDataContext();
			try
			{
				var person = context.Persons.FirstOrDefault(
					x => x.username.Equals(username, StringComparison.InvariantCultureIgnoreCase)); 

				if(person == null)
					return false;

				if (rolename.Equals("admin", StringComparison.InvariantCultureIgnoreCase))
					return person.IsAdmin;

				return true;
			}
			catch (Exception)
			{
				if (username.Equals("admin", StringComparison.InvariantCultureIgnoreCase))
					return true;
				
				return false;
			}
		}

		public override bool RoleExists(string rolename)
		{
			if ((rolename == "admin") || (rolename == "user"))
				return true;
			return false;
		}

		public override string[] FindUsersInRole(string rolename, string usernameToMatch)
		{
			throw new NotImplementedException();
		}

	}
}
