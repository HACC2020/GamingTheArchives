using CommandLine;

namespace ArchiveSiteBackend.Web.Options {
    [Verb("host", isDefault: true, HelpText = "Host the application")]
    public class HostOptions : CommonOptions {
    }
}
