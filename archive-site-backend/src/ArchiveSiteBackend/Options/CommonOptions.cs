using System;
using CommandLine;
using Microsoft.Extensions.Hosting;

namespace ArchiveSiteBackend.Web.Options {
    public class CommonOptions {
        [Option("environment")]
        public String Environment { get; set; } = Environments.Development;

        [Option("config-path")]
        public String ConfigPath { get; set; }
    }
}
