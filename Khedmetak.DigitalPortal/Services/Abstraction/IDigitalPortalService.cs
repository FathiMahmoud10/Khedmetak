using System.Threading.Tasks;
using Khedmetak.DigitalPortal.DTOs;

namespace Khedmetak.DigitalPortal.Services.Abstraction
{
    public interface IDigitalPortalService
    {
        Task<bool> SendOtpAsync(DigitalPortalLoginDto dto);
        Task<DigitalPortalCitizenDto?> VerifyOtpAndGetCitizenAsync(DigitalPortalOtpDto dto);
        Task<SyncDocumentsResultDto> SyncCitizenDocumentsAsync(int userId, string nationalId);

        /// <summary>
        /// يُرسل طلب إصدار مستند رسمي إلى البوابة الرقمية عند قبول طلب المواطن،
        /// ويحفظ الملف الصادر في مجلد المستخدم ويوثقه في قاعدة البيانات.
        /// </summary>
        Task<PortalSubmissionResultDto> SubmitAndIssueServiceRequestAsync(
            int userId,
            PortalSubmissionRequestDto dto);
    }
}
