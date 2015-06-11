namespace iControlInterfaces {
    public interface IiControlPlugin {
        string Name { get; }
        string Author { get; }
        string Version { get; }
        IiControlPluginHost Host { get; set; }

        bool Init();
        void Handle(string[] commands, IiControlClient client);
    }
}
