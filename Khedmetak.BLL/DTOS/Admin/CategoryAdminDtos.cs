using System.ComponentModel.DataAnnotations;

namespace Khedmetak.BLL.DTOS.Admin
{
    
    public class CreateCategoryDto
    {
        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;
    }

 
    public class UpdateCategoryDto
    {
        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;
    }
}
