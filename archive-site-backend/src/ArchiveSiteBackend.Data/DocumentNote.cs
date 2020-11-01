using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArchiveSite.Data {
    public class DocumentNote : EntityBase<DocumentNote> {
        [Required]
        [ForeignKey(nameof(Document))]
        public Int64 DocumentId { get; set; }

        [ForeignKey(nameof(DocumentNote))]
        public Int64? ParentNote { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public Int64 AuthorId { get; set; }

        [Required]
        public NoteType Type { get; set; }

        [Required, MinLength(1)]
        public String Message { get; set; }

        public DateTimeOffset CreatedTime { get; set; }
    }
}
