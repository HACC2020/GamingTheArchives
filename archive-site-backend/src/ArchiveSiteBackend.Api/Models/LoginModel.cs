using System;

namespace ArchiveSiteBackend.Api.Models {
    public class LoginModel {
        public String ReturnUrl { get; set; }

        public Boolean Insecure { get; set; }
    }
}
