using System;

namespace ArchiveSite.Data {
    public readonly struct FieldType {
        public static FieldType Boolean { get; } = new FieldType("boolean");
        public static FieldType Integer { get; } = new FieldType("integer");
        public static FieldType String { get; } = new FieldType("string");
        public static FieldType Date { get; } = new FieldType("date");

        public string TypeName { get; }

        private FieldType(String typeName) {
            TypeName = typeName;
        }

        public override Boolean Equals(Object obj) {
            return obj is FieldType other &&
                this.TypeName == other.TypeName;
        }

        public override Int32 GetHashCode() {
            return HashCode.Combine(typeof(FieldType), this.TypeName);
        }

        public override String ToString() {
            return this.TypeName;
        }

        public static Boolean operator==(FieldType ft1, FieldType ft2) {
            return ft1.Equals(ft2);
        }

        public static bool operator !=(FieldType ft1, FieldType ft2) {
            return !(ft1 == ft2);
        }

        public static explicit operator FieldType(String typeName) {
            switch (typeName) {
                case "boolean":
                    return Boolean;
                case "integer":
                    return Integer;
                case "string":
                    return String;
                case "date":
                    return Date;
                default:
                    throw new InvalidCastException();
            }
        }

        public static implicit operator String(FieldType fieldType) {
            return fieldType.TypeName;
        }
    }
}
