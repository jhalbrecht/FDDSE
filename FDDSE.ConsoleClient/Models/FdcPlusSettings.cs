using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;

namespace FDDSE.ConsoleClient.Models
{
    public class FdcPlusSettings : IFdcPlusSettings
    {
        private  NameValueCollection _settings;
        private Configuration _configFile;
        public FdcPlusSettings()
        {
            _settings = ConfigurationManager.AppSettings;
            _configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            // _configFile = ConfigurationManager.OpenExeConfiguration();
        }

        // private string _PortSpeed;
        public string PortSpeed
        {
            get
            {
                return ReadSetting("PortSpeed");
            }
            set
            {
                AddUpdateAppSettings("PortSpeed", value);
            }
        }


        public int Count()
        {
            //var settings = ConfigurationManager.AppSettings;
            //return settings.Count;
            return _settings.Count;
        }

        public string SerialPort { get; set; }

        private void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;

                //_settings = settings;
                _configFile = configFile;

                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);

                //if (_settings[key] == null)
                //{
                //    _settings.Add(key, value);
                //}
                //else
                //{
                //    _settings[key] = value;
                //}
                //_configFile.Save(ConfigurationSaveMode.Modified);
                //ConfigurationManager.RefreshSection(_configFile.AppSettings.SectionInformation.Name);
            }
            // catch (ConfigurationErrorsException)
            catch (Exception e)
            {
                Console.WriteLine("Error writing app settings");
                Console.WriteLine(e.Message.ToString());
            }
        }

        private string ReadSetting(string key)
        {
            try
            {
                // return _settings[key] ?? "Not Found";
                return _settings[key] ?? null;
            }
            catch (ConfigurationErrorsException)
            {
                return "Error reading app settings";
            }
        }
    }

}