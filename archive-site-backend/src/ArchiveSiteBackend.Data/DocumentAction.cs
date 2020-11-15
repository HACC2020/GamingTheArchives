using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArchiveSite.Data {
    public class DocumentAction : EntityBase<DocumentAction> {
        [Required]
        [ForeignKey(nameof(Document))]
        public Int64 DocumentId { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public Int64 UserId { get; set; }

        [Required]
        public DocumentActionType Type { get; set; }

        public DateTimeOffset ActionTime { get; set; }

        public virtual Document Document { get; set; }
        public virtual User User { get; set; }
    }
}
