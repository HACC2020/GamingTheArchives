using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using ArchiveSite.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Formatting = Newtonsoft.Json.Formatting;

namespace ArchiveSiteBackend.Api.Helpers {
    public static class RtpHelper {
        [ThreadStatic] private static XmlSerializer rtpColumnSerializer;

        public static XmlSerializer RtpColumnSerializer =>
            rtpColumnSerializer ??= new XmlSerializer(typeof(RtpColumn));

        public static Field ParseField(String commentText, XElement element) {
            var column = ParseRtpColumn(element);

            String name = null;
            String helpTextData = null;

            if (commentText != null) {
                if (commentText.IndexOf(" | ", StringComparison.Ordinal) > 0) {
                    var parts =
                        commentText.Split(new[] { '|' }, 2).Select(s => s.Trim()).ToArray();

                    name = parts[1];
                    if (name.Length > 100) {
                        name = name.Substring(0, 100);
                    }

                    helpTextData = JsonConvert.SerializeObject(new {
                        hi = parts[0],
                        en = parts[1]
                    });
                } else {
                    name = commentText.Trim();
                }
            }

            var field = new Field {
                Index = column.Index,
                Name = name,
                HelpTextData = helpTextData,
                Type = column.Type.ToString(),
                Required = column.Required
            };

            switch (column.Type) {
                case RtpColumnType.Boolean:
                    field.TrueValue = column.Parsing?.TrueValue;
                    field.FalseValue = column.Parsing?.FalseValue;
                    break;
                case RtpColumnType.Date:
                    field.ParsingFormat = column.Parsing?.Format;
                    break;
            }

            if (column.Validations != null) {
                field.ValidationData =
                    JsonConvert.SerializeObject(
                        column.Validations,
                        Formatting.None,
                        new JsonSerializerSettings {
                            NullValueHandling = NullValueHandling.Ignore,
                            Converters = {
                                new StringEnumConverter()
                            }
                        }
                    );
            }

            return field;
        }

        public static RtpColumn ParseRtpColumn(XElement element) {
            using var reader = element.CreateReader();
            return (RtpColumn)RtpColumnSerializer.Deserialize(reader);
        }

        [XmlType("column")]
        public class RtpColumn {
            [XmlElement("index")]
            public Int32 Index { get; set; }

            [XmlElement("type")]
            public RtpColumnType Type { get; set; }

            [XmlElement("required")]
            public Boolean Required { get; set; }

            [XmlElement("parsing")]
            public RtpParsingConfiguration Parsing { get; set; }

            [XmlArray("validations")]
            [XmlArrayItem("validation")]
            public RtpValidation[] Validations { get; set; }
        }

        public enum RtpColumnType {
            [XmlEnum(Name = "boolean")] Boolean,
            [XmlEnum(Name = "integer")] Integer,
            [XmlEnum(Name = "string")] String,
            [XmlEnum(Name = "date")] Date
        }

        public class RtpParsingConfiguration {
            [XmlElement("format")]
            public String Format { get; set; }

            [XmlElement("trueValue")]
            public String TrueValue { get; set; }

            [XmlElement("falseValue")]
            public String FalseValue { get; set; }
        }

        public class RtpValidation {
            [XmlElement("type")]
            public RtpValidationType Type { get; set; }

            [XmlElement("configuration")]
            public RtpValidationConfiguration Configuration { get; set; }

            [XmlElement("condition")]
            public RtpValidationCondition Condition { get; set; }
        }

        public enum RtpValidationType {
            // The column value must be between a specified minimum and maximum length.
            [XmlEnum("length")] Length,

            // The column value must match a specified regular expression.
            [XmlEnum("regex")] Regex,

            // The column value must be an integer between a specified minimum and maximum.
            [XmlEnum("integer-range")] IntegerRange,

            // The column value must be a date between a specified minimum and maximum.
            [XmlEnum("date-range")] DateRange,

            // The column value must be one of a specified set of allowed values.
            [XmlEnum("restricted-value")] RestrictedValue
        }

        public class RtpValidationConfiguration {
            [XmlElement("min")]
            public String Min { get; set; }

            [XmlElement("max")]
            public String Max { get; set; }

            [XmlArray("allowedValues")]
            [XmlArrayItem("value")]
            public String[] AllowedValues { get; set; }

            [XmlElement("pattern")]
            public String Pattern { get; set; }
        }

        [JsonConverter(typeof(RtpValidationConditionJsonConverter))]
        public class RtpValidationCondition : IXmlSerializable {
            public RtpValidationConditionOperator RootOperator { get; set; }

            public XmlSchema GetSchema() {
                return null;
            }

            public void ReadXml(XmlReader reader) {
                reader.ReadStartElement();
                if (reader.MoveToContent() == XmlNodeType.Element) {
                    this.RootOperator = this.ReadOperator(reader);
                }
            }

            public void WriteXml(XmlWriter writer) {
                throw new NotImplementedException();
            }

            private RtpValidationConditionOperator ReadOperator(XmlReader reader) {
                if (reader.IsEmptyElement) {
                    throw new XmlException(
                        $"Expected element {reader.LocalName} to contain child elements."
                    );
                }

                switch (reader.LocalName) {
                    case "and":
                        reader.ReadStartElement();
                        reader.MoveToContent();
                        var andOp = new RtpLogicalBooleanValidationConditionOperator {
                            Type = RtpValidationConditionOperatorType.And,
                            Operands = this.ReadOperands(reader).ToArray()
                        };
                        reader.ReadEndElement(); // </and>
                        return andOp;
                    case "or":
                        reader.ReadStartElement();
                        reader.MoveToContent();
                        var orOp = new RtpLogicalBooleanValidationConditionOperator {
                            Type = RtpValidationConditionOperatorType.Or,
                            Operands = this.ReadOperands(reader).ToArray()
                        };
                        reader.ReadEndElement(); // </or>
                        return orOp;
                    case "not":
                        reader.ReadStartElement();
                        reader.MoveToContent();
                        var notOp = new RtpLogicalNotValidationConditionOperator {
                            Operand = this.ReadOperator(reader)
                        };
                        reader.ReadEndElement(); // </not>
                        return notOp;
                    case "equals":
                    case "pattern":
                        var compareOp = new RtpComparisonValidationConditionOperator {
                            Type = reader.LocalName == "equals" ?
                                RtpValidationConditionOperatorType.Equals :
                                RtpValidationConditionOperatorType.Pattern
                        };

                        reader.ReadStartElement();
                        reader.MoveToContent();
                        while (reader.IsStartElement()) {
                            if (reader.IsEmptyElement) {
                                throw new XmlException(
                                    $"Expected element {reader.LocalName} to contain child elements."
                                );
                            }

                            switch (reader.LocalName) {
                                case "column":
                                    var columnContent = reader.ReadElementContentAsString();
                                    if (Int32.TryParse(columnContent, out var col)) {
                                        compareOp.Column = col;
                                    } else {
                                        throw new XmlException(
                                            $"Invalid value for column specifier in comparison: {columnContent}"
                                        );
                                    }

                                    break;
                                case "value":
                                    compareOp.Value = reader.ReadElementContentAsString();
                                    break;
                                default:
                                    throw new XmlException(
                                        $"Unexpected element encountered parsing RtpCondition comparison operator: {reader.LocalName}"
                                    );
                            }

                            // No need to ReadEndElement, ReadElementContentAsString consumes the whole element
                        }

                        reader.ReadEndElement(); // </equals> or </pattern>
                        return compareOp;
                    default:
                        throw new XmlException($"Unrecognized RtpCondition operator {reader.LocalName}");
                }
            }

            private IEnumerable<RtpValidationConditionOperator> ReadOperands(XmlReader reader) {
                while (reader.IsStartElement()) {
                    yield return ReadOperator(reader);
                    reader.MoveToContent(); // Skip any whitespace and comments
                }
            }
        }

        public enum RtpValidationConditionOperatorType {
            Equals,
            Pattern,
            And,
            Or,
            Not
        }

        public abstract class RtpValidationConditionOperator {
            public RtpValidationConditionOperatorType Type { get; set; }
        }

        public class RtpLogicalBooleanValidationConditionOperator : RtpValidationConditionOperator {
            public RtpValidationConditionOperator[] Operands { get; set; }
        }

        public class RtpLogicalNotValidationConditionOperator : RtpValidationConditionOperator {
            public RtpValidationConditionOperator Operand { get; set; }

            public RtpLogicalNotValidationConditionOperator() {
                this.Type = RtpValidationConditionOperatorType.Not;
            }
        }

        public class RtpComparisonValidationConditionOperator : RtpValidationConditionOperator {
            public Int32 Column { get; set; }
            public String Value { get; set; }
        }

        public class RtpValidationConditionJsonConverter : JsonConverter<RtpValidationCondition> {
            public override void WriteJson(
                JsonWriter writer,
                RtpValidationCondition value,
                JsonSerializer serializer) {

                /*
                 * // A condition object
                 * { "and": [
                 *     { "equals": { "column": 1, "value": "foo" } },
                 *     { "not": { "pattern": { "column": 2, "value": "[a-zA-Z]*" } } } ]
                 * }
                 */

                WriteOperatorJson(writer, value.RootOperator, serializer);
            }

            private static void WriteOperatorJson(
                JsonWriter writer,
                RtpValidationConditionOperator @operator,
                JsonSerializer serializer) {

                writer.WriteStartObject();
                switch (@operator) {
                    case RtpLogicalBooleanValidationConditionOperator logicalOperator:
                        writer.WritePropertyName(@operator.Type.ToString().ToLowerInvariant());
                        writer.WriteStartArray();
                        foreach (var operand in logicalOperator.Operands) {
                            WriteOperatorJson(writer, operand, serializer);
                        }

                        writer.WriteEndArray();

                        break;
                    case RtpLogicalNotValidationConditionOperator notOperator:
                        writer.WritePropertyName("not");
                        WriteOperatorJson(writer, notOperator.Operand, serializer);

                        break;
                    case RtpComparisonValidationConditionOperator comparisonOperator:
                        writer.WritePropertyName(comparisonOperator.Type.ToString().ToLowerInvariant());
                        serializer.Serialize(
                            writer,
                            new {
                                column = comparisonOperator.Column,
                                value = comparisonOperator.Value
                            }
                        );

                        break;
                }

                writer.WriteEndObject();
            }

            public override RtpValidationCondition ReadJson(
                JsonReader reader,
                Type objectType,
                RtpValidationCondition existingValue,
                Boolean hasExistingValue,
                JsonSerializer serializer) {

                throw new NotImplementedException();
            }
        }
    }
}
