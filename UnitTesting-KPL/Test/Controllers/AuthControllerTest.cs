using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using tubes_KPL_backend.Controllers;
using tubes_KPL_backend.Data;
using tubes_KPL_backend.DTOs;
using tubes_KPL_backend.Models;
using tubes_KPL_backend.Services;
using Xunit;

namespace UnitTesting_KPL.Controllers
{
    public class AuthControllerTest
    {
        // Membuat database sementara untuk testing
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        // Membuat AuthService dummy untuk testing controller
        private AuthService GetAuthService(AppDbContext context)
        {
            // Konfigurasi JWT dummy
            var settings = new Dictionary<string, string?>
            {
                {"Jwt:Key", "THIS_IS_SECRET_KEY_FOR_TESTING_12345"},
                {"Jwt:Issuer", "TestIssuer"},
                {"Jwt:Audience", "TestAudience"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            var httpContextAccessor = new HttpContextAccessor();

            return new AuthService(
                context,
                configuration,
                httpContextAccessor
            );
        }

        [Fact]
        public async Task Register_ShouldReturnOk_WhenRegisterSuccess()
        {
            // Arrange
            var dbContext = GetDbContext();

            var authService = GetAuthService(dbContext);

            var controller = new AuthController(
                dbContext,
                authService
            );

            var request = new RegisterDTO
            {
                Name = "Test User",
                Email = "test@gmail.com",
                Password = "password123"
            };

            // Act
            var result = await controller.Register(request);

            // Assert

            // Memastikan hasil berupa OK
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenLoginSuccess()
        {
            // Arrange
            var dbContext = GetDbContext();

            // Menambahkan user dummy
            dbContext.Users.Add(new User
            {
                Name = "Test User",
                Email = "test@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123")
            });

            await dbContext.SaveChangesAsync();

            var authService = GetAuthService(dbContext);

            var controller = new AuthController(
                dbContext,
                authService
            );

            var request = new LoginDTO
            {
                Email = "test@gmail.com",
                Password = "password123"
            };

            // Act
            var result = await controller.Login(request);

            // Assert

            // Memastikan hasil berupa OK
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenPasswordWrong()
        {
            // Arrange
            var dbContext = GetDbContext();

            // Menambahkan user dummy
            dbContext.Users.Add(new User
            {
                Name = "Test User",
                Email = "test@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123")
            });

            await dbContext.SaveChangesAsync();

            var authService = GetAuthService(dbContext);

            var controller = new AuthController(
                dbContext,
                authService
            );

            var request = new LoginDTO
            {
                Email = "test@gmail.com",
                Password = "passwordSalah"
            };

            // Act
            var result = await controller.Login(request);

            // Assert

            // Memastikan hasil berupa BadRequest
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}