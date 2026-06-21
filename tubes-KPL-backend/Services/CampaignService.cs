using tubes_KPL_backend.Data;
using tubes_KPL_backend.Models;
using Microsoft.EntityFrameworkCore;
using tubes_KPL_backend.Repositories;

namespace tubes_KPL_backend.Services
{
    public class CampaignService
    {
        private readonly IGenericRepository<Campaign> _repository;

        public CampaignService(IGenericRepository<Campaign> repository)
        {
            _repository = repository;
        }

        // GET ALL
        public async Task<IEnumerable<Campaign>> GetAllCampaigns()
        {
            return await _repository.GetAllAsync();
        }

        // GET BY ID
        public async Task<Campaign?> GetCampaignById(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        // CREATE
        public async Task<Campaign> CreateCampaign(Campaign campaign)
        {
            // cek duplicate id
            var existingCampaign = await _repository.GetByIdAsync(campaign.Id);
            if (existingCampaign != null)
            {
                return null;
            }
            await _repository.AddAsync(campaign);
            await _repository.SaveChangesAsync();
            return campaign;
        }

        // UPDATE
        public async Task<bool> UpdateCampaign(int id, Campaign updatedCampaign)
        {
            var campaign = await _repository.GetByIdAsync(id);

            if (campaign == null)
                return false;

            campaign.Title = updatedCampaign.Title;
            campaign.Description = updatedCampaign.Description;
            campaign.TargetAmount = updatedCampaign.TargetAmount;
            campaign.ImageUrl = updatedCampaign.ImageUrl;
            campaign.IsActive = updatedCampaign.IsActive;

            _repository.Update(campaign);
            await _repository.SaveChangesAsync();
            return true;
        }

        // DELETE
        public async Task<bool> DeleteCampaign(int id)
        {
            var campaign = await _repository.GetByIdAsync(id);

            if (campaign == null)
                return false;

            _repository.Delete(campaign);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}