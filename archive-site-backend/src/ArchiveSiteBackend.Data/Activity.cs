using System;
using System.ComponentModel.DataAnnotations;

namespace ArchiveSite.Data {
    public class Activity : EntityBase<Activity> {
        [Required]
        public Int64 UserId { get; set; }
        [Required]
        public String Message { get; set; }

        public Int64? EntityId { get; set; }
        [MaxLength(100)]
        public String EntityType { get; set; }

        [Required]
        public ActivityType Type { get; set; }

        [Required]
        public DateTimeOffset CreatedTime { get; set; }

        public virtual User User { get; set; }
    }
}
