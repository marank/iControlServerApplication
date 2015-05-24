using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iControlInterfaces {
    public interface IiControlPluginHost {
        void Log(string msg, IiControlPlugin plugin);
        Dictionary<string, string> DeserializeJSON(string path);
    }
}
