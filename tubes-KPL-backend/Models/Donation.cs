using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace tubes_KPL_backend.Models
{
    [PrimaryKey(nameof(Id))]
    public class Donation
    {
        public int Id { get; set; }

        // Campaign tujuan donasi
        public int CampaignId { get; set; }

        // Nama donatur (opsional login)
        public string DonorName { get; set; }

        // Email donatur
        public string DonorEmail { get; set; }

        // Nominal donasi
        public decimal Amount { get; set; }

        // Waktu donasi
        public DateTime CreatedDate { get; set; }

        // Relasi ke Campaign
        [ForeignKey(nameof(CampaignId))]
        public Campaign? Campaign { get; set; }
    }
}
