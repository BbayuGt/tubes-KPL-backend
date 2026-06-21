using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;
using tubes_KPL_backend.Models;
using tubes_KPL_backend.Data;
using tubes_KPL_backend.Repositories;

namespace tubes_KPL_backend.Controllers
{
    [ApiController]
    [Route("api/webhook")]
    public class WebhookController : ControllerBase
    {
        private readonly XenditSettings _xendit;
        private readonly IGenericRepository<Payment> _repository;
        private readonly IGenericRepository<Campaign> _campaignRepository;
        private readonly IGenericRepository<Donation> _donationRepository;

        // Table-Driven Dictionary Mapping
        private readonly Dictionary<string, Func<Payment, JsonElement, Task>> _statusHandlers;

        public WebhookController(
            IOptions<XenditSettings> xendit, 
            IGenericRepository<Payment> repository,
            IGenericRepository<Campaign> campaignRepository,
            IGenericRepository<Donation> donationRepository)
        {
            _xendit = xendit.Value;
            _repository = repository;
            _campaignRepository = campaignRepository;
            _donationRepository = donationRepository;

            // Mapping status webhook ke handler
            _statusHandlers = new Dictionary<string, Func<Payment, JsonElement, Task>>
            {
                { PaymentStatus.SUCCEEDED, HandleSuccessPayment },
                { PaymentStatus.EXPIRED, HandleExpiredPayment },
                { PaymentStatus.FAILED, HandleFailedPayment }
            };
        }

        [HttpPost("xendit")]
        public async Task<IResult> HandleWebhook([FromBody] JsonElement payload)
        {
            // Ambil callback token dari header
            var callbackToken = Request.Headers["x-callback-token"].ToString();

            // Security validation
            if (callbackToken != _xendit.CallbackToken)
            {
                return Results.Content("Invalid callback token", statusCode: 403);
            }

            // Ambil data webhook
            var status = payload.GetProperty("status").GetString();
            var externalId = payload.GetProperty("external_id").GetString();

            Console.WriteLine($"Webhook masuk: {externalId} - {status}");

            // Cari data di database berdasarkan ExternalId
            var payment = await _repository.GetByExpression(p => p.ExternalId == externalId);

            if (payment != null)
            {
                // Table-Driven Processing
                if (_statusHandlers.TryGetValue(status, out var handler))
                {
                    if (status == null)
                    {
                        return Results.Content("Invalid status", statusCode: 403);
                    }
                    await _statusHandlers[status](payment, payload);
                }

                // Save perubahan ke database
                await _repository.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine($"Payment dengan ExternalId {externalId} tidak ditemukan.");
            }

            return Results.Ok(new
            {
                success = true,
                message = "Webhook processed"
            });
        }

        // HANDLER: SUCCESS PAYMENT
        private async Task HandleSuccessPayment(Payment payment, JsonElement payload)
        {
            payment.Status = PaymentStatus.SUCCEEDED;

            // Ambil paid_at jika ada
            if (payload.TryGetProperty("paid_at", out var paidAtProp))
            {
                payment.PaidAt = paidAtProp.GetDateTimeOffset().UtcDateTime;
            }
            else
            {
                payment.PaidAt = DateTime.UtcNow;
            }

            Console.WriteLine($"Database updated: Pembayaran sukses untuk {payment.ExternalId}");
            _repository.Update(payment);

            var parts = payment.ExternalId.Split('-');
            if (parts.Length >= 3 && int.TryParse(parts[1], out int campaignId) && campaignId > 0)
            {
                var campaign = await _campaignRepository.GetByExpression(c => c.Id == campaignId);
                if (campaign != null)
                {
                    campaign.CollectedAmount += payment.Amount;
                    _campaignRepository.Update(campaign);
                    Console.WriteLine($"Campaign {campaign.Id} updated with {payment.Amount}");

                    var donorName = payment.Description.StartsWith("Donation by ") 
                        ? payment.Description.Substring("Donation by ".Length) 
                        : "Hamba Allah";

                    var donation = new Donation
                    {
                        CampaignId = campaignId,
                        DonorName = donorName,
                        DonorEmail = payment.PayerEmail,
                        Amount = payment.Amount,
                        CreatedDate = DateTime.UtcNow
                    };

                    await _donationRepository.AddAsync(donation);
                    Console.WriteLine($"Donation record created for {donorName}");
                }
            }

            await Task.CompletedTask;
        }

        // HANDLER: EXPIRED PAYMENT
        private async Task HandleExpiredPayment(Payment payment, JsonElement payload)
        {
            payment.Status = PaymentStatus.EXPIRED;

            Console.WriteLine($"Database updated: Invoice expired untuk {payment.ExternalId}");
            _repository.Update(payment);
            await Task.CompletedTask;
        }

        // HANDLER: FAILED PAYMENT
        private async Task HandleFailedPayment(Payment payment, JsonElement payload)
        {
            payment.Status = PaymentStatus.FAILED;

            Console.WriteLine($"Database updated: Pembayaran gagal untuk {payment.ExternalId}");
            _repository.Update(payment);
            await Task.CompletedTask;
        }
    }
}