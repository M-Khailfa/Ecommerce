using Ecommerce.Core.Dtos.Enums;
using Ecommerce.Core.DTOs;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class WebhooksController(AppDbContext db, Paymob paymob) : ControllerBase
{
    [HttpPost("paymob")]
    public async Task<IActionResult> HandlePaymobWebhook(
        [FromQuery] string hmac,
        [FromBody] PaymobWebhookPayload payload)
    {
        if (payload.Type != "TRANSACTION")
            return Ok(); // ignore non-transaction events

        var obj = payload.Obj;

        // 1. Build concatenated string (exact field order per Paymob docs)
        var concatenated =
            $"{obj.AmountCents}{obj.CreatedAt}{obj.Currency}{obj.ErrorOccured}" +
            $"{obj.HasParentTransaction}{obj.Id}{obj.IntegrationId}{obj.Is3dSecure}" +
            $"{obj.IsAuth}{obj.IsCapture}{obj.IsRefunded}{obj.IsStandalonePayment}" +
            $"{obj.IsVoided}{obj.Order.Id}{obj.Owner}{obj.Pending}" +
            $"{obj.SourceData.Pan}{obj.SourceData.SubType}{obj.SourceData.Type}{obj.Success}";

        // 2. Validate HMAC
        using var hmacSha = new HMACSHA512(Encoding.UTF8.GetBytes(paymob.HMAC));
        var hash = hmacSha.ComputeHash(Encoding.UTF8.GetBytes(concatenated));
        var calculated = BitConverter.ToString(hash).Replace("-", "").ToLower();

        if (calculated != hmac.ToLower())
            return Unauthorized("Invalid HMAC.");

        // 3. Find the payment by Paymob's special_reference
        var reference = obj.SpecialReference; // make sure it's mapped in your DTO
        var payment = await db.Payments
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.Reference == reference);

        if (payment is null)
            return NotFound();

        // 4. Update payment and order
        if (obj.Success && !obj.IsRefund && !obj.IsVoid)
        {
            payment.Status = PaymentStatus.Completed;
            payment.PaidAt = DateTime.UtcNow;
            payment.Order.Status = OrderStatus.Processing;
        }
        else
        {
            payment.Status = PaymentStatus.Failed;
        }

        await db.SaveChangesAsync();

        return Ok(); // ← always return 200 so Paymob stops retrying
    }
}