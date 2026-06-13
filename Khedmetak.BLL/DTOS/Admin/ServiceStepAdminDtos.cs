namespace Khedmetak.BLL.DTOS.Admin
{
    public class ServiceStepAdminDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int StepOrder { get; set; }
    }

    public class CreateServiceStepDto
    {
        public string Title { get; set; } = string.Empty;
        public int StepOrder { get; set; }
    }

    public class UpdateServiceStepDto
    {
        public string Title { get; set; } = string.Empty;
        public int StepOrder { get; set; }
    }

  
}
