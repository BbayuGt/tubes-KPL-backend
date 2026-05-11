using tubes_KPL_backend.Data;
using tubes_KPL_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace tubes_KPL_backend.Services
{
    public class CampaignService
    {
        private readonly AppDbContext _context;

        public CampaignService(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL
        public async Task<List<Campaign>> GetAllCampaigns()
        {
            return await _context.Campaigns.ToListAsync();
        }

        // GET BY ID
        public async Task<Campaign?> GetCampaignById(int id)
        {
            return await _context.Campaigns.FindAsync(id);
        }

        // CREATE
        public async Task<Campaign> CreateCampaign(Campaign campaign)
        {
            // cek duplicate id
            var existingCampaign = await _context.Campaigns.FindAsync(campaign.Id);
            if (existingCampaign != null)
            {
                return null;
            }
            _context.Campaigns.Add(campaign);
            await _context.SaveChangesAsync();
            return campaign;
        }

        // UPDATE
        public async Task<bool> UpdateCampaign(int id, Campaign updatedCampaign)
        {
            var campaign = await _context.Campaigns.FindAsync(id);

            if (campaign == null)
                return false;

            campaign.Title = updatedCampaign.Title;
            campaign.Description = updatedCampaign.Description;
            campaign.TargetAmount = updatedCampaign.TargetAmount;
            campaign.ImageUrl = updatedCampaign.ImageUrl;
            campaign.IsActive = updatedCampaign.IsActive;

            await _context.SaveChangesAsync();
            return true;
        }

        // DELETE
        public async Task<bool> DeleteCampaign(int id)
        {
            var campaign = await _context.Campaigns.FindAsync(id);

            if (campaign == null)
                return false;

            _context.Campaigns.Remove(campaign);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}