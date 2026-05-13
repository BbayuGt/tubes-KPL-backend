using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace tubes_KPL_backend.Models;

public class Payment
{
    public int Id { get; set; }
    [Key]
    public string ExternalId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PayerEmail { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "PENDING";
    public string InvoiceUrl { get; set; } = string.Empty;
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiryDate { get; set; }
}