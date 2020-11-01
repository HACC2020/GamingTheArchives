using System;

namespace ArchiveSiteBackend.Api.Configuration {
    public class OriginPolicyConfiguration {
        public String Allow { get; set; }

        public Boolean HasOrigin() {
            return !String.IsNullOrWhiteSpace(this.Allow);
        }
    }
}
