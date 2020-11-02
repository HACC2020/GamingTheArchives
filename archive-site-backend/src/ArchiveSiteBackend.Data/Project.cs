using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ArchiveSite.Data {
    public class Project : EntityBase<Project> {
        [Required, StringLength(100)]
        public String Name { get; set; }

        public String Description { get; set; }

        [StringLength(1024)]
        public String SampleDocumentUrl { get; set; }

        [DefaultValue(true)]
        public Boolean Active { get; set; } = true;
    }
}
