using System;
using System.ComponentModel.DataAnnotations;

namespace ArchiveSiteBackend.Data {
    public class Project : EntityBase {
        [Required, StringLength(100)]
        public String Name { get; set; }

        public String Description { get; set; }

        [StringLength(1024)]
        public String SampleDocumentUrl { get; set; }
    }
}
