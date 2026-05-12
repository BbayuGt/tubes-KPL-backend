using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using tubes_KPL_backend.Data;
using tubes_KPL_backend.DTOs;
using tubes_KPL_backend.Models;
using tubes_KPL_backend.Services;
using Xunit;

namespace UnitTesting_KPL.Services
{
    public class DonationServiceTest
    {
        // Membuat database sementara untuk unit testing
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())

                // Mengabaikan warning transaction
                // karena InMemoryDatabase tidak support transaction
                .ConfigureWarnings(w =>
                    w.Ignore(InMemoryEventId.TransactionIgnoredWarning))

                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task CreateDonationAsync_ShouldThrowException_WhenAmountIsZero()
        {
            // Arrange = menyiapkan object testing
            var dbContext = GetDbContext();

            var service = new DonationService(dbContext);

            var request = new CreateDonationRequestDTO
            {
                Amount = 0,
                UserId = 1,
                CampaignId = 1
            };

            // Act & Assert
            // Memastikan error muncul jika amount <= 0
            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.CreateDonationAsync(request));
        }

        [Fact]
        public async Task CreateDonationAsync_ShouldCreateDonation()
        {
            // Arrange
            var dbContext = GetDbContext();

            // Menambahkan user dummy
            dbContext.Users.Add(new User
            {
                Id = 1,
                Name = "Test User",
                Email = "test@gmail.com",
                PasswordHash = "hashedpassword"
            });

            // Menambahkan campaign dummy
            dbContext.Campaigns.Add(new Campaign
            {
                Id = 1,
                Title = "Campaign Test",
                Description = "Description Test",
                ImageUrl = "test.jpg",
                TargetAmount = 100000,
                CollectedAmount = 0
            });

            await dbContext.SaveChangesAsync();

            var service = new DonationService(dbContext);

            var request = new CreateDonationRequestDTO
            {
                UserId = 1,
                CampaignId = 1,
                Amount = 50000
            };

            // Act
            var result = await service.CreateDonationAsync(request);

            // Assert

            // Memastikan response tidak null
            Assert.NotNull(result);

            // Memastikan nominal donasi benar
            Assert.Equal(50000, result.DonationAmount);

            // Mengambil campaign dari database
            var campaign = await dbContext.Campaigns.FindAsync(1);

            // Memastikan campaign tidak null
            Assert.NotNull(campaign);

            // Memastikan total campaign bertambah
            Assert.Equal(50000, campaign.CollectedAmount);
        }

        [Fact]
        public async Task GetDonationByIdAsync_ShouldReturnDonation()
        {
            // Arrange
            var dbContext = GetDbContext();

            dbContext.Donations.Add(new Donation
            {
                Id = 1,
                UserId = 1,
                CampaignId = 1,
                Amount = 10000,
                CreatedDate = DateTime.UtcNow
            });

            await dbContext.SaveChangesAsync();

            var service = new DonationService(dbContext);

            // Act
            var result = await service.GetDonationByIdAsync(1);

            // Assert

            // Memastikan donasi ditemukan
            Assert.NotNull(result);

            // Memastikan amount sesuai
            Assert.Equal(10000, result.Amount);
        }

        [Fact]
        public async Task DeleteDonationAsync_ShouldReturnTrue()
        {
            // Arrange
            var dbContext = GetDbContext();

            // Menambahkan campaign
            dbContext.Campaigns.Add(new Campaign
            {
                Id = 1,
                Title = "Campaign Donation",
                Description = "Description Donation",
                ImageUrl = "donation.jpg",
                TargetAmount = 100000,
                CollectedAmount = 50000
            });

            // Menambahkan donation
            dbContext.Donations.Add(new Donation
            {
                Id = 1,
                UserId = 1,
                CampaignId = 1,
                Amount = 50000,
                CreatedDate = DateTime.UtcNow
            });

            await dbContext.SaveChangesAsync();

            var service = new DonationService(dbContext);

            // Act
            var result = await service.DeleteDonationAsync(1);

            // Assert

            // Memastikan delete berhasil
            Assert.True(result);

            // Memastikan donation sudah hilang
            Assert.Empty(dbContext.Donations);

            // Mengambil campaign dari database
            var campaign = await dbContext.Campaigns.FindAsync(1);

            // Memastikan campaign tidak null
            Assert.NotNull(campaign);

            // Memastikan total campaign menjadi 0
            Assert.Equal(0, campaign.CollectedAmount);
        }
    }
}