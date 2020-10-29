using System;
using System.ComponentModel.DataAnnotations;

namespace ArchiveSiteBackend.Data {
    public class DocumentNote : EntityBase {
        [Required]
        public Int64 DocumentId { get; set; }

        public Int64? ParentNote { get; set; }

        [Required]
        public Int64 AuthorId { get; set; }

        [Required]
        public NoteType Type { get; set; }

        [Required, MinLength(1)]
        public String Message { get; set; }

        [Required]
        public DateTime CreatedTime { get; set; }
    }
}
