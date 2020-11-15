using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArchiveSite.Data {
    public class Transcription : EntityBase<Transcription> {
        [Required]
        [ForeignKey(nameof(Document))]
        public Int64 DocumentId { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public Int64 UserId { get; set; }

        /// <summary>
        /// A JSON Payload containing the transcribed data.
        /// </summary>
        public String Data { get; set; }

        /// <summary>
        /// A JSON serialized array of validation issues, or <c>null</c> if there are no issues.
        /// </summary>
        public String ValidationErrors { get; set; }

        /// <summary>
        /// Whether or not this transcription has been submitted.
        /// </summary>
        public Boolean IsSubmitted { get; set; }

        public virtual Document Document { get; set; }
    }
}
