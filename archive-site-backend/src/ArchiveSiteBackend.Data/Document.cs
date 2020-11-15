using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArchiveSite.Data {
    public class Document : EntityBase<Document> {
        [Required, StringLength(1024)]
        public String FileName { get; set; }

        [Required, StringLength(1024)]
        public String DocumentImageUrl { get; set; }

        [Required]
        [ForeignKey(nameof(Project))]
        public Int64 ProjectId { get; set; }

        [Required]
        public DocumentStatus Status { get; set; } = DocumentStatus.PendingTranscription;

        [Required]
        public Int32 TranscriptionCount { get; set; }

        [ForeignKey(nameof(Transcription))]
        public Int64? ApprovedTranscriptionId { get; set; }

        public virtual Project Project { get; set; }
    }
}
