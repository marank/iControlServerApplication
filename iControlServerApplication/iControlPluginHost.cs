using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iControlPluginInterface;

namespace iControlServerApplication {
    class iControlPluginHost : IiControlPluginHost {

        public void Log(string msg, IiControlPlugin plugin) {
            Program.WriteLog(String.Format("[{0}] {1}", plugin.Name, msg));
        }

        public Dictionary<string, string> DeserializeJSON(string path) {
            return Newtonsoft.Json.JsonConvert.DeserializeObject <Dictionary<string, string>>(System.IO.File.ReadAllText(path));
        }
    }
}
