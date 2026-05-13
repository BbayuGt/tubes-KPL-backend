using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;
using tubes_KPL_backend.Models;
using tubes_KPL_backend.Data;

namespace tubes_KPL_backend.Controllers
{
    [ApiController]
    [Route("api/webhook")]
    public class WebhookController : ControllerBase
    {
        private readonly XenditSettings _xendit;
        private readonly AppDbContext _db;

        // Tambahkan AppDbContext di sini
        public WebhookController(IOptions<XenditSettings> xendit, AppDbContext db)
        {
            _xendit = xendit.Value;
            _db = db;
        }

        [HttpPost("xendit")]
        public async Task<IActionResult> HandleWebhook([FromBody] JsonElement payload)
        {
            var callbackToken = Request.Headers["x-callback-token"].ToString();

            // Security check
            if (callbackToken != _xendit.CallbackToken)
            {
                return StatusCode(403, "Invalid callback token");
            }

            // Ambil data dari webhook
            var status = payload.GetProperty("status").GetString();
            var externalId = payload.GetProperty("external_id").GetString();

            Console.WriteLine($"Webhook masuk: {externalId} - {status}");

            // Cari data di database berdasarkan ExternalId
            var payment = await _db.Payments.FirstOrDefaultAsync(p => p.ExternalId == externalId);

            if (payment != null)
            {
                if (status == PaymentStatus.SUCCEEDED)
                {
                    payment.Status = PaymentStatus.SUCCEEDED;
                    // Pakai TryGetProperty untuk paid_at karena kadang formatnya bisa beda
                    if (payload.TryGetProperty("paid_at", out var paidAtProp))
                    {
                        payment.PaidAt = paidAtProp.GetDateTimeOffset().UtcDateTime;
                    }
                    else
                    {
                        payment.PaidAt = DateTime.UtcNow;
                    }
                    
                    Console.WriteLine($"Database updated: Pembayaran sukses untuk {externalId}");
                }
                else if (status == PaymentStatus.EXPIRED)
                {
                    payment.Status = PaymentStatus.EXPIRED;
                    Console.WriteLine($"Database updated: Invoice expired untuk {externalId}");
                }
                else if (status == PaymentStatus.FAILED)
                {
                    payment.Status = PaymentStatus.FAILED;
                }

                // Save perubahan ke database
                await _db.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine($"Payment dengan ExternalId {externalId} tidak ditemukan di database.");
            }

            return Ok(new
            {
                success = true,
                message = "Webhook processed"
            });
        }
    }
}
