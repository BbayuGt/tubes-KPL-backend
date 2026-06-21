using tubes_KPL_backend.Data;
using tubes_KPL_backend.Models;

namespace tubes_KPL_backend.Services
{
    public class WebhookService
    {
        private readonly AppDbContext _context;

        private readonly Dictionary<string, Func<WebhookPayload, Task>> _eventHandlers; // Digunakan sebagai tabel mapping event.

        public WebhookService(AppDbContext context)
        {
            _context = context;

            _eventHandlers = new Dictionary<string, Func<WebhookPayload, Task>>
            {
                { "donation.success", HandleDonationSuccess },
                { "donation.failed", HandleDonationFailed },    // event dipetakan dalam bentuk tabel dictionary.
                { "campaign.closed", HandleCampaignClosed }
            };
        }

        public async Task<bool> ProcessWebhook(WebhookPayload payload)  //membaca event webhook
        {
            if (_eventHandlers.TryGetValue(payload.EventType, out var handler))
            {
                await handler(payload);
                return true;
            }

            return false;
        }

        private async Task HandleDonationSuccess(WebhookPayload payload)
        {
            var campaign = await _context.Campaigns.FindAsync(payload.CampaignId);

            if (campaign == null)
                return;

            campaign.CollectedAmount += payload.Amount;     //menambahkan total donasi campaign

            await _context.SaveChangesAsync();
        }

        private async Task HandleDonationFailed(WebhookPayload payload)
        {
            Console.WriteLine("Donasi gagal diproses");

            await Task.CompletedTask;
        }

        private async Task HandleCampaignClosed(WebhookPayload payload) //menutup campaign otomatis
        {
            var campaign = await _context.Campaigns.FindAsync(payload.CampaignId);

            if (campaign == null)
                return;

            campaign.IsActive = false;

            await _context.SaveChangesAsync();
        }
    }
}