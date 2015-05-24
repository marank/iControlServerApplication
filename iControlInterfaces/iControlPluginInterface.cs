﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iControlInterfaces {
    public interface IiControlPlugin {
        string Name { get; }
        string Author { get; }
        IiControlPluginHost Host { get; set; }

        bool Init();
        void Handle(string[] commands, iControlInterfaces.IiControlClient client);
    }
}