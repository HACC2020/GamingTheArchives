using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArchiveSite.Data {
    public class Field : EntityBase<Field> {
        [Required]
        [ForeignKey(nameof(Project))]
        public Int64 ProjectId { get; set; }

        [Required]
        [StringLength(100)]
        public String Name { get; set; }

        /// <summary>
        /// A JSON map of language codes to description text.
        /// </summary>
        public String HelpTextData { get; set; }

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

        /// <summary>
        /// A JSON serialized objects defining how this field is to be validated.
        /// </summary>
        public String ValidationData { get; set; }

        // The predicted coordinates of this field on each page in % terms
        public Double? Top { get; set; }
        public Double? Left { get; set; }
        public Double? Width { get; set; }
        public Double? Height { get; set; }
    }
}
