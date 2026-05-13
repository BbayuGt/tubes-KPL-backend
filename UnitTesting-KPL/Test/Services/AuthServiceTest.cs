using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using tubes_KPL_backend.Data;
using tubes_KPL_backend.Models;
using tubes_KPL_backend.Services;
using Xunit;
using tubes_KPL_backend.Repositories;

namespace UnitTesting_KPL.Services
{
    public class AuthServiceTest
    {
        // Membuat database sementara untuk testing
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        // Membuat JWT configuration palsu untuk testing
        private IConfiguration GetConfiguration()
        {
            var settings = new Dictionary<string, string?>
            {
                 {"Jwt:Key", "TEST_SECRET_KEY_123456789"},
                 {"Jwt:Issuer", "TestIssuer"},
                 {"Jwt:Audience", "TestAudience"}
          };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();
        }

        [Fact]
        public async Task RegisterUser_ShouldCreateUser()
        {
            // Arrange = menyiapkan data testing
            var dbContext = GetDbContext();

            var configuration = GetConfiguration();

            // Mock HttpContextAccessor
            var httpContextAccessor = new Mock<IHttpContextAccessor>();

            var service = new AuthService(
                new GenericRepository<User>(dbContext),
                configuration,
                httpContextAccessor.Object
            );

            // Act = menjalankan method register
            var result = await service.RegisterUser(
                "Dhaifullah",
                "dhaif@test.com",
                "password123"
            );

            // Assert = memastikan hasil sesuai harapan

            // Memastikan user berhasil dibuat
            Assert.NotNull(result);

            // Memastikan email sesuai
            Assert.Equal("dhaif@test.com", result.Email);

            // Memastikan user masuk database
            Assert.Equal(1, dbContext.Users.Count());
        }

        [Fact]
        public async Task RegisterUser_ShouldThrowException_WhenEmailAlreadyExists()
        {
            // Arrange
            var dbContext = GetDbContext();

            // Menambahkan user awal
            dbContext.Users.Add(new User
            {
                Id = 1,
                Name = "User Lama",
                Email = "test@gmail.com",
                PasswordHash = "hashedpassword"
            });

            await dbContext.SaveChangesAsync();

            var configuration = GetConfiguration();

            var httpContextAccessor = new Mock<IHttpContextAccessor>();

            var service = new AuthService(
                new GenericRepository<User>(dbContext),
                configuration,
                httpContextAccessor.Object
            );

            // Act & Assert
            // Memastikan error muncul jika email sudah dipakai
            await Assert.ThrowsAsync<BadHttpRequestException>(() =>
                service.RegisterUser(
                    "User Baru",
                    "test@gmail.com",
                    "password123"
                ));
        }

        [Fact]
        public async Task Login_ShouldThrowException_WhenPasswordIsWrong()
        {
            // Arrange
            var dbContext = GetDbContext();

            // Hash password asli
            var hashedPassword =
                BCrypt.Net.BCrypt.HashPassword("password123");

            dbContext.Users.Add(new User
            {
                Id = 1,
                Name = "Test User",
                Email = "test@gmail.com",
                PasswordHash = hashedPassword
            });

            await dbContext.SaveChangesAsync();

            var configuration = GetConfiguration();

            var httpContextAccessor = new Mock<IHttpContextAccessor>();

            var service = new AuthService(
                new GenericRepository<User>(dbContext),
                configuration,
                httpContextAccessor.Object
            );

            // Act & Assert
            // Memastikan login gagal jika password salah
            await Assert.ThrowsAsync<BadHttpRequestException>(() =>
                service.Login(
                    "test@gmail.com",
                    "passwordSalah"
                ));
        }
    }
}