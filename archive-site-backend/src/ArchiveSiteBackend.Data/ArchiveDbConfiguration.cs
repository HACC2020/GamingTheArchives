using System;

namespace ArchiveSiteBackend.Data {
    public class ArchiveDbConfiguration {
        public String Host { get; set; } = "localhost";
        public UInt16 Port { get; set; } = 5432;
        public String Database { get; set; } = "archives";
        public String User { get; set; } = "postgres";
        public String Password { get; set; }
    }
}
