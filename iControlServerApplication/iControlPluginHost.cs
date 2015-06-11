using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iControlInterfaces;

namespace iControlServerApplication {
    class iControlPluginHost : IiControlPluginHost {

        public String DataDir {
            get { return Program.DataDir; }
        }

        public String PluginDir {
            get { return System.IO.Path.Combine(Program.DataDir, "plugins"); }
        }

        public void Log(string msg, IiControlPlugin plugin) {
            Program.WriteLog(String.Format("[{0}] {1}", plugin.Name, msg));
        }

        public Dictionary<string, object> DeserializeJSON(string path) {
            return Newtonsoft.Json.JsonConvert.DeserializeObject <Dictionary<string, object>>(System.IO.File.ReadAllText(path));
        }

        public void SerializeJSON(string path, Dictionary<string, object> dict) {
            System.IO.File.WriteAllText(path, Newtonsoft.Json.JsonConvert.SerializeObject(dict));
        }
    }
}
