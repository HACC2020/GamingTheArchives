using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArchiveSite.Data {
    public class User : EntityBase<User> {
        [Required, StringLength(1024), Index("ix_User_EmailAddress", IsUnique = true)]
        public String EmailAddress { get; set; }

        [Required, StringLength(100), MinLength(3)]
        public String DisplayName { get; set; }

        [Required]
        public UserType Type { get; set; }

        public DateTimeOffset LastLogin { get; set; }

        public DateTimeOffset SignupDate { get; set; }
    }
}
