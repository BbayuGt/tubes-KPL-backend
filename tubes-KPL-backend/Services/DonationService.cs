using tubes_KPL_backend.DTOs;
using tubes_KPL_backend.Models;
using tubes_KPL_backend.Repositories;

namespace tubes_KPL_backend.Services
{
    public class DonationService
    {
        private readonly IGenericRepository<Donation> _DonationRepository;
        private readonly IGenericRepository<Campaign> _CampaignRepository;

    public DonationService(
        IGenericRepository<Donation> donationRepository,
        IGenericRepository<Campaign> campaignRepository)
        {
            _DonationRepository = donationRepository;
            _CampaignRepository = campaignRepository;
        }

        public async Task<Donation?> GetDonationByIdAsync(int id)
        {
            return await _DonationRepository.GetByExpression(d => d.Id == id);
        }

        public async Task<IEnumerable<Donation>> GetAllDonationsAsync()
        {
            return await _DonationRepository.GetAllAsync();
        }

        public async Task<CreateDonationResponseDTO> CreateDonationAsync(CreateDonationRequestDTO request)
        {
            // Validasi nominal
            if (request.Amount <= 0)
            {
                throw new ArgumentException("Nominal donasi harus lebih dari 0.");
            }

            // Validasi nama
            if (string.IsNullOrWhiteSpace(request.DonorName))
            {
                throw new ArgumentException("Nama donatur wajib diisi.");
            }

            // Validasi email
            if (string.IsNullOrWhiteSpace(request.DonorEmail))
            {
                throw new ArgumentException("Email donatur wajib diisi.");
            }

            // Cari campaign
            var campaign = await _CampaignRepository.GetByExpression(c => c.Id == request.CampaignId);

            if (campaign == null)
            {
                throw new KeyNotFoundException("Campaign tidak ditemukan.");
            }

            // Buat data donasi
            var donation = new Donation
            {
                CampaignId = request.CampaignId,
                DonorName = request.DonorName,
                DonorEmail = request.DonorEmail,
                Amount = request.Amount,
                CreatedDate = DateTime.UtcNow
            };

            await _DonationRepository.AddAsync(donation);
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
