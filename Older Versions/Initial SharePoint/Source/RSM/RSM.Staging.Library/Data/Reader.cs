using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RSMDB = RSM.Support;

namespace RSM.Staging.Library.Data
{
	public class Reader
	{
		//reader for S2 import
		// Location 1
		public static string Location1Reader1ExternalId = "001.Reader1";
		public static RSMDB.Reader Location1Reader1 = Factory.CreateReader("Front door IN", 0, direction: RSMDB.ReaderDirection.In);
		public static string Location1Reader2ExternalId = "001.Reader2";
		public static RSMDB.Reader Location1Reader2 = Factory.CreateReader("Front door OUT", 0, direction: RSMDB.ReaderDirection.Out);

		// Location 2
		public static string Location2Reader1ExternalId = "002.Reader1";
		public static RSMDB.Reader Location2Reader1 = Factory.CreateReader("Front door IN", 0, direction: RSMDB.ReaderDirection.In);
		public static string Location2Reader2ExternalId = "002.Reader2";
		public static RSMDB.Reader Location2Reader2 = Factory.CreateReader("Front door OUT", 0, direction: RSMDB.ReaderDirection.Out);	
		public static string Location2Reader3ExternalId = "002.Reader3";
		public static RSMDB.Reader Location2Reader3 = Factory.CreateReader("Back door IN", 0, direction: RSMDB.ReaderDirection.In);
		public static string Location2Reader4ExternalId = "002.Reader4";
		public static RSMDB.Reader Location2Reader4 = Factory.CreateReader("Back door OUT", 0, direction: RSMDB.ReaderDirection.Out);
	}
}
