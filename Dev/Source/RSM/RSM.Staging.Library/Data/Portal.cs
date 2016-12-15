using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RSMDB = RSM.Support;

namespace RSM.Staging.Library.Data
{
	public class Portal
	{
		//portal for S2 import
		public static string Location1Portal1ExternalId = "001.Portal1";
		public static RSMDB.Portal Location1Portal1 = Factory.CreatePortal("Company A Front door", 0);

		public static string Location2Portal1ExternalId = "002.Portal1";
		public static RSMDB.Portal Location2Portal1 = Factory.CreatePortal("Company B Front door", 0);

		public static string Location2Portal2ExternalId = "002.Portal2";
		public static RSMDB.Portal Location2Portal2 = Factory.CreatePortal("Company B Back door", 0);
	}
}
