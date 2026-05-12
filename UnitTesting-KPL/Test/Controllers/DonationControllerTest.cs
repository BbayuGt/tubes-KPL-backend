using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using tubes_KPL_backend.Controllers;
using tubes_KPL_backend.Data;
using tubes_KPL_backend.DTOs;
using tubes_KPL_backend.Models;
using tubes_KPL_backend.Services;
using Xunit;

namespace UnitTesting_KPL.Controllers
{
    public class DonationControllerTest
    {
        // Membuat database sementara untuk testing
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())

                // Mengabaikan warning transaction
                .ConfigureWarnings(w =>
                    w.Ignore(InMemoryEventId.TransactionIgnoredWarning))

                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetDonationById_ShouldReturnOk_WhenDonationExists()
        {
            // Arrange
            var dbContext = GetDbContext();

            // Menambahkan donation dummy
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

            var controller = new DonationController(service);

            // Act
            var result = await controller.GetDonationById(1);

            // Assert

            // Memastikan hasil berupa OK
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            // Memastikan value tidak null
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetDonationById_ShouldReturnNotFound_WhenDonationDoesNotExist()
        {
            // Arrange
            var dbContext = GetDbContext();

            var service = new DonationService(dbContext);

            var controller = new DonationController(service);

            // Act
            var result = await controller.GetDonationById(999);

            // Assert

            // Memastikan hasil berupa NotFound
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetAllDonations_ShouldReturnOkResult()
        {
            // Arrange
            var dbContext = GetDbContext();

            // Menambahkan donation dummy
            dbContext.Donations.Add(new Donation
            {
                Id = 1,
                UserId = 1,
                CampaignId = 1,
                Amount = 10000,
                CreatedDate = DateTime.UtcNow
            });

            dbContext.Donations.Add(new Donation
            {
                Id = 2,
                UserId = 2,
                CampaignId = 1,
                Amount = 20000,
                CreatedDate = DateTime.UtcNow
            });

            await dbContext.SaveChangesAsync();

            var service = new DonationService(dbContext);

            var controller = new DonationController(service);

            // Act
            var result = await controller.GetAllDonations();

            // Assert

            // Memastikan hasil berupa OK
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            // Memastikan value tidak null
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task CreateDonation_ShouldReturnCreated_WhenSuccess()
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

            var controller = new DonationController(service);

            var request = new CreateDonationRequestDTO
            {
                UserId = 1,
                CampaignId = 1,
                Amount = 50000
            };

            // Act
            var result = await controller.CreateDonation(request);

            // Assert

            // Memastikan hasil berupa Created
            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public async Task CreateDonation_ShouldReturnBadRequest_WhenAmountInvalid()
        {
            // Arrange
            var dbContext = GetDbContext();

            var service = new DonationService(dbContext);

            var controller = new DonationController(service);

            var request = new CreateDonationRequestDTO
            {
                UserId = 1,
                CampaignId = 1,
                Amount = 0
            };

            // Act
            var result = await controller.CreateDonation(request);

            // Assert

            // Memastikan hasil berupa BadRequest
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeleteDonation_ShouldReturnNoContent_WhenDeleteSuccess()
        {
            // Arrange
            var dbContext = GetDbContext();

            // Menambahkan campaign dummy
            dbContext.Campaigns.Add(new Campaign
            {
                Id = 1,
                Title = "Campaign Donation",
                Description = "Description Donation",
                ImageUrl = "donation.jpg",
                TargetAmount = 100000,
                CollectedAmount = 50000
            });

            // Menambahkan donation dummy
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

            var controller = new DonationController(service);

            // Act
            var result = await controller.DeleteDonation(1);

            // Assert

            // Memastikan hasil berupa NoContent
            Assert.IsType<NoContentResult>(result);
        }
    }
}