using Khedmetak.BLL.DTOS.Fawry;
using Khedmetak.DAL.Entities.FawrySettings;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

public class FawryService
{
    private readonly HttpClient _http;
    private readonly FawrySettings _settings;

    public FawryService(HttpClient http, IOptions<FawrySettings> options)
    {
        _http = http;
        _settings = options.Value;
    }

    // ✅ توليد الـ Signature
    private string GenerateSignature(string merchantCode, string merchantRefNum,
        string customerProfileId, string paymentMethod, decimal amount, string securityKey)
    {
        // فوري بيحسب الـ hash بالترتيب ده
        string raw = merchantCode + merchantRefNum + customerProfileId +
                     paymentMethod + amount.ToString("F2") + securityKey;

        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(raw));
        return Convert.ToHexString(bytes).ToLower();
    }

    // ✅ إنشاء طلب دفع (الاتنين — PAYATFAWRY أو CARD)
    public async Task<FawryChargeResponse> CreateChargeAsync(FawryChargeRequest request)
    {
        var merchantRefNum = Guid.NewGuid().ToString("N")[..16];

        var signature = GenerateSignature(
            _settings.MerchantCode,
            merchantRefNum,
            request.CustomerProfileId,
            request.PaymentMethod, // "PAYATFAWRY" أو "CARD"
            request.Amount,
            _settings.SecurityKey
        );

        var payload = new
        {
            merchantCode = _settings.MerchantCode,
            merchantRefNum = merchantRefNum,
            customerProfileId = request.CustomerProfileId,
            customerName = request.CustomerName,
            customerEmail = request.CustomerEmail,
            customerMobile = request.CustomerMobile,
            paymentMethod = request.PaymentMethod,
            amount = request.Amount,
            currencyCode = "EGP",
            description = request.Description,
            chargeItems = request.Items.Select(i => new
            {
                itemId = i.ItemId,
                description = i.Description,
                price = i.Price,
                quantity = i.Quantity
            }),
            returnUrl = _settings.ReturnUrl,
            authCaptureModePayment = false,
            signature = signature
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _http.PostAsync($"{_settings.BaseUrl}/ECommerceWeb/Fawry/payments/charge", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<FawryChargeResponse>(responseBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    // ✅ الاستعلام عن حالة الدفع
    public async Task<FawryStatusResponse> GetPaymentStatusAsync(string merchantRefNum)
    {
        string raw = _settings.MerchantCode + merchantRefNum + _settings.SecurityKey;
        using var sha256 = SHA256.Create();
        var signature = Convert.ToHexString(sha256.ComputeHash(Encoding.UTF8.GetBytes(raw))).ToLower();

        var url = $"{_settings.BaseUrl}/ECommerceWeb/Fawry/payments/status/v2?" +
                  $"merchantCode={_settings.MerchantCode}&merchantRefNum={merchantRefNum}&signature={signature}";

        var response = await _http.GetAsync(url);
        var body = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<FawryStatusResponse>(body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
}