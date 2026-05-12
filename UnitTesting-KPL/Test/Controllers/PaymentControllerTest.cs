using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using tubes_KPL_backend.Controllers;
using tubes_KPL_backend.Data;
using tubes_KPL_backend.DTOs;
using tubes_KPL_backend.Models;
using Xunit;

namespace UnitTesting_KPL.Controllers
{
    public class PaymentControllerTest
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
        public async Task GetStatus_ShouldReturnOk_WhenPaymentExists()
        {
            // Arrange

            var dbContext = GetDbContext();

            // Menambahkan data payment dummy
            dbContext.Payments.Add(new Payment
            {
                Id = 1,
                ExternalId = "payment-001",
                Amount = 100000,
                PayerEmail = "test@gmail.com",
                Description = "Test Payment",
                Status = "PAID",
                InvoiceUrl = "https://invoice.com",
                CreatedAt = DateTime.UtcNow
            });

            await dbContext.SaveChangesAsync();

            // Mock XenditSettings
            var xenditSettings = Options.Create(new XenditSettings
            {
                SecretKey = "dummy-secret-key",
                CallbackToken = "dummy-token"
            });

            // Mock HttpClientFactory
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();

            // Mock Logger
            var loggerMock = new Mock<ILogger<PaymentController>>();

            // Membuat controller
            var controller = new PaymentController(
                xenditSettings,
                httpClientFactoryMock.Object,
                loggerMock.Object,
                dbContext
            );

            // Act

            var result = await controller.GetStatus("payment-001");

            // Assert

            // Memastikan response berupa OK
            var okResult = Assert.IsType<OkObjectResult>(result);

            // Memastikan data response tidak null
            Assert.NotNull(okResult.Value);

            // Mengambil isi DTO
            var dto = Assert.IsType<PaymentStatusDTO>(okResult.Value);

            // Memastikan ExternalId sesuai
            Assert.Equal("payment-001", dto.ExternalId);

            // Memastikan status payment sesuai
            Assert.Equal("PAID", dto.Status);
        }

        [Fact]
        public async Task GetStatus_ShouldReturnNotFound_WhenPaymentDoesNotExist()
        {
            // Arrange

            var dbContext = GetDbContext();

            var xenditSettings = Options.Create(new XenditSettings
            {
                SecretKey = "dummy-secret-key",
                CallbackToken = "dummy-token"
            });

            var httpClientFactoryMock = new Mock<IHttpClientFactory>();

            var loggerMock = new Mock<ILogger<PaymentController>>();

            var controller = new PaymentController(
                xenditSettings,
                httpClientFactoryMock.Object,
                loggerMock.Object,
                dbContext
            );

            // Act

            var result = await controller.GetStatus("payment-tidak-ada");

            // Assert

            // Memastikan response berupa NotFound
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task CreateInvoice_ShouldReturnBadRequest_WhenAmountLessThanZero()
        {
            // Arrange

            var dbContext = GetDbContext();

            var xenditSettings = Options.Create(new XenditSettings
            {
                SecretKey = "dummy-secret-key",
                CallbackToken = "dummy-token"
            });

            var httpClientFactoryMock = new Mock<IHttpClientFactory>();

            var loggerMock = new Mock<ILogger<PaymentController>>();

            var controller = new PaymentController(
                xenditSettings,
                httpClientFactoryMock.Object,
                loggerMock.Object,
                dbContext
            );

            // Request dengan amount tidak valid
            var request = new CreateInvoiceRequest
            {
                Amount = 0,
                PayerEmail = "test@gmail.com",
                Description = "Test Invoice"
            };

            // Act

            var result = await controller.CreateInvoice(request);

            // Assert

            // Memastikan response berupa BadRequest
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}