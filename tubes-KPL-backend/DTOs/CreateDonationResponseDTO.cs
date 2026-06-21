namespace tubes_KPL_backend.DTOs
{
    public class CreateDonationResponseDTO
    {
        //DonationId = donation.Id,
        //CampaignId = campaign.Id,
        //DonationAmount = donation.Amount,
        //UpdatedCampaignTotal = campaign.CollectedAmount,
        //CreatedDate = donation.CreatedDate
        public int DonationId { get; set; } 
        public int CampaignId { get; set; }
        public decimal DonationAmount { get; set; }
        public decimal UpdatedCampaignTotal { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
