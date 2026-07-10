using Khedmetak.DAL.Entities.Base;
using System.Text.Json.Serialization;

namespace Khedmetak.DAL.Entities
{
    public class ConditionalRule : BaseEntity
    {
        public string TargetType { get; set; } = string.Empty; 
        public int TargetId { get; set; }
        public string DependentOnType { get; set; } = string.Empty;
        public int DependentOnId { get; set; }
        public string Operator { get; set; } = string.Empty; 
        public string Value { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty; 

        #region Foreign Keys
        public int GovServiceId { get; set; }
        #endregion

        #region Relations
        [JsonIgnore]
        public GovService GovService { get; set; } = null!;
        #endregion
    }
}
