using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iControlPluginInterface {
    public interface IiControlPlugin {
        string Name { get; }
        string Author { get; }
        IiControlPluginHost Host { get; set; }

        bool Init();
        void Handle(string[] commands, string ip);
    }

    public interface IiControlPluginHost {
        void Log(string msg, IiControlPlugin plugin);
        Dictionary<string, string> DeserializeJSON(string path);
    }
}
