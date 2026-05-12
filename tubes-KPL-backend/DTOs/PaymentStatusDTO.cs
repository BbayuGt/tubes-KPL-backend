namespace tubes_KPL_backend.DTOs;

public class PaymentStatusDTO
{
    public string ExternalId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PayerEmail { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string InvoiceUrl { get; set; } = string.Empty;
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiryDate { get; set; }
}
