using Khedmetak.BLL.DTOS.Fawry;
using Khedmetak.BLL.Services.Abstraction.Fawry;
using Khedmetak.DAL.Repo;
using Khedmetak.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Khedmetak.DAL.Entities;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IFawryService _fawry;
    private readonly IPaymentRepository _paymentRepo;
    private readonly UserManager<User> _userManager;

    public PaymentController(IFawryService fawry, IPaymentRepository paymentRepo, UserManager<User> userManager)
    {
        _fawry = fawry;
        _paymentRepo = paymentRepo;
        _userManager = userManager;
    }

    // ✅ إنشاء طلب دفع
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] FawryChargeRequest request)
    {
        // جيب الـ UserId من الـ JWT Token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();
        var userId = int.Parse(userIdClaim);

        var result = await _fawry.CreateChargeAsync(request, userId);
        return Ok(result);
    }

    // ✅ Callback من فوري (بدون Auth)
    [AllowAnonymous]
    [HttpPost("callback")]
    public async Task<IActionResult> Callback([FromBody] FawryCallbackDto dto)
    {
        var payment = await _paymentRepo.GetByMerchantRefNumAsync(dto.MerchantRefNum);

        if (payment == null) return NotFound();

        payment.Status = dto.PaymentStatus; // PAID / UNPAID / EXPIRED
        payment.FawryRefNumber = dto.FawryRefNumber;

        if (dto.PaymentStatus == "PAID")
            payment.PaidAt = DateTime.UtcNow;

        await _paymentRepo.UpdateAsync(payment);

        return Ok();
    }

    // ✅ استعلام عن حالة دفع
    [HttpGet("status/{merchantRefNum}")]
    public async Task<IActionResult> Status(string merchantRefNum)
    {
        var result = await _fawry.GetPaymentStatusAsync(merchantRefNum);
        return Ok(result);
    }

    // ✅ دفع المحادثة (Chat Payment)
    [HttpPost("chat-payment")]
    public async Task<IActionResult> ChatPayment()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();

        var user = await _userManager.FindByIdAsync(userIdClaim);
        if (user == null) return NotFound();

        user.HasPaidForChat = true;
        await _userManager.UpdateAsync(user);

        return Ok(new { success = true, message = "تم تفعيل المحادثة غير المحدودة بنجاح." });
    }
}
