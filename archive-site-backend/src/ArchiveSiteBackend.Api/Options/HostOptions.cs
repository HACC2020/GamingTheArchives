using CommandLine;

namespace ArchiveSiteBackend.Api.Options {
    [Verb("host", isDefault: true, HelpText = "Host the application")]
    public class HostOptions : CommonOptions {
    }
}
