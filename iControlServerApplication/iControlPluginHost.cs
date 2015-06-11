using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iControlInterfaces;

namespace iControlServerApplication {
    class iControlPluginHost : IiControlPluginHost {

        public object GetInfo(string info) {
            object value;
            switch (info.ToLower()) {
                case "datadir":
                    value = Program.DataDir;
                    break;
                case "plugindir":
                    value = System.IO.Path.Combine(Program.DataDir, "plugins");
                    break;
                default:
                    value = null;
                    break;
            }

            return value;
        }

        public object GetSetting(string key, object value, IiControlPlugin plugin) {
            Dictionary<string, object> settings = new Dictionary<string, object>();
            string path = System.IO.Path.Combine((string)GetInfo("plugindir"), plugin.Name + ".config");

            if (System.IO.File.Exists(path)) {
                settings = DeserializeJSON(path);
            } else {
                return value;
            }

            if (settings.ContainsKey(key)) {
                return settings[key];
            } else {
                return value;
            }
        }

        public void SetSetting(string key, object value, IiControlPlugin plugin) {
            Dictionary<string, object> settings = new Dictionary<string, object>();
            string path = System.IO.Path.Combine((string)GetInfo("plugindir"), plugin.Name + ".config");

            if (System.IO.File.Exists(path)) {
                settings = DeserializeJSON(path);
            }

            if (settings.ContainsKey(key)) {
                settings[key] = value;
            } else {
                settings.Add(key, value);
            }

            SerializeJSON(path, settings);
        }

        public void Log(string msg, IiControlPlugin plugin) {
            Program.WriteLog(string.Format("[{0}] {1}", plugin.Name, msg));
        }

        public Dictionary<string, object> DeserializeJSON(string path) {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(System.IO.File.ReadAllText(path));
        }

        public void SerializeJSON(string path, Dictionary<string, object> dict) {
            System.IO.File.WriteAllText(path, Newtonsoft.Json.JsonConvert.SerializeObject(dict, Newtonsoft.Json.Formatting.Indented));
        }
    }
}
