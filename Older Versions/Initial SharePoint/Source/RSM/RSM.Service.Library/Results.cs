using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace RSM.Service.Library
{
	[DataContract]
	public class Result<T>
		where T: class
	{
		public const string SuccessMessage = "Operation completed successfully!";
		public const string FailureMessage = "Operation failed to complete.";
		public const string ValidationMessage = "Validation errors occurred.";
		public const string DatabaseMessage = "Database validation errors occurred.";

		[DataMember(EmitDefaultValue = false)]
		public int RowsReturned { get; set; }
		
		[DataMember(EmitDefaultValue = false)]
		public ResultType Outcome { get; protected set; }

		[DataMember(EmitDefaultValue = false)]
		public string Message { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public Guid? CorrelationId { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public Dictionary<string, string> Details { get; protected set; }

		[DataMember(EmitDefaultValue = false)]
		public Exception OriginalException { get; set; }

		public T Entity { get; set; }

		public static Result<T> Success()
		{
			return new Result<T>();
		}

		public Result() :
			this(ResultType.Success, SuccessMessage)
		{
			RowsReturned = 1;
		}

		public Result(Dictionary<string, string> details) :
			this()
		{
			Details = details;
		}

		public Result(ResultType type, string message)
		{
			Outcome = type;
			Message = message;
			Entity = null;
		}

		public Result(ResultType type, string message, params string[] args) :
			this(type, string.Format(message, args))
		{ }

		public Result(ResultType type, string message, Dictionary<string, string> details) :
			this(type, message)
		{
			Details = details;
		}

		[DataMember(EmitDefaultValue = false)]
		public bool Succeeded
		{
			get
			{
				return (Outcome == ResultType.Success);
			}
			protected set
			{ }
		}

		[DataMember(EmitDefaultValue = false)]
		public bool Failed
		{
			get
			{
				return (Outcome != ResultType.Success);
			}
			protected set
			{ }
		}

		public Result<T> Set(ResultType type, string message, params string[] args)
		{
			Outcome = type;
			Message = string.Format(message, args);

			return this;
		}
		public Result<T> Set(ResultType type, Exception e, string message, params string[] args)
		{
			OriginalException = e;

			return Set(type, message, args);
		}

		/// <summary>
		///		Will optionally merge two collections of error messages.
		///		Entity value is not merged.
		/// </summary>
		/// <param name="result"></param>
		public virtual Result<T> Merge<T2>(Result<T2> result)
			where T2: class
		{
			Outcome = result.Outcome;
			Message = result.Message;

			if (result.Details != null)
			{
				Details = Details ?? new Dictionary<string, string>();

				result.Details.ToList().ForEach(x => Details.Add(GenerateKey(x.Key), x.Value));
			}

			return this;
		}

		/// <summary>
		///		Will optionally merge two collections of error messages, but will NOT check for duplicate keys prior to merging
		/// </summary>
		/// <param name="result"></param>
		public void Append(Result<T> result)
		{
			if (result.Details == null)
				return;

			if (Details == null)
				Details = new Dictionary<string, string>();

			result.Details.ToList().ForEach(x => Details.Add(x.Key, x.Value));
		}

		public void InvalidField(string message, string key = null)
		{
			Assert(true, message, key);
		}

		public Result<T> Fail(string message, string key = null)
		{
			Assert(false, message, key);

			return this;
		}

		public void Assert(bool condition, string message, string key = null)
		{
			if (condition)
				return;

			if (Details == null)
				Details = new Dictionary<string, string>();

			if (Outcome == ResultType.Success)
			{
				Outcome = ResultType.ValidationError;
				Message = ValidationMessage;
			}

			RowsReturned = 0;

			var finalKey = GenerateKey(key);

			Details.Add(finalKey, message);
		}

		/// <summary>
		///		Always evaluates
		/// </summary>
		public void Assert(Func<bool> condition, string message, string key = null)
		{
			Assert(condition(), message, key);
		}

		/// <summary>
		///		Only evaluates if Result is NOT in an error state
		/// </summary>
		public void ConditionalAssert(Func<bool> condition, string message, string key = null)
		{
			if (Failed)
				return;

			Assert(condition(), message, key);
		}

		public void RequiredField(string input, string message, string key = null, bool evaluateAlways = true)
		{
			RequiredField(delegate() { return !string.IsNullOrEmpty(input); }, message, key);
		}

		public void RequiredField(Func<bool> condition, string message, string key = null, bool evaluateAlways = true)
		{
			if (evaluateAlways)
				Assert(condition(), message, key);
			else
				ConditionalAssert(condition, message, key);
		}

		public void RequiredValue<T2>(T2? input, string message, string key = null, bool evaluateAlways = true)
			where T2 : struct
		{
			bool isNull = !input.HasValue && input.Equals(default(T2));

			if (evaluateAlways)
				Assert(!isNull, message, key);
			else
				ConditionalAssert(() => !isNull, message, key);
		}

		public void RequiredValue<T2>(T2 input, string message, string key = null, bool evaluateAlways = true)
			where T2 : struct
		{
			bool isNull = input.Equals(default(T2));

			if (evaluateAlways)
				Assert(!isNull, message, key);
			else
				ConditionalAssert(() => !isNull, message, key);
		}

		public void RequiredObject(object input, string message, string key = null, bool evaluateAlways = true)
		{
			if (evaluateAlways)
				Assert(input != null, message, key);
			else
				ConditionalAssert(() => input != null, message, key);
		}

		private string GenerateKey(string key = null)
		{
			key = (key != null && key.StartsWith("_key.")) ? null : key;
			string output = key ?? string.Format("_key.{000}", Details.Count());

			if (Details.ContainsKey(output))
				output = string.Format("_key.{000}", Details.Count());

			return output;
		}

		public override string ToString()
		{
			if ((Details == null) || (!Details.Any()))
			{
				return string.Format("<result type='{0}' timeStamp='{1}' correlationId='{2}'><message>{3}</message></result>", Outcome, DateTime.UtcNow, CorrelationId.ToString(), Message);
			}
			else
			{
				StringBuilder builder = new StringBuilder();

				builder.AppendFormat("<result type='{0}' timeStamp='{1}' correlationId='{2}'><message>{3}</message>", Outcome, DateTime.UtcNow, CorrelationId.ToString(), Message);
				builder.Append("<details>");

				foreach (var entry in Details)
				{
					builder.AppendFormat("<detail key='{0}'>{1}</detail>", entry.Key, entry.Value);
				}

				builder.Append("</details>");
				builder.Append("</result>");

				return builder.ToString();
			}
		}

		public virtual Result<T> Clone()
		{
			return new Result<T>(this.Outcome, this.Message, this.Details) { CorrelationId = this.CorrelationId, OriginalException = this.OriginalException };
		}
	}
}
