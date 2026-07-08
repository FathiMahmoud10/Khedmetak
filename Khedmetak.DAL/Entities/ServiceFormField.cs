using Khedmetak.DAL.Entities.Base;
using System.Text.Json.Serialization;

namespace Khedmetak.DAL.Entities
{
    public class ServiceFormField : BaseEntity
    {
        public string FieldName { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Placeholder { get; set; } = string.Empty;
        public string FieldType { get; set; } = string.Empty; // e.g., Text, Number, Date, Select, Checkbox
        public string? Choices { get; set; } // Comma-separated options for Select fields
        public bool IsRequired { get; set; }
        public string? ValidationRegex { get; set; }
        public int DisplayOrder { get; set; }

        #region Foreign Keys
        public int GovServiceId { get; set; }
        #endregion

        #region Relations
        [JsonIgnore]
        public GovService GovService { get; set; } = null!;
        #endregion
    }
}
