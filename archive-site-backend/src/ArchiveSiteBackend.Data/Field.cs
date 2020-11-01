using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArchiveSite.Data {
    public class Field : EntityBase<Field> {
        [Required]
        [ForeignKey(nameof(Project))]
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

        // The predicted coordinates of this field on each page in % terms
        public Double? Top { get; set; }
        public Double? Left { get; set; }
        public Double? Width { get; set; }
        public Double? Height { get; set; }
    }
}
