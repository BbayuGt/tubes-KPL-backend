using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tubes_KPL_backend.Controllers;
using tubes_KPL_backend.Data;
using tubes_KPL_backend.Models;
using tubes_KPL_backend.Services;
using Xunit;

namespace UnitTesting_KPL.Controllers
{
    public class CampaignControllerTest
    {
        // Membuat database sementara untuk unit testing
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkResult()
        {
            // Arrange
            var dbContext = GetDbContext();

            // Menambahkan data campaign dummy
            dbContext.Campaigns.Add(new Campaign
            {
                Id = 1,
                Title = "Campaign 1",
                Description = "Description 1",
                ImageUrl = "image1.jpg",
                TargetAmount = 100000
            });

            dbContext.Campaigns.Add(new Campaign
            {
                Id = 2,
                Title = "Campaign 2",
                Description = "Description 2",
                ImageUrl = "image2.jpg",
                TargetAmount = 200000
            });

            await dbContext.SaveChangesAsync();

            var service = new CampaignService(dbContext);

            var controller = new CampaignController(service);

            // Act
            var result = await controller.GetAll();

            // Assert

            // Memastikan hasil berupa OK
            var okResult = Assert.IsType<OkObjectResult>(result);

            // Memastikan value tidak null
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenCampaignExists()
        {
            // Arrange
            var dbContext = GetDbContext();

            // Menambahkan campaign dummy
            dbContext.Campaigns.Add(new Campaign
            {
                Id = 1,
                Title = "Campaign Test",
                Description = "Description Test",
                ImageUrl = "test.jpg",
                TargetAmount = 100000
            });

            await dbContext.SaveChangesAsync();

            var service = new CampaignService(dbContext);

            var controller = new CampaignController(service);

            // Act
            var result = await controller.GetById(1);

            // Assert

            // Memastikan hasil berupa OK
            var okResult = Assert.IsType<OkObjectResult>(result);

            // Memastikan value tidak null
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenCampaignDoesNotExist()
        {
            // Arrange
            var dbContext = GetDbContext();

            var service = new CampaignService(dbContext);

            var controller = new CampaignController(service);

            // Act
            var result = await controller.GetById(999);

            // Assert

            // Memastikan hasil berupa NotFound
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Create_ShouldReturnCampaign_WhenCreateSuccess()
        {
            // Arrange
            var dbContext = GetDbContext();

            var service = new CampaignService(dbContext);

            var controller = new CampaignController(service);

            var campaign = new Campaign
            {
                Id = 1,
                Title = "Campaign Baru",
                Description = "Description Baru",
                ImageUrl = "baru.jpg",
                TargetAmount = 500000
            };

            // Act
            var result = await controller.Create(campaign);

            // Assert

            // Memastikan hasil tidak null
            Assert.NotNull(result);

            // Memastikan title sesuai
            Assert.Equal("Campaign Baru", result.Title);
        }

        [Fact]
        public async Task Update_ShouldReturnOk_WhenUpdateSuccess()
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
                TargetAmount = 100000
            });

            await dbContext.SaveChangesAsync();

            var service = new CampaignService(dbContext);

            var controller = new CampaignController(service);

            var updatedCampaign = new Campaign
            {
                Title = "Campaign Update",
                Description = "Description Update",
                ImageUrl = "update.jpg",
                TargetAmount = 999999
            };

            // Act
            var result = await controller.Update(1, updatedCampaign);

            // Assert

            // Memastikan hasil berupa OK
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnOk_WhenDeleteSuccess()
        {
            // Arrange
            var dbContext = GetDbContext();

            // Menambahkan campaign dummy
            dbContext.Campaigns.Add(new Campaign
            {
                Id = 1,
                Title = "Campaign Delete",
                Description = "Description Delete",
                ImageUrl = "delete.jpg",
                TargetAmount = 100000
            });

            await dbContext.SaveChangesAsync();

            var service = new CampaignService(dbContext);

            var controller = new CampaignController(service);

            // Act
            var result = await controller.Delete(1);

            // Assert

            // Memastikan hasil berupa OK
            Assert.IsType<OkObjectResult>(result);
        }
    }
}