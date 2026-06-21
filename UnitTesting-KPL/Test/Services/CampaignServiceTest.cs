using Microsoft.EntityFrameworkCore;
using tubes_KPL_backend.Data;
using tubes_KPL_backend.Models;
using tubes_KPL_backend.Services;
using Xunit;
using tubes_KPL_backend.Repositories;

namespace UnitTesting_KPL.Services
{
    public class CampaignServiceTest
    {
        // Membuat database sementara untuk testing
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task CreateCampaign_ShouldAddCampaign()
        {
            // Arrange = menyiapkan database dan service
            var dbContext = GetDbContext();

            var service = new CampaignService(new GenericRepository<Campaign>(dbContext));

            var campaign = new Campaign
            {
                Id = 1,

                // Wajib diisi karena non-nullable
                Title = "Bantu Pendidikan",

                // Wajib diisi karena non-nullable
                Description = "Campaign untuk pendidikan",

                // Wajib diisi karena non-nullable
                ImageUrl = "image.jpg",

                TargetAmount = 100000
            };

            // Act = menjalankan method create campaign
            var result = await service.CreateCampaign(campaign);

            // Assert = memastikan hasil sesuai harapan

            // Memastikan campaign berhasil dibuat
            Assert.NotNull(result);

            // Memastikan data masuk ke database
            Assert.Equal(1, dbContext.Campaigns.Count());
        }

        [Fact]
        public async Task GetCampaignById_ShouldReturnCampaign()
        {
            // Arrange
            var dbContext = GetDbContext();

            dbContext.Campaigns.Add(new Campaign
            {
                Id = 1,
                Title = "Campaign Test",
                Description = "Description Test",
                ImageUrl = "test.jpg",
                TargetAmount = 50000
            });

            await dbContext.SaveChangesAsync();

            var service = new CampaignService(new GenericRepository<Campaign>(dbContext));

            // Act
            var result = await service.GetCampaignById(1);

            // Assert

            // Memastikan campaign ditemukan
            Assert.NotNull(result);

            // Memastikan title sesuai
            Assert.Equal("Campaign Test", result.Title);
        }

        [Fact]
        public async Task DeleteCampaign_ShouldReturnTrue_WhenCampaignExists()
        {
            // Arrange
            var dbContext = GetDbContext();

            dbContext.Campaigns.Add(new Campaign
            {
                Id = 1,
                Title = "Campaign Delete",
                Description = "Delete Description",
                ImageUrl = "delete.jpg",
                TargetAmount = 20000
            });

            await dbContext.SaveChangesAsync();

            var service = new CampaignService(new GenericRepository<Campaign>(dbContext));

            // Act
            var result = await service.DeleteCampaign(1);

            // Assert

            // Memastikan delete berhasil
            Assert.True(result);

            // Memastikan database kosong setelah delete
            Assert.Empty(dbContext.Campaigns);
        }

        [Fact]
        public async Task CreateCampaign_ShouldReturnNull_WhenIdAlreadyExists()
        {
            // Arrange
            var dbContext = GetDbContext();

            // Menambahkan campaign awal
            dbContext.Campaigns.Add(new Campaign
            {
                Id = 1,
                Title = "Campaign Lama",
                Description = "Description Lama",
                ImageUrl = "lama.jpg",
                TargetAmount = 10000
            });

            await dbContext.SaveChangesAsync();

            var service = new CampaignService(new GenericRepository<Campaign>(dbContext));

            var duplicateCampaign = new Campaign
            {
                Id = 1,
                Title = "Campaign Baru",
                Description = "Description Baru",
                ImageUrl = "baru.jpg",
                TargetAmount = 50000
            };

            // Act
            var result = await service.CreateCampaign(duplicateCampaign);

            // Assert

            // Harus null karena ID sudah dipakai
            Assert.Null(result);
        }
    }
}