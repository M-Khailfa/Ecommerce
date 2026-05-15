using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Ecommerce.Core.Interfaces;
using Ecommerce.Core.Settings;

namespace Ecommerce.Infrastructure.Repos;

public class PaymobService(HttpClient httpClient, Paymob paymob) : IPaymobService
{
    public async Task<string> CreatePaymentIntentionAsync(decimal totalAmount, string orderReference)
    {
        var amountInCents = (int)(totalAmount * 100);

        var payload = new
        {
            amount = amountInCents,
            currency = "EGP",
            payment_methods = new[] { "card", "kiosk", "wallet" },
            items = Array.Empty<object>(),
            special_reference = orderReference,
            billing_data = new
            {
                first_name = "Customer",
                last_name = "N/A",
                email = "noreply@store.com",
                phone_number = "+201000000000",
                street = "N/A",
                building = "N/A",
                floor = "N/A",
                apartment = "N/A",
                city = "Cairo",
                country = "EG"
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "intention/")
        {
            Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json")
        };
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Token", paymob.SecretKey);

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<JsonElement>();
        return data.GetProperty("client_secret").GetString()!;
    }
}