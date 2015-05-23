using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iControlPluginInterface;

namespace iControlServerApplication {
    class iControlPluginHost : IiControlPluginHost {

        public void Log(string msg, IiControlPlugin plugin) {
            Program.Log(String.Format("[{0}] {1}", plugin.Name, msg));
        }
    }
}
