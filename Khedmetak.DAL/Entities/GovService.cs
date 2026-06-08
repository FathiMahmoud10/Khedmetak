namespace Khedmetak.DAL.Entities

{ 
public class GovService
{
    public int SrvId { get; set; }

    public string SrvName { get; set; } = string.Empty;
    //تفاصيل الخدمة
    public string? SrvDesc { get; set; }
    //رسوم الخدمة
    public decimal SrvFees { get; set; }
    //الرسوم التقديرية
    public decimal EstimatedFees { get; set; }

    // وقت الخدمة
    public int SrvTime { get; set; }   
    public int CategoryId { get; set; }

        #region Relations 

        public Category Category { get; set; } = null!;


    public ICollection<RequiredDocument> RequiredDocuments { get; set; } = new List<RequiredDocument>();

    public ICollection<KnowledgeBase> KnowledgeBaseEntries { get; set; } = new List<KnowledgeBase>();
        #endregion

    }
}