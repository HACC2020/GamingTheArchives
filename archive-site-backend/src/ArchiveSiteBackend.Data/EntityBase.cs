using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace ArchiveSite.Data {
    public abstract class EntityBase<T> where T : EntityBase<T> {
        private static readonly PropertyInfo[] Properties =
            typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite).ToArray();

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public virtual void CopyTo(T destination) {
            foreach (var property in Properties.Where(p => p.Name != nameof(Id))) {
                property.SetValue(destination, property.GetValue(this));
            }
        }
    }
}
