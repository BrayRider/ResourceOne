using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.IO;
using System.Xml;


namespace RSM.Support.S2
{
    /// <summary>
    /// Represents a an interface into the S2 HTTP API.
    /// </summary>
    public class S2API
    {
        public const int MAX_LEVELS_S2_SUPPORTS = 32;
        string _uri;
        string _sessionID;
        public string lastResponse { get; private set; }

        public S2API(string uri, string user, string password)
        {
            _uri = uri;
            QuickAES crypt = new QuickAES();
            try
            {
                if (!Login(user, password))
                {
             
                    throw (new Exception(string.Format("Failed to log into S2 at {2} as {0} with the password {1}", user, crypt.DecryptString(password), uri)));
                }
            }
            catch (Exception e)
            {
                throw (new Exception(string.Format("Failed to log into S2 at {2} as {0} with the password {1}\n{3}", user, crypt.DecryptString(password), uri, e)));

            }
        }

        public string URI
        {
            get
            {
                return _uri;
            }
            set
            {
                _uri = value;
            }
        }


         ~S2API()
        {
            LogOut();
        }

#region "Public API"



        bool DoesPersonExist(string userID)
        {
            return (GetPerson(userID) != null);
        }

        public bool SavePerson(Person person)
        {
            return DoesPersonExist(person.PersonID.ToString()) ? ModifyPerson(person) : AddPerson(person);
        }

        bool Login(string user, string pass)
        {
            QuickAES crypt = new QuickAES();
            string decPass = crypt.DecryptString(pass);
            XmlOutput xo = new XmlOutput()
                           .XmlDeclaration()
                           .Node("NETBOX-API").Within()
                           .Node("COMMAND").Attribute("name", "Login").Attribute("num", "1").Within()
                           .Node("PARAMS").Within()
                           .Node("USERNAME").InnerText(user)
                           .Node("PASSWORD").InnerText(decPass);

            XmlDocument doc = HttpPost(xo);

            if (CallWasSuccessful(doc))
            {

                try
                {
                    _sessionID = doc["NETBOX"].Attributes["sessionid"].InnerText;
                }
                catch (Exception)
                {
                    _sessionID = "NOT NEEDED";
                }
                return true;
            }

            throw (new Exception(doc.InnerXml));
            
        }


        public bool LogOut()
        {

            XmlOutput xo = new XmlOutput()
                           .XmlDeclaration()
                           .Node("NETBOX-API").Attribute("sessionid", _sessionID).Within()
                           .Node("COMMAND").Attribute("name", "Logout").Attribute("num", "1").Attribute("dateformat", "tzoffset").Within();
                   

            XmlDocument doc = HttpPost(xo);
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

            string jobDescr = personDispCredentials.Length > 0 ? string.Format("{0}, {1}", jobDisplayDescription, personDispCredentials) : jobDisplayDescription;


            RSMDataModelDataContext db = new RSMDataModelDataContext();
            XmlOutput xo;
            try
            {

                xo = new XmlOutput()
                            .XmlDeclaration()
                            .Node("NETBOX-API").Attribute("sessionid", _sessionID).Within()
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
                int lcount = 0;
                foreach (AccessLevel l in levels)
                {
                    lcount++;
                    if(lcount < MAX_LEVELS_S2_SUPPORTS)
                        xo.Node("ACCESSLEVEL").InnerText(l.AccessLevelName);
                    
                }

                //if(lcount == 0)
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
                    db.Syslog(RSMDataModelDataContext.LogSources.S2EXPORT,
                              RSMDataModelDataContext.LogSeverity.INFO,
                              string.Format("Exported associate \"{0}, {1} {2}\" to S2.", person.LastName, person.FirstName, person.MiddleName),
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
                db.Syslog(RSMDataModelDataContext.LogSources.S2EXPORT,
                              RSMDataModelDataContext.LogSeverity.ERROR,
                              string.Format("FAILED exporting associate \"{0}, {1} {2}\" to S2.", person.LastName, person.FirstName, person.MiddleName),
                              doc["NETBOX"]["RESPONSE"]["DETAILS"]["ERRMSG"].InnerText);
            }
            catch (Exception)
            {
                throw (new Exception(string.Format("Exception logging error for {0}, {1}\n'{2}'", person.LastName, person.FirstName, doc.InnerXml)));
            }
                return false;
        }

        public bool ModifyPerson(Person person)
        {
            RSMDataModelDataContext db = new RSMDataModelDataContext();

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
                if(jobDisplayDescription.Length <1)
                    jobDisplayDescription = jobDescription;
            }
            catch (Exception)
            {
                jobDisplayDescription = jobDescription;

            }

            string jobDescr = personDispCredentials.Length > 0 ? string.Format("{0}, {1}", jobDisplayDescription, personDispCredentials) : jobDisplayDescription;

            XmlOutput xo;
            try
            {
                xo = new XmlOutput()
                               .XmlDeclaration()
                               .Node("NETBOX-API").Attribute("sessionid", _sessionID).Within()
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
                int levelCount = 0;

                foreach (AccessLevel l in levels)
                {
                    levelCount++;
                    if (levelCount < MAX_LEVELS_S2_SUPPORTS)
                        xo.Node("ACCESSLEVEL").InnerText(l.AccessLevelName);
                }

                if(person.Active == false)
                {
                    xo.Node("ACCESSLEVEL").InnerText("TERMINATED ASSOCIATE");
                }

                //if (levelCount > MAX_LEVELS_S2_SUPPORTS)
                //{
                //    db.Syslog(RSMDataModelDataContext.LogSources.S2EXPORT,
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
                XmlDocument doc = HttpPost(xo);

                if (CallWasSuccessful(doc))
                {
                    db.Syslog(RSMDataModelDataContext.LogSources.S2EXPORT,
                              RSMDataModelDataContext.LogSeverity.INFO,
                              string.Format("Exported associate \"{0}, {1} {2}\" to S2.", person.LastName, person.FirstName, person.MiddleName),
                              "");

                   
                    return true;
                }

                db.Syslog(RSMDataModelDataContext.LogSources.S2EXPORT,
                              RSMDataModelDataContext.LogSeverity.ERROR,
                              string.Format("FAILED exporting associate \"{0}, {1} {2}\" to S2.", person.LastName, person.FirstName, person.MiddleName),
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
            XmlOutput xo = new XmlOutput()
                            .XmlDeclaration()
                            .Node("NETBOX-API").Attribute("sessionid", _sessionID ).Within()
                            .Node("COMMAND").Attribute("name", "GetPerson").Attribute("num", "1").Within()
                            .Node("PARAMS").Within()
                            .Node("PERSONID").InnerText(userID);


            StringWriter sw = new StringWriter();
            XmlTextWriter tx = new XmlTextWriter(sw);
            xo.GetXmlDocument().WriteTo(tx);

            //string data = sw.ToString();


            XmlDocument doc = HttpPost(xo);

            if (CallWasSuccessful(doc))
            {
                XmlNode details = doc["NETBOX"]["RESPONSE"]["DETAILS"];
                return details;
            }

            return null;
        }


        public XmlNode GetPicture(string userID)
        {
            XmlOutput xo = new XmlOutput()
                            .XmlDeclaration()
                            .Node("NETBOX-API").Attribute("sessionid", _sessionID).Within()
                            .Node("COMMAND").Attribute("name", "GetPicture").Attribute("num", "1").Within()
                            .Node("PARAMS").Within()
                            .Node("PERSONID").InnerText(userID);


            StringWriter sw = new StringWriter();
            XmlTextWriter tx = new XmlTextWriter(sw);
            xo.GetXmlDocument().WriteTo(tx);

            //string data = sw.ToString();


            XmlDocument doc = HttpPost(xo);

            if (CallWasSuccessful(doc))
            {
                XmlNode details = doc["NETBOX"]["RESPONSE"]["DETAILS"];
                return details;
            }

            return null;
        }

        public XmlNode GetAccessLevel(string key)
        {
            XmlOutput xo = new XmlOutput()
                            .XmlDeclaration()
                            .Node("NETBOX-API").Attribute("sessionid", _sessionID ).Within()
                            .Node("COMMAND").Attribute("name", "GetAccessLevel").Attribute("num", "1").Within()
                            .Node("PARAMS").Within()
                            .Node("ACCESSLEVELKEY").InnerText(key);


            StringWriter sw = new StringWriter();
            XmlTextWriter tx = new XmlTextWriter(sw);
            xo.GetXmlDocument().WriteTo(tx);

            //string data = sw.ToString();


            XmlDocument doc = HttpPost(xo);

            if (CallWasSuccessful(doc))
            {

                XmlNode details = doc["NETBOX"]["RESPONSE"]["DETAILS"];
                return details;
            }
            else
            {
                
            }

            return null;


        }

        public string[] GetLevelIDS()
        {
            string[] ids = null;

            XmlOutput xo = new XmlOutput()
                            .XmlDeclaration()
                            .Node("NETBOX-API").Attribute("sessionid", _sessionID ).Within()
                            .Node("COMMAND").Attribute("name", "GetAccessLevels").Attribute("num", "1").Within()
                            .Node("PARAMS").Within()
                            .Node("WANTKEY").InnerText("TRUE");


            StringWriter sw = new StringWriter();
            XmlTextWriter tx = new XmlTextWriter(sw);
            xo.GetXmlDocument().WriteTo(tx);

            //string data = sw.ToString();


            XmlDocument doc = HttpPost(xo);

            if(CallWasSuccessful(doc))
            {
                XmlNodeList results = doc.GetElementsByTagName("ACCESSLEVEL");
                if (results.Count > 0)
                {
                    ids = new string[results.Count];
                    for (int x = 0; x < results.Count; x++)
                    {
                        ids[x] = results[x].InnerText;
                    }
                }
            
            }

            return ids;

        }



        public string GetAPIVersion()
        {
            XmlOutput xo = new XmlOutput()
                            .XmlDeclaration()
                            .Node("NETBOX-API").Attribute("sessionid", _sessionID ).Within()
                            .Node("COMMAND").Attribute("name", "GetAPIVersion").Attribute("num", "1");

            XmlDocument doc = HttpPost(xo);

            if (CallWasSuccessful(doc))
            {
                XmlNodeList results = doc.GetElementsByTagName("APIVERSION");
                return results.Count == 1 ? string.Format("API Version: {0}", results[0].InnerXml) : doc.InnerXml;
            }



            return "call failed";
        }

#endregion


#region "Internal Helpers"

        XmlDocument HttpPost(XmlOutput xmlData)
        {
            // parameters: name1=value1&name2=value2	
            WebRequest webRequest = WebRequest.Create(_uri);

            StringWriter sw = new StringWriter();
            XmlTextWriter tx = new XmlTextWriter(sw);
            xmlData.GetXmlDocument().WriteTo(tx);

            string data = sw.ToString();

            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";
            byte[] bytes = Encoding.ASCII.GetBytes("APIcommand=" + data);
            Stream os = null;
            try
            { // send the Post
                webRequest.ContentLength = bytes.Length;   //Count bytes to send
                os = webRequest.GetRequestStream();
                os.Write(bytes, 0, bytes.Length);         //Send it
            }
            catch (WebException )
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
            { // get the response
                WebResponse webResponse = webRequest.GetResponse();
                // ReSharper disable AssignNullToNotNullAttribute
                StreamReader sr = new StreamReader(webResponse.GetResponseStream());
                // ReSharper restore AssignNullToNotNullAttribute
                XmlDocument doc = new XmlDocument();
                lastResponse = sr.ReadToEnd().Trim();
                doc.LoadXml(lastResponse);
                return doc;
            }
            catch (Exception e )
            {
                EventLog.WriteEntry("R1SM", string.Format("Error getting S2 response {0}", e.Message), EventLogEntryType.Error);
            }
            return null;
        } // end HttpPost  
        
        bool CallWasSuccessful(XmlDocument doc)
        {
            try
            {
                XmlNodeList results = doc.GetElementsByTagName("CODE");
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
                EventLog.WriteEntry("R1SM", string.Format("Failed S2 call response:\n {0}", lastResponse), EventLogEntryType.Error);
            }
            return false;
        }

#endregion

    }
}
