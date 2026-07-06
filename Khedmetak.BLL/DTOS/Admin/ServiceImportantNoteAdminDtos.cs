using System.ComponentModel.DataAnnotations;

namespace Khedmetak.BLL.DTOS.Admin
{
    public class ServiceImportantNoteAdminDto
    {
        public int Id { get; set; }
        public string Note { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }

    public class CreateServiceImportantNoteDto
    {
        [Required, MaxLength(500)]
        public string Note { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }
    }

    public class UpdateServiceImportantNoteDto
    {
        [Required, MaxLength(500)]
        public string Note { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }
    }
}
