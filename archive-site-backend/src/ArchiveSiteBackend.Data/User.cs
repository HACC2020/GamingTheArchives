using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArchiveSiteBackend.Data {
    public class User : EntityBase {
        [Required, StringLength(1024), Index("ix_User_EmailAddress", IsUnique = true)]
        public String EmailAddress { get; set; }

        [Required, StringLength(100), MinLength(3)]
        public String DisplayName { get; set; }

        [Required]
        public UserType Type { get; set; }

        [Required]
        public DateTime LastLogin { get; set; }

        [Required]
        public DateTime SignupDate { get; set; }
    }
}
