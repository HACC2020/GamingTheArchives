using System;
using CommandLine;

namespace ArchiveSiteBackend.Api.Options {
    [Verb("host", isDefault: true, HelpText = "Host the application")]
    public class HostOptions : CommonOptions {
        /// <summary>
        /// A shim option actually handled by the configuration system.
        /// </summary>
        [Option("OriginPolicy:Allow", HelpText = "Sets a hostname from which cross origin API requests should be allowed.")]
        public String OriginPolicyAllow { get; set; }
    }
}
