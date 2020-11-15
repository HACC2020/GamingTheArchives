using System;
using System.ComponentModel.DataAnnotations;

namespace ArchiveSite.Data {
    public class Category : EntityBase<Category> {
        [Required, MaxLength(100), MinLength(2)]
        public String Name { get; set; }
    }
}
