using Khedmetak.DAL.Entities.Base;

public class StandardDocument : BaseEntity
{
    public string DocumentName { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
    public string? GeneralRule { get; set; }

    #region Relations
    public ICollection<RequiredDocument> RequiredDocuments { get; set; } = new List<RequiredDocument>();
    #endregion
}