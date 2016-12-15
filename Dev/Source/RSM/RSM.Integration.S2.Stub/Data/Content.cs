using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using RSM.Support;
using Staged = RSM.Staging.Library.Data;
using RSM.Staging.Library;

namespace RSM.Integration.S2.Stub.Data
{
	public class Content
	{
		public static Dictionary<string, Person> People = new Dictionary<string, Person>();
		public static Dictionary<string, Portal> Portals = new Dictionary<string, Portal>();
		public static Dictionary<string, Reader> Readers = new Dictionary<string, Reader>();
		public static Dictionary<string, Model.AccessHistory> AccessHistories = new Dictionary<string, Model.AccessHistory>();

		public static int NextLogId = 1;

		static Content()
		{
			People.Add(Staged.People.R1Person1ExternalId, Staged.People.R1Person1);
			People.Add(Staged.People.R1Person2ExternalId, Staged.People.R1Person2);
			People.Add("New.Person10", Factory.CreatePerson("Some", "New", "Guy"));

			Portals.Add(Staged.Portal.Location1Portal1ExternalId, Staged.Portal.Location1Portal1);
			Portals.Add(Staged.Portal.Location2Portal1ExternalId, Staged.Portal.Location2Portal1);
			Portals.Add(Staged.Portal.Location2Portal2ExternalId, Staged.Portal.Location2Portal2);

			Readers.Add(Staged.Reader.Location1Reader1ExternalId, Staged.Reader.Location1Reader1);
			Readers.Add(Staged.Reader.Location1Reader2ExternalId, Staged.Reader.Location1Reader2);
			Readers.Add(Staged.Reader.Location2Reader1ExternalId, Staged.Reader.Location2Reader1);
			Readers.Add(Staged.Reader.Location2Reader2ExternalId, Staged.Reader.Location2Reader2);
			Readers.Add(Staged.Reader.Location2Reader3ExternalId, Staged.Reader.Location2Reader3);
			Readers.Add(Staged.Reader.Location2Reader4ExternalId, Staged.Reader.Location2Reader4);

			ReloadAccessHistory();
		}

		/// <summary>
		/// Generates access history records over the past 10 minutes.
		/// </summary>
		public static void ReloadAccessHistory()
		{
			AccessHistories.Clear();

			var iPerson = 0;
			var iPortal = 0;
			var iReader = 0;
			var max = 20;
			var interval = 30; //seconds
			var start = DateTime.Now.Subtract(TimeSpan.FromSeconds(max * interval));
			for (var i = 1; i < max; i++)
			{
				var id = string.Format("Good{0:00000000}", NextLogId++);

				Math.DivRem(i, People.Count, out iPerson);
				Math.DivRem(i, Portals.Count, out iPortal);
				Math.DivRem(i, Readers.Count, out iReader);

				AccessHistories.Add(id, new Model.AccessHistory
				{
					LogId = id,
					PersonId = People.ElementAt(iPerson).Key,
					PortalKey = Portals.ElementAt(iPortal).Key,
					ReaderKey = Readers.ElementAt(iReader).Key,
					ReaderName = Readers.ElementAt(iReader).Value.Name,
					Type = ((int)S2.AccessType.Valid).ToString(),
					Dttm = start
				});

				start = start.AddSeconds((double)interval);
			}
		}
	}
}