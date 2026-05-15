using System.Text.Json.Serialization;
namespace Ecommerce.Core.DTOs;
public class PaymobWebhookPayload
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;  // "TRANSACTION"

    [JsonPropertyName("obj")]
    public PaymobTransactionObj Obj { get; set; } = null!;
}

public class PaymobTransactionObj
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("pending")]
    public bool Pending { get; set; }

    [JsonPropertyName("amount_cents")]
    public int AmountCents { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("is_auth")]
    public bool IsAuth { get; set; }

    [JsonPropertyName("is_capture")]
    public bool IsCapture { get; set; }

    [JsonPropertyName("is_standalone_payment")]
    public bool IsStandalonePayment { get; set; }

    [JsonPropertyName("is_voided")]
    public bool IsVoided { get; set; }

    [JsonPropertyName("is_refunded")]
    public bool IsRefunded { get; set; }

    [JsonPropertyName("is_3d_secure")]
    public bool Is3dSecure { get; set; }

    [JsonPropertyName("is_refund")]
    public bool IsRefund { get; set; }

    [JsonPropertyName("is_void")]
    public bool IsVoid { get; set; }

    [JsonPropertyName("error_occured")]
    public bool ErrorOccured { get; set; }

    [JsonPropertyName("has_parent_transaction")]
    public bool HasParentTransaction { get; set; }

    [JsonPropertyName("owner")]
    public int Owner { get; set; }

    [JsonPropertyName("integration_id")]
    public int IntegrationId { get; set; }

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; } = string.Empty;

    [JsonPropertyName("special_reference")]
    public string? SpecialReference { get; set; }   // ← this is your ORDER-{id} reference

    [JsonPropertyName("order")]
    public PaymobOrderRef Order { get; set; } = null!;

    [JsonPropertyName("source_data")]
    public PaymobSourceData SourceData { get; set; } = null!;
}

public class PaymobOrderRef
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
}

public class PaymobSourceData
{
    [JsonPropertyName("pan")]
    public string Pan { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("sub_type")]
    public string SubType { get; set; } = string.Empty;
}