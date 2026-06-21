namespace tubes_KPL_backend.Models
{ 
    public class WebhookPayload
    {
        public string EventType { get; set; }       //Digunakan untuk membaca jenis event webhook.
        public int CampaignId { get; set; }         //Digunakan untuk mengetahui campaign mana yang diproses.
        public decimal Amount { get; set; }         //Digunakan untuk nominal donasi.
    }
}