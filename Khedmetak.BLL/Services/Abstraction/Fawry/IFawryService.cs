using Khedmetak.BLL.DTOS.Fawry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.BLL.Services.Abstraction.Fawry
{
    public interface IFawryService
    {
        Task<FawryChargeResponse> CreateChargeAsync(FawryChargeRequest request, int userId);
        Task<FawryStatusResponse> GetPaymentStatusAsync(string merchantRefNum);
    }
}
