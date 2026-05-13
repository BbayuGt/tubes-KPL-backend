using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using tubes_KPL_backend.Models;
using tubes_KPL_backend.Data;
using tubes_KPL_backend.DTOs;
using Microsoft.EntityFrameworkCore;

namespace tubes_KPL_backend.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly XenditSettings _xendit;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<PaymentController> _logger;
        private readonly AppDbContext _db;

        public PaymentController(
            IOptions<XenditSettings> xendit,
            IHttpClientFactory httpClientFactory,
            ILogger<PaymentController> logger,
            AppDbContext db
        )
        {
            _xendit = xendit.Value;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _db = db;
        }

        [HttpPost("create-invoice")]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceRequest request)
        {
            if (request.Amount <= 0)
            {
                return BadRequest(new { message = "Amount must be greater than 0." });
            }

            var client = _httpClientFactory.CreateClient();

            var authToken = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{_xendit.SecretKey}:")
            );

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);

            var externalId = string.IsNullOrWhiteSpace(request.ExternalId)
                    ? $"order-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}"
                    : request.ExternalId;

            var payload = new
            {
                external_id = externalId,
                amount = request.Amount,
                payer_email = request.PayerEmail,
                description = request.Description
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync(
                "https://api.xendit.co/v2/invoices",
                content
            );

            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Xendit create invoice failed. StatusCode: {StatusCode}, Response: {Response}",
                    response.StatusCode,
                    result
                );
                return StatusCode((int)response.StatusCode, result);
            }
            var invoice = JsonSerializer.Deserialize<JsonElement>(result);
            var invoiceUrl = invoice.GetProperty("invoice_url").GetString()!;
            var expiryDate = invoice.GetProperty("expiry_date").GetDateTimeOffset().UtcDateTime;

            
            //buat save database kalau error comment aowk
            var payment = new Payment
            {
                ExternalId = externalId,
                Amount = request.Amount,
                PayerEmail = request.PayerEmail,
                Description = request.Description,
                Status = PaymentStatus.PENDING,
                InvoiceUrl = invoiceUrl,
                CreatedAt = DateTime.UtcNow,
                ExpiryDate = expiryDate
            };
            await _db.Payments.AddAsync(payment);
            await _db.SaveChangesAsync();

            return Content(result, "application/json");
        }

        [HttpGet("status/{externalId}")]
        public async Task<IActionResult> GetStatus(string externalId)
        {
            var payment = await _db.Payments
                .FirstOrDefaultAsync(p => p.ExternalId == externalId);

            if (payment == null)
            {
                return NotFound(new { message = "Payment not found." });
            }

            var dto = new PaymentStatusDTO
            {
                ExternalId = payment.ExternalId,
                Amount = payment.Amount,
                PayerEmail = payment.PayerEmail,
                Description = payment.Description,
                Status = payment.Status,
                InvoiceUrl = payment.InvoiceUrl,
                PaidAt = payment.PaidAt,
                CreatedAt = payment.CreatedAt,
                ExpiryDate = payment.ExpiryDate
            };

            return Ok(dto);
        }
    }

    public class CreateInvoiceRequest
    {
        public string? ExternalId { get; set; }
        public decimal Amount { get; set; }
        public string PayerEmail { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
