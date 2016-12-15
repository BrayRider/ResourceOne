using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using RSM.Support;

namespace RSM.Support.S2
{
    public class S2Importer
    {
        RSMDataModelDataContext db;
        S2API _api;

        public S2Importer(S2API api)
        {
            db = new RSMDataModelDataContext();
            _api = api;
            
        }

        public S2Importer(string connStr, S2API api)
        {
            db = new RSMDataModelDataContext(connStr);
            _api = api;
        }

        public int ImportLevels()
        {
            AccessLevel level;
            int count = 0;
            string newLevels ="";
            
            
            string[] IDs = null;

            try
            {
                // First we have to get the IDs
                IDs = _api.GetLevelIDS();
            }
            catch (Exception exception)
            {
                
                throw new Exception("Error getting level IDs", exception);
            }
            
            

            if (IDs != null)
            {
                try
                {
                    // Mark all levels as missing first, we'll unmark the ones that are actually there in a bit
                    db.FlagAllLevelsAsMissing();
                }
                catch (Exception exception)
                {
                    
                    throw new Exception("Error marking levels as missing.", exception);
                }
                

                // Now that we have some we have to look each one up.

                foreach (string ID in IDs)
                {
                    try
                    {
                        // Let's see if we already have this level
                        level = (from l in db.AccessLevels
                                 where l.AccessLevelID == int.Parse(ID)
                                 select l).Single();
                    }
                    catch(Exception) 
                    {
                        level = new AccessLevel {AccessLevelName = ""};
                        db.AccessLevels.InsertOnSubmit(level);
                        level.AccessLevelID = int.Parse(ID);
                        
                        count++;
                    }

                    level.Missing = false;

                    // Now let's get the details for the level from S2.
                    XmlNode details = null;
                    try
                    {
                       details = _api.GetAccessLevel(ID);
                    }
                    catch (Exception exception)
                    {
                        
                        throw new Exception(string.Format("Error getting details for access level {0}", ID), exception);
                    }


                    if(details == null)
                    {
                        EventLog.WriteEntry("R1SM", string.Format("S2 returned no details for access level {0}.\n{1}", ID, _api.lastResponse), EventLogEntryType.Error);
                       
                        
                        continue;
                    }

                    try
                    {

                   
                    if ((level.AccessLevelName.Length > 0) && level.AccessLevelName != details["ACCESSLEVELNAME"].InnerText)
                    {
                        db.Syslog(RSMDataModelDataContext.LogSources.S2IMPORT,
                                  RSMDataModelDataContext.LogSeverity.WARN,
                                  string.Format("S2 access level \"{0}\" renamed to \"{1}\".", level.AccessLevelName, details["ACCESSLEVELNAME"].InnerText), 
                                  "This may indicate a change in the intent of the access level.  Please review your rules and roles.");

                        
                    }
                    if (level.AccessLevelName.Length == 0)
                    {
                        if (newLevels.Length == 0)
                            newLevels = details["ACCESSLEVELNAME"].InnerText;
                        else
                            newLevels = string.Format("{0}, {1}", newLevels, details["ACCESSLEVELNAME"].InnerText);
                    }

                    level.AccessLevelName = details["ACCESSLEVELNAME"].InnerText;
                    level.AccessLevelDesc = details["ACCESSLEVELDESCRIPTION"].InnerText;

                    }
                    catch (Exception exception)
                    {

                        throw new Exception(string.Format("Error parsing level response from S2 (1).\n{0}", details.OuterXml), exception);
                    }

                    try
                    {
                        level.ReaderGroupID = int.Parse(details["READERGROUPKEY"].InnerText);
                    }
                    catch (Exception)
                    {
                        // Older version of S2 have this renamed.
                        level.ReaderGroupID = int.Parse(details["READERKEY"].InnerText);
                    }

                    try
                    {
                        level.TimeSpecID = int.Parse(details["TIMESPECGROUPKEY"].InnerText);
                        level.ThreatLevelGroupID = int.Parse(details["THREATLEVELGROUPKEY"].InnerText);
                    }
                    catch (Exception exception)
                    {

                        throw new Exception(string.Format("Error parsing level response from S2 (2). \n{0}", details.OuterXml), exception);
                    }
                }


                try
                {
                    db.SubmitChanges();
                    if (count > 0)
                        db.Syslog(RSMDataModelDataContext.LogSources.S2IMPORT,
                                  RSMDataModelDataContext.LogSeverity.WARN,
                                  string.Format("S2 level import successful.  Found {0} new levels.", count), 
                                  string.Format("New Level names: {0}.", newLevels));

                }
                catch (Exception e)
                {
                    db.Syslog(RSMDataModelDataContext.LogSources.S2IMPORT,
                                  RSMDataModelDataContext.LogSeverity.ERROR,
                                  "S2 level import FAILED.  Could not save levels to database.", 
                                  e.ToString());
                }

                // At this point all of the levels have been added, now we need to look at any that
                // are still flagged as missing and remove them.
                int dcount = 0;
                string missingNames = "";
                var missingLevels = (from l in db.AccessLevels
                                     where l.Missing == true
                                     select l);

                foreach(AccessLevel missingLevel in missingLevels)
                {
                    dcount++;
                    if (missingNames.Length > 1)
                    {
                        missingNames += ", " + missingLevel.AccessLevelName;
                    }
                    else
                    {
                        missingNames = missingLevel.AccessLevelName;
                    }

                    // Flag anyone with this level as needing a rule pass.
                    var peeps = db.PeopleWithLevel(missingLevel.AccessLevelID);
                    foreach (Person peep in peeps)
                    {
                        peep.NeedsRulePass = true;
                    }
                }

                // Save the people changes and then delete the missing levels from the DB
                db.SubmitChanges();
                db.DeleteMissingLevels();
                if (dcount > 0)
                    db.Syslog(RSMDataModelDataContext.LogSources.S2IMPORT,
                              RSMDataModelDataContext.LogSeverity.WARN,
                              string.Format("Removed {0} levels that were deleted on the S2 box.", dcount), 
                              string.Format("Levels removed: {0}.", missingNames));

            }

            return count;

        }

    }
}
