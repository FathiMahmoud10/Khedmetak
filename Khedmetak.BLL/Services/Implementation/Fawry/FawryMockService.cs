using Khedmetak.BLL.DTOS.Fawry;
using Khedmetak.BLL.Services.Abstraction.Fawry;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo;

public class FawryMockService : IFawryService
{
    private readonly IPaymentRepository _paymentRepo;

    public FawryMockService(IPaymentRepository paymentRepo)
    {
        _paymentRepo = paymentRepo;
    }

    public async Task<FawryChargeResponse> CreateChargeAsync(FawryChargeRequest request, int userId)
    {
        var isFawry = request.PaymentMethod == "PAYATFAWRY";
        var merchantRefNum = Guid.NewGuid().ToString("N")[..16];

        // ✅ خزّن في الـ DB
        await _paymentRepo.CreateAsync(new Payment
        {
            MerchantRefNum = merchantRefNum,
            FawryRefNumber = isFawry ? "123456789" : null,
            PaymentUrl = isFawry ? null : "https://atfawry.fawrystaging.com/mockpay",
            PaymentMethod = request.PaymentMethod,
            Amount = request.Amount,
            Status = "PENDING",
            UserId = userId
        });

        return new FawryChargeResponse
        {
            ReferenceNumber = isFawry ? "123456789" : null,
            PaymentUrl = isFawry ? null : "https://atfawry.fawrystaging.com/mockpay",
            StatusCode = "200",
            StatusDescription = "Operation done successfully",
            MerchantRefNum = merchantRefNum
        };
    }

    public Task<FawryStatusResponse> GetPaymentStatusAsync(string merchantRefNum)
    {
        return Task.FromResult(new FawryStatusResponse
        {
            PaymentStatus = "PAID",
            PaymentAmount = 150.00m,
            MerchantRefNum = merchantRefNum
        });
    }
}