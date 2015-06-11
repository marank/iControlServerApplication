using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iControlInterfaces {
    public interface IiControlPluginHost {
        String DataDir { get; }
        String PluginDir { get; }
        void Log(string msg, IiControlPlugin plugin);
        Dictionary<string, object> DeserializeJSON(string path);
        void SerializeJSON(string path, Dictionary<string, object> dict);
    }
}
