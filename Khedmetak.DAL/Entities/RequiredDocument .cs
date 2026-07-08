using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Entities.Base;
using Khedmetak.DAL.Enums;

public class RequiredDocument : BaseEntity
{
    public string DocumentName { get; set; } = string.Empty;
    public bool IsMandatory { get; set; }
    public DocumentType DocumentType { get; set; } = DocumentType.Any;

    #region Foreign Keys
    public int GovServiceId { get; set; }
    public int? StandardDocumentId { get; set; }   // <-- جديد
    #endregion

    #region Relations
    public GovService GovService { get; set; } = null!;
    public ICollection<UserDocument> UserDocuments { get; set; } = new List<UserDocument>();
    public StandardDocument? StandardDocument { get; set; }
    #endregion
}