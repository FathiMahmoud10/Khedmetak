using System.ComponentModel.DataAnnotations;

namespace Khedmetak.BLL.DTOS.Admin
{
    /// <summary>
    /// DTO used by the Admin to CREATE a new category.
    /// </summary>
    public class CreateCategoryDto
    {
        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO used by the Admin to UPDATE an existing category.
    /// </summary>
    public class UpdateCategoryDto
    {
        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;
    }
}
