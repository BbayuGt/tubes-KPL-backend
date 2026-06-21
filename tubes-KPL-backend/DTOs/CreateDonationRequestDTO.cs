namespace tubes_KPL_backend.DTOs
{
    public class CreateDonationRequestDTO
    {

        public int CampaignId { get; set; }
        public decimal Amount { get; set; }
        public int UserId { get; set; }
        
    }
}
