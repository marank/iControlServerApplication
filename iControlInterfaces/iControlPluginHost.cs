using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iControlInterfaces {
    public interface IiControlPluginHost {
        Object GetInfo(string info);
        void Log(string msg, IiControlPlugin plugin);

        Object GetSetting(string key, object value, IiControlPlugin plugin);
        void SetSetting(string key, object value, IiControlPlugin plugin);

        Dictionary<string, object> DeserializeJSON(string path);
        void SerializeJSON(string path, Dictionary<string, object> dict);
    }
}
