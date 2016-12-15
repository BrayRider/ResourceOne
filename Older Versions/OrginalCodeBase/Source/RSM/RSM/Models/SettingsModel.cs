using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.Configuration;
using System.ComponentModel.DataAnnotations;
using RSM.Support;

namespace RSM.Models
{
    // This just shapes the configuration info into a model so it's
    // easy to edit using a view/controller.
    //
    public class SettingsModel
    {
        public Configuration _config;
       
        private void SetConfigVal(string var, string val)
        {
            _config.AppSettings.Settings[var].Value = val;
            _config.Save();
        }
        
        private void SetConfigVal(string var, bool val)
        {
            SetConfigVal(var, val.ToString());
        }

        private string GetConfigString(string var)
        {
            return _config.AppSettings.Settings[var].Value;
        }

        private bool GetConfigBool(string var)
        {
            return bool.Parse(_config.AppSettings.Settings[var].Value);
        }

        public bool JobCodesFirst
        {
            get
            {
                return GetConfigBool("JobCodesFirst");
            }
            set
            {
                SetConfigVal("JobCodesFirst", value);

            }
        }

        public bool RequireApproval 
        { 
            get
            {
                return GetConfigBool("RequireAccessApproval");
            }
            set 
            {
                SetConfigVal("RequireAccessApproval", value);
                
            }
        }
        [Required(ErrorMessage="You must supply an address for your S2 hardware. ")]
        public string S2Address
        {
            get
            {
                return GetConfigString("S2Address");
            }
            set
            {
                SetConfigVal("S2Address", value);                
            }
        }

        [Required(ErrorMessage = "You must supply a folder name for the red import folder. ")]
        public string RedFolder
        {
            get
            {
                return GetConfigString("RedImportFolder");
            }
            set
            {
                SetConfigVal("RedImportFolder", value);
            }
        }
        
        [Required(ErrorMessage = "You must supply a folder name for the green import folder. ")]
        public string GreenFolder
        {
            get
            {
                return GetConfigString("GreenImportFolder");
            }
            set
            {
                SetConfigVal("GreenImportFolder", value);
            }
        }

        [Required(ErrorMessage = "You must supply an account for R1SM to log into your S2 hardware. ")]
        public string S2RSMAccountName
        {
            get
            {
                return GetConfigString("S2RSMAccountName");
            }
            set
            {
                SetConfigVal("S2RSMAccountName", value);
                
            }
        }

        
        public string S2RSMAccountPassword
        {
            get
            {
                QuickAES crypt = new QuickAES();
                try
                {
                    return crypt.DecryptString(GetConfigString("S2RSMAccountPassword"));
                }
                catch
                {
                    return "";
                }
            }
            set
            {
                QuickAES crypt = new QuickAES();
                SetConfigVal("S2RSMAccountPassword",  crypt.EncryptToString(value));                
            }
        }
        public bool S2AllowImport
        {
            get
            {
                return GetConfigBool ("S2AllowImport");
            }
            set
            {
                SetConfigVal("S2AllowImport", value);
                
            }
        }
        public bool S2AllowExport
        {
            get
            {
                return GetConfigBool("S2AllowExport");
            }
            set
            {
                SetConfigVal("S2AllowExport", value);
                
            }
        }

        
        public string AdminPass
        {
            get
            {
                QuickAES crypt = new QuickAES();
                try
                {
                    return crypt.DecryptString(GetConfigString("AdminPass"));
                }
                catch
                {
                    return "";
                }
            }
            set
            {
                QuickAES crypt = new QuickAES();
                SetConfigVal("AdminPass", crypt.EncryptToString(value));
            }
        }
        public bool RuleEngineAllow
        {
            get
            {
                return GetConfigBool("RuleEngineAllow");
            }
            set
            {
                SetConfigVal("RuleEngineAllow", value);
                
            }
        }


        public bool PSAllowImport
        {
            get
            {
                return GetConfigBool("PSAllowImport");
            }
            set
            {
                SetConfigVal("PSAllowImport", value);
                
            }
        }
       

        public SettingsModel()
        {
            if (!ReadConfigFile("app.config"))
            {
                throw new OperationCanceledException();
            }
        }

        public SettingsModel(string path)
        {
            

            if (!ReadConfigFile(path))
            {
                throw new OperationCanceledException();
            }
        }

        bool ReadConfigFile(string path)
        {
            try
            {
                _config = WebConfigurationManager.OpenWebConfiguration("/");
                _config.AppSettings.File = path;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }


    }
}