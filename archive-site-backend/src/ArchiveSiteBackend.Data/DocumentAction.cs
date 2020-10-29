using System;
using System.ComponentModel.DataAnnotations;

namespace ArchiveSiteBackend.Data {
    public class DocumentAction : EntityBase {
        [Required]
        public Int64 DocumentId { get; set; }

        [Required]
        public Int64 UserId { get; set; }

        [Required]
        public DocumentActionType Type { get; set; }

        [Required]
        public DateTime ActionTime { get; set; }
    }
}
