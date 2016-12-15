using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using RSM.Support;

namespace RSM.Support.S2
{
    public class S2Exporter
    {
        RSMDataModelDataContext db;
        readonly S2API _api;

        public S2Exporter(string uri, S2API api)
        {
            db = new RSMDataModelDataContext();
            _api = api;
        }

        public S2Exporter(string uri, string connStr, S2API api)
        {
            db = new RSMDataModelDataContext(connStr);
            _api = api;
        }

        public void ImportLevels()
        {
            string[] IDs = null;

            // First we have to get the IDs
            IDs = _api.GetLevelIDS();

            if (IDs == null) return;
            // Now that we have some we have to look each one up.

            foreach (string ID in IDs)
            {
                AccessLevel level;
                try
                {
                    // Let's see if we already have this level
                    level = (from l in db.AccessLevels
                             where l.AccessLevelID == int.Parse(ID)
                             select l).Single();
                }
                catch (Exception) //TODO: proper exception handling
                {
                    level = new AccessLevel();
                    db.AccessLevels.InsertOnSubmit(level);
                    level.AccessLevelID = int.Parse(ID);
                }

                // Now let's get the details for the level from S2.
                XmlNode details = _api.GetAccessLevel(ID);

                var xmlElement = details["ACCESSLEVELNAME"];
                if (xmlElement != null)
                    level.AccessLevelName = xmlElement .InnerText;
                    
                var element = details["ACCESSLEVELDESCRIPTION"];
                if (element != null)
                    level.AccessLevelDesc = element.InnerText;
                    
                var xmlElement1 = details["READERGROUPKEY"];
                if (xmlElement1 != null)
                    level.ReaderGroupID = int.Parse(xmlElement1.InnerText);
                    
                var element1 = details["TIMESPECGROUPKEY"];
                if (element1 != null)
                    level.TimeSpecID = int.Parse(element1.InnerText);
                    
                var xmlElement2 = details["THREATLEVELGROUPKEY"];
                if (xmlElement2 != null)
                    level.ThreatLevelGroupID = int.Parse(xmlElement2.InnerText);
            }

            db.SubmitChanges();
        }


    }
}