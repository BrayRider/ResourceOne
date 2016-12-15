using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Xml;
using RSM.Artifacts.Log;


namespace RSM.Support.S2
{
	/// <summary>
	/// Represents a an interface into the S2 HTTP API.
	/// </summary>
	public class S2API
	{
		#region Constants

		public const int MAX_LEVELS_S2_SUPPORTS = 32;

		public static string DateFormat = "yyyy-MM-dd HH:mm:ss";

		#endregion

		#region Properties

		public string ApiVersion { get; private set; }

		public string SessionID { get; private set; }

		public string LastResponse { get; private set; }

		public ExternalSystem ImportSystem { get; set; }
		public ExternalSystem ExportSystem { get; set; }

		private string _uri;

		public string URI
		{
			get { return _uri; }
			set { _uri = value; }
		}

		#endregion

		#region Constructors / Dispose

		public S2API(string uri, string user, string password, bool encryptedPassword = true)
		{
			_uri = uri;
			var crypt = new QuickAES();

			try
			{
				if (!Login(user, password, encryptedPassword))
				{
					throw (new Exception(string.Format("Failed to log into S2 at {2} as {0} with the password {1}", user,
					                                   crypt.DecryptString(password), uri)));
				}
			}
			catch (Exception e)
			{
				throw (new Exception(string.Format("Failed to log into S2 at {2} as {0} with the password {1}\n{3}", user,
				                                   crypt.DecryptString(password), uri, e)));
			}

			using (var context = new RSMDataModelDataContext())
			{
				ImportSystem = context.ExternalSystems.FirstOrDefault(x => x.Name == "S2 Import");
				ExportSystem = context.ExternalSystems.FirstOrDefault(x => x.Name == "S2 Export");
			}

			ApiVersion = GetAPIVersion();
		}

		~S2API()
		{
			LogOut();
		}

		#endregion

		#region Public API

		public bool SavePerson(Person person)
		{
			return DoesPersonExist(person.PersonID.ToString()) ? ModifyPerson(person) : AddPerson(person);
		}

		public bool LogOut()
		{
			var xo = new XmlOutput()
				.XmlDeclaration()
				.Node("NETBOX-API").Attribute("sessionid", SessionID).Within()
				.Node("COMMAND").Attribute("name", "Logout").Attribute("num", "1").Attribute("dateformat", "tzoffset").Within();

			HttpPost(xo);

			return true;
		}

		public bool AddPerson(Person person)
		{
			string personDispCredentials;
			string jobDisplayDescription;
			string jobDescription;

			try
			{
				personDispCredentials = person.DisplayCredentials;
				
				if (personDispCredentials == " ")
					personDispCredentials = string.Empty;
			}
			catch (Exception)
			{
				personDispCredentials = string.Empty;
			}

			try
			{
				jobDescription = person.Job.JobDescription;
			}
			catch (Exception)
			{
				jobDescription = string.Empty;
			}

			try
			{
				jobDisplayDescription = person.Job.DisplayDescription;
				if (jobDisplayDescription.Length < 1)
					jobDisplayDescription = jobDescription;
			}
			catch (Exception)
			{
				jobDisplayDescription = jobDescription;
			}

			var jobDescr = personDispCredentials.Length > 0
			                  	? string.Format("{0}, {1}", jobDisplayDescription, personDispCredentials)
			                  	: jobDisplayDescription;

			var db = new RSMDataModelDataContext();
			XmlOutput xo;
			try
			{
				xo = new XmlOutput()
					.XmlDeclaration()
					.Node("NETBOX-API").Attribute("sessionid", SessionID).Within()
					.Node("COMMAND").Attribute("name", "AddPerson").Attribute("num", "1").Within()
					.Node("PARAMS").Within()
					.Node("PERSONID").InnerText(person.PersonID.ToString())
					.Node("LASTNAME").InnerText(person.LastName)
					.Node("FIRSTNAME").InnerText(person.NickFirst)
					.Node("MIDDLENAME").InnerText(person.MiddleName)
					.Node("CONTACTLOCATION").InnerText(person.Facility)
					.Node("UDF1").InnerText(person.JobCode)
					.Node("UDF2").InnerText(jobDescr)
					.Node("UDF3").InnerText(person.DeptID)
					.Node("UDF4").InnerText(person.DeptDescr)
					.Node("UDF5").InnerText(person.Facility)
					.Node("UDF6").InnerText(person.BadgeNumber)
					.Node("UDF7").InnerText(person.JobDescr)
					.Node("UDF8").InnerText(personDispCredentials)
					.Node("UDF9").InnerText(person.EmployeeID)
					.Node("ACCESSLEVELS").Within();
			}
			catch (Exception)
			{
				throw (new Exception(string.Format("Exception building API XML for {0}, {1}", person.LastName, person.FirstName)));
			}

			try
			{
				// TODO Deal with no access levels
				var levels = db.LevelsAssignedToPerson(person.PersonID);
				var lcount = 0;

				foreach (var l in levels)
				{
					lcount++;
					if (lcount < MAX_LEVELS_S2_SUPPORTS)
						xo.Node("ACCESSLEVEL").InnerText(l.AccessLevelName);
				}
			}
			catch (Exception)
			{
				throw (new Exception(string.Format("Exception adding levels for {0}, {1}", person.LastName, person.FirstName)));
			}

			XmlDocument doc;
			try
			{
				doc = HttpPost(xo);

				if (CallWasSuccessful(doc))
				{
					db.Syslog(ExportSystem,
					          Severity.Informational,
					          string.Format("Exported associate \"{0}, {1} {2}\" to S2.", person.LastName, person.FirstName,
					                        person.MiddleName),
					          "");

					XmlNode details = doc["NETBOX"]["RESPONSE"]["DETAILS"];
					return true;
				}
			}
			catch (Exception)
			{
				throw (new Exception(string.Format("Exception checking success for {0}, {1}", person.LastName, person.FirstName)));
			}

			try
			{
				db.Syslog(ExportSystem,
				          Severity.Error,
				          string.Format("FAILED exporting associate \"{0}, {1} {2}\" to S2.", person.LastName, person.FirstName,
				                        person.MiddleName),
				          doc["NETBOX"]["RESPONSE"]["DETAILS"]["ERRMSG"].InnerText);
			}
			catch (Exception)
			{
				throw (new Exception(string.Format("Exception logging error for {0}, {1}\n'{2}'", person.LastName, person.FirstName,
				                                   doc.InnerXml)));
			}
			return false;
		}

		public bool ModifyPerson(Person person)
		{
			var db = new RSMDataModelDataContext();

			string personDispCredentials;
			string jobDisplayDescription;
			string jobDescription;

			try
			{
				personDispCredentials = person.DisplayCredentials;

				if (personDispCredentials == " ")
					personDispCredentials = string.Empty;
			}
			catch (Exception)
			{
				personDispCredentials = string.Empty;
			}

			try
			{
				jobDescription = person.Job.JobDescription;
			}
			catch (Exception)
			{
				jobDescription = string.Empty;
			}

			try
			{
				jobDisplayDescription = person.Job.DisplayDescription;
				if (jobDisplayDescription.Length < 1)
					jobDisplayDescription = jobDescription;
			}
			catch (Exception)
			{
				jobDisplayDescription = jobDescription;
			}

			var jobDescr = personDispCredentials.Length > 0
			                  	? string.Format("{0}, {1}", jobDisplayDescription, personDispCredentials)
			                  	: jobDisplayDescription;

			XmlOutput xo;
			try
			{
				xo = new XmlOutput()
					.XmlDeclaration()
					.Node("NETBOX-API").Attribute("sessionid", SessionID).Within()
					.Node("COMMAND").Attribute("name", "ModifyPerson").Attribute("num", "1").Within()
					.Node("PARAMS").Within()
					.Node("PERSONID").InnerText(person.PersonID.ToString())
					.Node("LASTNAME").InnerText(person.LastName)

					.Node("FIRSTNAME").InnerText(person.NickFirst)
					.Node("MIDDLENAME").InnerText(person.MiddleName)
					.Node("CONTACTLOCATION").InnerText(person.Facility)
					//.Node("DELETED").InnerText((person.Active == true ? "FALSE" : "TRUE"))
					.Node("DELETED").InnerText("FALSE")
					.Node("UDF1").InnerText(person.JobCode)
					.Node("UDF2").InnerText(jobDescr)
					.Node("UDF3").InnerText(person.DeptID)
					.Node("UDF4").InnerText(person.DeptDescr)
					.Node("UDF5").InnerText(person.Facility)
					.Node("UDF6").InnerText(person.BadgeNumber)
					.Node("UDF7").InnerText(person.JobDescr)
					.Node("UDF8").InnerText(personDispCredentials)
					.Node("UDF9").InnerText(person.EmployeeID)
					.Node("ACCESSLEVELS").Within();
			}
			catch (Exception)
			{
				throw (new Exception(string.Format("Exception building API XML for {0}, {1}", person.LastName, person.FirstName)));
			}

			try
			{
				var levels = db.LevelsAssignedToPerson(person.PersonID);
				var levelCount = 0;

				foreach (var l in levels)
				{
					levelCount++;
					if (levelCount < MAX_LEVELS_S2_SUPPORTS)
						xo.Node("ACCESSLEVEL").InnerText(l.AccessLevelName);
				}

				if (person.Active == false)
				{
					xo.Node("ACCESSLEVEL").InnerText("TERMINATED ASSOCIATE");
				}

				//if (levelCount > MAX_LEVELS_S2_SUPPORTS)
				//{
				//    db.Syslog(OwningSystem,
				//              RSMDataModelDataContext.LogSeverity.ERROR,
				//              string.Format("{0}, {1} {2} has too many levels assigned.", person.LastName, person.FirstName, person.MiddleName),
				//              string.Format("The S2 hardware has a limit of {0} access levels per person.  This person has {1}.  You will need to remove some roles or consolidate multiple levels into one on the S2 hardware.", MAX_LEVELS_S2_SUPPORTS, levelCount));

				//    return false;
				//}            
			}
			catch (Exception)
			{
				throw (new Exception(string.Format("Exception adding levels for {0}, {1}", person.LastName, person.FirstName)));
			}

			try
			{
				var doc = HttpPost(xo);

				if (CallWasSuccessful(doc))
				{
					db.Syslog(ExportSystem,
					          Severity.Informational,
					          string.Format("Exported associate \"{0}, {1} {2}\" to S2.", person.LastName, person.FirstName,
					                        person.MiddleName),
					          "");

					return true;
				}

				db.Syslog(ExportSystem,
				          Severity.Error,
				          string.Format("FAILED exporting associate \"{0}, {1} {2}\" to S2.", person.LastName, person.FirstName,
				                        person.MiddleName),
				          doc["NETBOX"]["RESPONSE"]["DETAILS"]["ERRMSG"].InnerText);
			}
			catch (Exception)
			{
				throw (new Exception(string.Format("Exception parsing response for {0}, {1}", person.LastName, person.FirstName)));
			}

			return false;
		}

		public XmlNode GetPerson(string userID)
		{
			var xo = new XmlOutput()
				.XmlDeclaration()
				.Node("NETBOX-API").Attribute("sessionid", SessionID).Within()
				.Node("COMMAND").Attribute("name", "GetPerson").Attribute("num", "1").Within()
				.Node("PARAMS").Within()
				.Node("PERSONID").InnerText(userID)
				.Node("ALLPARTITIONS").InnerText("TRUE");

			var sw = new StringWriter();
			var tx = new XmlTextWriter(sw);

			xo.GetXmlDocument().WriteTo(tx);

			var doc = HttpPost(xo);

			if (CallWasSuccessful(doc))
			{
				XmlNode details = doc["NETBOX"]["RESPONSE"]["DETAILS"];
				return details;
			}

			return null;
		}

		public XmlNode SearchPersonData(string id = null, string startingKey = null, PersonState state = PersonState.All)
		{
			var xo = new XmlOutput()
				.XmlDeclaration()
				.Node("NETBOX-API").Attribute("sessionid", SessionID).Within()
				.Node("COMMAND").Attribute("name", "SearchPersonData").Attribute("num", "1").Within()
				.Node("PARAMS").Within()
				.Node("ALLPARTITIONS").InnerText("TRUE");
			
			if (!string.IsNullOrEmpty(id))
				xo.Node("PERSONID").InnerText(id);

			if (!string.IsNullOrEmpty(startingKey))
				xo.Node("STARTFROMKEY").InnerText(startingKey);

			var deleted = "ALL";
			if (state == PersonState.Deleted)
				deleted = "TRUE";
			else if (state == PersonState.NotDeleted)
				deleted = "FALSE";

			xo.Node("DELETED").InnerText(deleted);

			var sw = new StringWriter();
			var tx = new XmlTextWriter(sw);

			xo.GetXmlDocument().WriteTo(tx);

			var doc = HttpPost(xo);

			if (CallWasSuccessful(doc))
			{
				XmlNode details = doc["NETBOX"]["RESPONSE"]["DETAILS"];
				return details;
			}

			return null;
		}

		public XmlNode GetPicture(string userID)
		{
			var xo = new XmlOutput()
				.XmlDeclaration()
				.Node("NETBOX-API").Attribute("sessionid", SessionID).Within()
				.Node("COMMAND").Attribute("name", "GetPicture").Attribute("num", "1").Within()
				.Node("PARAMS").Within()
				.Node("PERSONID").InnerText(userID)
				.Node("ALLPARTITIONS").InnerText("TRUE");

			var sw = new StringWriter();
			var tx = new XmlTextWriter(sw);

			xo.GetXmlDocument().WriteTo(tx);

			var doc = HttpPost(xo);

			if (CallWasSuccessful(doc))
			{
				XmlNode details = doc["NETBOX"]["RESPONSE"]["DETAILS"];
				return details;
			}

			return null;
		}

		private static byte[] GetBytesFromSteam(Stream stream)
		{
			if (stream == null)
				return null;

			var buffer = new byte[4096];

			using (var memoryStream = new MemoryStream())
			{
				var count = 0;
				do
				{
					count = stream.Read(buffer, 0, buffer.Length);
					memoryStream.Write(buffer, 0, count);

				} while (count != 0);

				return memoryStream.ToArray();
			}
		}

		public byte[] GetPictureFromUrl(string url)
		{
			var fullUrl = string.Format("{0}/upload/pics/{1}", _uri.Replace("/goforms/nbapi", ""), url);
			var uri = new Uri(fullUrl);

			var request = HttpWebRequest.Create(uri);
			request.Method = "GET";
			request.Headers.Add(HttpRequestHeader.Cookie, string.Format(".sessionId={0}", SessionID));

			try
			{
				using(var response = request.GetResponse())
				{
					using(var responseStream = response.GetResponseStream())
					{
						var result = GetBytesFromSteam(responseStream);

						return result;
					}
				}
			}
			catch (Exception e)
			{
				EventLog.WriteEntry("R1SM", string.Format("Error getting image from S2: {0}", e.Message), EventLogEntryType.Error);
			}
			
			return null;
		}

		public XmlNode GetAccessLevel(string key)
		{
			var xo = new XmlOutput()
				.XmlDeclaration()
				.Node("NETBOX-API").Attribute("sessionid", SessionID).Within()
				.Node("COMMAND").Attribute("name", "GetAccessLevel").Attribute("num", "1").Within()
				.Node("PARAMS").Within()
				.Node("ACCESSLEVELKEY").InnerText(key);

			var sw = new StringWriter();
			var tx = new XmlTextWriter(sw);
			xo.GetXmlDocument().WriteTo(tx);

			var doc = HttpPost(xo);

			if (CallWasSuccessful(doc))
			{
				XmlNode details = doc["NETBOX"]["RESPONSE"]["DETAILS"];
				return details;
			}

			return null;
		}

		public string[] GetLevelIDS()
		{
			string[] ids = null;

			var xo = new XmlOutput()
				.XmlDeclaration()
				.Node("NETBOX-API").Attribute("sessionid", SessionID).Within()
				.Node("COMMAND").Attribute("name", "GetAccessLevels").Attribute("num", "1").Within()
				.Node("PARAMS").Within()
				.Node("WANTKEY").InnerText("TRUE");

			var sw = new StringWriter();
			var tx = new XmlTextWriter(sw);

			xo.GetXmlDocument().WriteTo(tx);

			var doc = HttpPost(xo);

			if (CallWasSuccessful(doc))
			{
				var results = doc.GetElementsByTagName("ACCESSLEVEL");
				if (results.Count > 0)
				{
					ids = new string[results.Count];
					for (var x = 0; x < results.Count; x++)
					{
						ids[x] = results[x].InnerText;
					}
				}
			}

			return ids;
		}

		public string GetAPIVersion()
		{
			var xo = new XmlOutput()
				.XmlDeclaration()
				.Node("NETBOX-API").Attribute("sessionid", SessionID).Within()
				.Node("COMMAND").Attribute("name", "GetAPIVersion").Attribute("num", "1");

			var doc = HttpPost(xo);

			if (CallWasSuccessful(doc))
			{
				var results = doc.GetElementsByTagName("APIVERSION");
				return results.Count == 1 ? string.Format("API Version: {0}", results[0].InnerXml) : doc.InnerXml;
			}

			return "call failed";
		}

		public XmlNode GetPortal(string key)
		{
			key = key.Trim();
			var xo = new XmlOutput()
				.XmlDeclaration()
				.Node("NETBOX-API").Attribute("sessionid", SessionID).Within()
				.Node("COMMAND").Attribute("name", "GetPortals").Attribute("num", "1").Within()
				.Node("PARAMS").Within()
				.Node("STARTFROMKEY").InnerText(key);

			var sw = new StringWriter();
			var tx = new XmlTextWriter(sw);

			xo.GetXmlDocument().WriteTo(tx);

			var doc = HttpPost(xo);

			var node = doc.SelectSingleNode("NETBOX/RESPONSE/DETAILS/PORTALS/PORTAL");
			if (CallWasSuccessful(doc) && node != null && node["PORTALKEY"].InnerText == key)
			{
				return node;
			}

			return null;
		}

		public XmlNode GetReader(string key)
		{
			var xo = new XmlOutput()
				.XmlDeclaration()
				.Node("NETBOX-API").Attribute("sessionid", SessionID).Within()
				.Node("COMMAND").Attribute("name", "GetReader").Attribute("num", "1").Within()
				.Node("PARAMS").Within()
				.Node("READERKEY").InnerText(key);

			var sw = new StringWriter();
			var tx = new XmlTextWriter(sw);

			xo.GetXmlDocument().WriteTo(tx);

			var doc = HttpPost(xo);

			var node = doc.SelectSingleNode("NETBOX/RESPONSE/DETAILS/READER");
			if (CallWasSuccessful(doc) && node != null)
			{
				return node;
			}

			return null;
		}

		public XmlNode GetAccessHistory(DateTime from, string nextId = null, string startFromId = null)
		{
			var xo = new XmlOutput()
				.XmlDeclaration()
				.Node("NETBOX-API").Attribute("sessionid", SessionID).Within()
				.Node("COMMAND").Attribute("name", "GetAccessHistory").Attribute("num", "1").Within();

			var param = xo.Node("PARAMS").Within();

			param.Node("OLDESTDTTM").InnerText(from.ToString(DateFormat));
			//optional NEWESTDTTM

			if (nextId != null || startFromId != null)
			{
				if (!string.IsNullOrEmpty(nextId))
				{
					param.Node("STARTLOGID").InnerText(nextId);
				}

				if (!string.IsNullOrEmpty(startFromId))
				{
					param.Node("AFTERLOGID").InnerText(startFromId);
				}
			}

			var sw = new StringWriter();
			var tx = new XmlTextWriter(sw);

			xo.GetXmlDocument().WriteTo(tx);

			var doc = HttpPost(xo);

			var code = doc.SelectSingleNode("NETBOX/RESPONSE/CODE");
			if (code != null &&
			    (code.InnerXml == "SUCCESS" || code.InnerXml == "NOT FOUND"))
			{
				return code.InnerXml == "SUCCESS" ? doc["NETBOX"]["RESPONSE"]["DETAILS"] : code;
			}

			return null;
		}

		#endregion

		#region Internal Helpers

		private bool DoesPersonExist(string userID)
		{
			return (GetPerson(userID) != null);
		}

		private bool Login(string user, string pass, bool encryptedPassword)
		{
			var crypt = new QuickAES();

			var decPass = encryptedPassword ? crypt.DecryptString(pass) : pass;

			var xo = new XmlOutput()
				.XmlDeclaration()
				.Node("NETBOX-API").Within()
				.Node("COMMAND").Attribute("name", "Login").Attribute("num", "1").Within()
				.Node("PARAMS").Within()
				.Node("USERNAME").InnerText(user)
				.Node("PASSWORD").InnerText(decPass);

			var doc = HttpPost(xo);

			if (CallWasSuccessful(doc))
			{
				try
				{
					SessionID = doc["NETBOX"].Attributes["sessionid"].InnerText;
				}
				catch (Exception ex)
				{
					SessionID = "NOT NEEDED";
				}
				return true;
			}

			throw (new Exception(doc.InnerXml));
		}

		private XmlDocument HttpPost(XmlOutput xmlData)
		{
			// parameters: name1=value1&name2=value2	
			var webRequest = WebRequest.Create(_uri);

			var sw = new StringWriter();
			var tx = new XmlTextWriter(sw);

			xmlData.GetXmlDocument().WriteTo(tx);

			var data = sw.ToString();

			webRequest.ContentType = "application/x-www-form-urlencoded";
			webRequest.Method = "POST";

			var bytes = Encoding.ASCII.GetBytes("APIcommand=" + data);
			Stream os = null;

			try
			{
				// send the Post
				webRequest.ContentLength = bytes.Length; //Count bytes to send
				os = webRequest.GetRequestStream();
				os.Write(bytes, 0, bytes.Length); //Send it
			}
			catch (WebException)
			{
				return null;
			}
			finally
			{
				if (os != null)
				{
					os.Close();
				}
			}

			try
			{
				// get the response
				var webResponse = webRequest.GetResponse();

				// ReSharper disable AssignNullToNotNullAttribute
				var sr = new StreamReader(webResponse.GetResponseStream());
				// ReSharper restore AssignNullToNotNullAttribute

				var doc = new XmlDocument();
				LastResponse = sr.ReadToEnd().Trim();
				doc.LoadXml(LastResponse);
				return doc;
			}
			catch (Exception e)
			{
				EventLog.WriteEntry("R1SM", string.Format("Error getting S2 response {0}", e.Message), EventLogEntryType.Error);
			}
			return null;
		}

		private bool CallWasSuccessful(XmlDocument doc)
		{
			try
			{
				var results = doc.GetElementsByTagName("CODE");
				if (results.Count > 0)
				{
					if (results[0].InnerXml == "SUCCESS")
					{
						return true;
					}
				}
			}
			catch (Exception)
			{
				EventLog.WriteEntry("R1SM", string.Format("Failed S2 call response:\n {0}", LastResponse), EventLogEntryType.Error);
			}
			return false;
		}

		#endregion
	}
}