using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;
using tubes_KPL_backend.Controllers;
using tubes_KPL_backend.Data;
using tubes_KPL_backend.Models;
using Xunit;
using tubes_KPL_backend.Repositories;

namespace UnitTesting_KPL.Controllers
{
    public class WebhookControllerTest
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
        public async Task HandleWebhook_ShouldReturnForbidden_WhenTokenInvalid()
        {
            // Arrange

            var dbContext = GetDbContext();

            // Setup Xendit settings dummy
            var xenditSettings = Options.Create(new XenditSettings
            {
                SecretKey = "dummy-secret",
                CallbackToken = "TOKEN_BENAR"
            });

            // Membuat controller
            var controller = new WebhookController(
                xenditSettings,
                new GenericRepository<Payment>(dbContext)
            );

            // Membuat HttpContext palsu
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Mengisi token salah pada header
            controller.HttpContext.Request.Headers["x-callback-token"] = "TOKEN_SALAH";

            // Membuat payload dummy webhook
            var payload = JsonDocument.Parse(@"
            {
                ""status"": ""PAID"",
                ""external_id"": ""payment-001""
            }").RootElement;

            // Act

            var result = await controller.HandleWebhook(payload);

            // Assert

            // Memastikan response berupa ObjectResult
            var objectResult = Assert.IsType<ObjectResult>(result);

            // Memastikan status code 403
            Assert.Equal(403, objectResult.StatusCode);
        }

        [Fact]
        public async Task HandleWebhook_ShouldUpdatePaymentStatusToPaid()
        {
            // Arrange

            var dbContext = GetDbContext();

            // Menambahkan payment dummy
            dbContext.Payments.Add(new Payment
            {
                Id = 1,
                ExternalId = "payment-001",
                Amount = 100000,
                PayerEmail = "test@gmail.com",
                Description = "Test Payment",
                Status = "PENDING",
                InvoiceUrl = "https://invoice.com",
                CreatedAt = DateTime.UtcNow
            });

            await dbContext.SaveChangesAsync();

            // Setup Xendit settings dummy
            var xenditSettings = Options.Create(new XenditSettings
            {
                SecretKey = "dummy-secret",
                CallbackToken = "TOKEN_BENAR"
            });

            // Membuat controller
            var controller = new WebhookController(
                xenditSettings,
                new GenericRepository<Payment>(dbContext)
            );

            // Membuat HttpContext palsu
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Mengisi token benar pada header
            controller.HttpContext.Request.Headers["x-callback-token"] = "TOKEN_BENAR";

            // Payload webhook status PAID
            var payload = JsonDocument.Parse(@"
            {
                ""status"": ""SUCCEEDED"",
                ""external_id"": ""payment-001"",
                ""paid_at"": ""2025-01-01T10:00:00Z""
            }").RootElement;

            // Act

            var result = await controller.HandleWebhook(payload);

            // Mengambil data payment terbaru dari database
            var payment = await dbContext.Payments
                .FirstOrDefaultAsync(p => p.ExternalId == "payment-001");

            // Assert

            // Memastikan response berupa OK
            Assert.IsType<OkObjectResult>(result);

            // Memastikan payment tidak null
            Assert.NotNull(payment);

            // Memastikan status berubah menjadi SUCCEEDED
            Assert.Equal("SUCCEEDED", payment!.Status);

            // Memastikan PaidAt terisi
            Assert.NotNull(payment.PaidAt);
        }

        [Fact]
        public async Task HandleWebhook_ShouldUpdatePaymentStatusToExpired()
        {
            // Arrange

            var dbContext = GetDbContext();

            // Menambahkan payment dummy
            dbContext.Payments.Add(new Payment
            {
                Id = 1,
                ExternalId = "payment-002",
                Amount = 50000,
                PayerEmail = "expired@gmail.com",
                Description = "Expired Payment",
                Status = "PENDING",
                InvoiceUrl = "https://invoice.com",
                CreatedAt = DateTime.UtcNow
            });

            await dbContext.SaveChangesAsync();

            // Setup Xendit settings dummy
            var xenditSettings = Options.Create(new XenditSettings
            {
                SecretKey = "dummy-secret",
                CallbackToken = "TOKEN_BENAR"
            });

            // Membuat controller
            var controller = new WebhookController(
                xenditSettings,
                new GenericRepository<Payment>(dbContext)
            );

            // Membuat HttpContext palsu
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Mengisi token benar pada header
            controller.HttpContext.Request.Headers["x-callback-token"] = "TOKEN_BENAR";

            // Payload webhook status EXPIRED
            var payload = JsonDocument.Parse(@"
            {
                ""status"": ""EXPIRED"",
                ""external_id"": ""payment-002""
            }").RootElement;

            // Act

            var result = await controller.HandleWebhook(payload);

            // Mengambil payment terbaru dari database
            var payment = await dbContext.Payments
                .FirstOrDefaultAsync(p => p.ExternalId == "payment-002");

            // Assert

            // Memastikan response berupa OK
            Assert.IsType<OkObjectResult>(result);

            // Memastikan payment tidak null
            Assert.NotNull(payment);

            // Memastikan status berubah menjadi EXPIRED
            Assert.Equal("EXPIRED", payment!.Status);
        }
    }
}