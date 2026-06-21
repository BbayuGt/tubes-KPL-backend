namespace tubes_KPL_backend.DTOs
{
    public class CreateDonationRequestDTO
    {

        public int CampaignId { get; set; }
        public decimal Amount { get; set; }
        public string DonorName { get; set; }
        public string DonorEmail { get; set; }

    }
}
