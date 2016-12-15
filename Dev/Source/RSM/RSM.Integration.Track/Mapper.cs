using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RSM.Service.Library;
using RSM.Service.Library.Model;
using RSM.Service.Library.Extensions;

namespace RSM.Integration.Track
{
	public static class MapperExtensions
	{
		public static string Max(this string text, int max)
		{
			return text.Length <= max ? text : text.Substring(0, max);
		}
	}

	public class Mapper
	{
		public static string DateFormat = "yyyyMMddHHmmss";

		private Task _task;
		public Mapper(Task task)
		{
			_task = task;
		}

		public int ToInt(string text, int? def = null)
		{
			int value;

			if (!int.TryParse(text, out value))
			{
				if (def == null)
					throw new ArgumentException(_task.LogError("value of {0} must be a valid int.", text));
				else
					value = (int)def;
			}

			return value;
		}

		public string ToMaxString(string text, int max = -1)
		{
			return text != null
				? max > -1 ? text.Max(max) : text
				: string.Empty;
		}

		public int EventLocId(string id) 
		{
			return ToInt(id);
		}
		public int DeviceId(string id, int internalId)
		{
			return ToInt(id, internalId);
		}

		public string EventDate(DateTime src)
		{
			return src.ToString(DateFormat);
		}

		public string ExtEmpRef(string src)
		{
			return ToMaxString(src, 30);
		}

		public string CardID(string src)
		{
			return ToMaxString(src, 11);
		}

		/// <summary>
		/// Location ID defined by Track
		/// </summary>
		/// <param name="src"></param>
		/// <returns></returns>
		public string LocationID(Location src)
		{
			var criteria = Factory.CreateLocation(system: ExternalSystem.TrackOut);
			criteria.InternalId = src.InternalId;

			var keys = criteria.Get(SelectKeys.Internal);
			if (keys.Failed)
				throw new ApplicationException(_task.LogError("no external Track keys exist for location {0}.", src.InternalId));

			return keys.Entity.ExternalId;
		}
		public string FirstName(string src)
		{
			return ToMaxString(src, 15);
		}
		public string MiddleName(string src)
		{
			// Need to do this since middle name is required
			var name = ToMaxString(src, 15);
			return string.IsNullOrWhiteSpace(name) ? " " : name;
		}
		public string LastName(string src)
		{
			return ToMaxString(src, 15);
		}
		public string ExtCmpRef(string src)
		{
			return ToMaxString(src, 65);
		}

		/// <summary>
		/// Portal ID will default to the internal ID if the id specified cannot be converted to a valid integer.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="internalId"></param>
		/// <returns></returns>
		public int PortalId(string id, int internalId)
		{
			return ToInt(id, internalId);
		}
		public string PortalName(string src)
		{
			return ToMaxString(src, 80);
		}
		public string PortalNetworkAddress(string src)
		{
			return ToMaxString(src, 255);
		}
		public string PortalCapabilities(string src)
		{
			return ToMaxString(src, 80);
		}
		public int PortalType(int? src)
		{
			return src != null ? (int)src : 0;
		}

		public string LocationName(string src)
		{
			return ToMaxString(src, 60);
		}

		public string ReaderName(string src)
		{
			return ToMaxString(src, 30);
		}

	}
}
