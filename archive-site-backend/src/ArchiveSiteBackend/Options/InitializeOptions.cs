using System;
using CommandLine;

namespace ArchiveSiteBackend.Web.Options {
    [Verb("init", HelpText = "Initialize the archive database.")]
    public class InitializeOptions : CommonOptions {
        [Option("drop")]
        public Boolean DropDatabase { get; set; }
    }
}
