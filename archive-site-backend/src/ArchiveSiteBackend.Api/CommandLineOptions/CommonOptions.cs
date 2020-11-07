using System;
using CommandLine;

namespace ArchiveSiteBackend.Api.CommandLineOptions {
    public class CommonOptions {
        [Option("environment", HelpText = "The environment to use for configuration purposes.")]
        public String Environment { get; set; }

        [Option("config-path", HelpText = "A path to the folder containing appsettings configuration files.")]
        public String ConfigPath { get; set; }

        [Option("debug", HelpText = "Wait for the debugger to attach before running the command")]
        public Boolean Debug { get; set; }

        // These are all shims that are actually processed by the configuration system.
        [Option("ArchiveDb:Host", HelpText = "Overrides the database host specified in configuration.")]
        public String Host { get; set; }
        [Option("ArchiveDb:Port", HelpText = "Overrides the database port specified in configuration.")]
        public UInt16 Port { get; set; }
        [Option("ArchiveDb:Database", HelpText = "Overrides the database name specified in configuration.")]
        public String Database { get; set; }
        [Option("ArchiveDb:User", HelpText = "Overrides the database user specified in configuration.")]
        public String User { get; set; }
        [Option("ArchiveDb:Password", HelpText = "Overrides the database password specified in configuration.")]
        public String Password { get; set; }
    }
}
