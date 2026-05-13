using Microsoft.EntityFrameworkCore;
using tubes_KPL_backend.Data;
using tubes_KPL_backend.DTOs;
using tubes_KPL_backend.Models;
using tubes_KPL_backend.Repositories;

namespace tubes_KPL_backend.Services
{
    public class DonationService
    {
        private readonly IGenericRepository<User> _UserRepository;
        private readonly IGenericRepository<Donation> _DonationRepository;
        private readonly IGenericRepository<Campaign>  _CampaignRepository;
        public DonationService(IGenericRepository<User> urepository, IGenericRepository<Donation> donationRepository,  IGenericRepository<Campaign> campaignRepository)
        {
            _UserRepository = urepository;
            _DonationRepository = donationRepository;
            _CampaignRepository = campaignRepository;
        }
        public async Task<Donation?> GetDonationByIdAsync(int id)
        {
            var donations = await _DonationRepository.GetByExpression(u => u.Id == id);
            return donations;
        }
        public async Task<IEnumerable<Donation>> GetAllDonationsAsync()
        {
            var donations = await _DonationRepository.GetAllAsync();
            return donations;
        }

        public async Task<CreateDonationResponseDTO> CreateDonationAsync(CreateDonationRequestDTO request)
        {
            // Validasi nilai donasi wajib > 0 agar tidak ada transaksi nominal nol/negatif.
            if (request.Amount <= 0)
            {
                throw new ArgumentException("Nominal donasi harus lebih dari 0.");
            }

            // Pastikan user (donatur) valid.
            var userExists = await _UserRepository.ExistsAsync(u => u.Id == request.UserId);
            if (!userExists)
            {
                throw new KeyNotFoundException("User tidak ditemukan.");
            }

            // Ambil campaign tujuan yang akan ditambahkan total dananya.
            var campaign = await _CampaignRepository.GetByExpression(c => c.Id == request.CampaignId);
            if (campaign == null)
            {
                throw new KeyNotFoundException("Campaign tidak ditemukan.");
            }

            // Atomic transaction: catat donasi + update total campaign harus sukses bersama.
            var donation = new Donation
            {
                UserId = request.UserId,
                CampaignId = request.CampaignId,
                Amount = request.Amount,
                CreatedDate = DateTime.UtcNow
            };

            _DonationRepository.AddAsync(donation);

            // Update akumulasi dana campaign secara sistematis setelah donasi valid.
            campaign.CollectedAmount += request.Amount;

            await _DonationRepository.SaveChangesAsync();

            return new CreateDonationResponseDTO
            {
                DonationId = donation.Id,
                CampaignId = campaign.Id,
                DonationAmount = donation.Amount,
                UpdatedCampaignTotal = campaign.CollectedAmount,
                CreatedDate = donation.CreatedDate
            };
        }

        public async Task<bool> DeleteDonationAsync(int id)
        {
            var donation = await _DonationRepository.GetByExpression(d => d.Id == id);
            if (donation == null)
            {
                return false;
            }

            var campaign = await _CampaignRepository.GetByExpression(c => c.Id == donation.CampaignId);
            if (campaign == null)
            {
                throw new KeyNotFoundException("Campaign tidak ditemukan untuk donasi ini.");
            }

            // Atomic transaction: hapus donasi + koreksi total campaign harus sinkron.
            _DonationRepository.Delete(donation);
            campaign.CollectedAmount -= donation.Amount;
            if (campaign.CollectedAmount < 0)
            {
                campaign.CollectedAmount = 0;
            }

            await _DonationRepository.SaveChangesAsync();

            return true;
        }
    }
}
