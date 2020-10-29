using System;
using System.ComponentModel.DataAnnotations;

namespace ArchiveSiteBackend.Data {
    public class Project : EntityBase {
        [Required, StringLength(100)]
        public String Name { get; set; }

        public String Description { get; set; }
    }
}
