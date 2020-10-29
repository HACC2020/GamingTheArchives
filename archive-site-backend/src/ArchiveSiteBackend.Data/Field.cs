using System;
using System.ComponentModel.DataAnnotations;

namespace ArchiveSiteBackend.Data {
    public class Field : EntityBase {
        public Int64 ProjectId { get; set; }

        /// <summary>
        /// The position of the field in the field list for this project.
        /// </summary>
        [Required]
        public Int32 Index { get; set; }

        [Required, StringLength(100)]
        public String Type { get; set; }

        [Required]
        public Boolean Required { get; set; }

        [StringLength(1024)]
        public String ParsingFormat { get; set; }

        [StringLength(100)]
        public String TrueValue { get; set; }

        [StringLength(100)]
        public String FalseValue { get; set; }
    }
}
