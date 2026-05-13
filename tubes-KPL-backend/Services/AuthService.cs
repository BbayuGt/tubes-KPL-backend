using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.IdentityModel.Tokens;
using tubes_KPL_backend.Data;
using tubes_KPL_backend.DTOs;
using tubes_KPL_backend.Models;
using tubes_KPL_backend.Repositories;

namespace tubes_KPL_backend.Services;

public class AuthService
{
    private readonly IGenericRepository<User> _repository;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(IGenericRepository<User> repository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _repository = repository;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<User> RegisterUser(string name, string email, string password)
    {
        if (await _repository.ExistsAsync(u => u.Email == email))
            throw new BadHttpRequestException("Email already exists");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User
        {
            Name = name,
            Email = email,
            PasswordHash = passwordHash
        };

        await _repository.AddAsync(user);
        await _repository.SaveChangesAsync();

        return user;
    }

    public async Task<ActionResult<string>> Login(string email, string password)
    {
        var user = await _repository.GetByExpression(u => u.Email == email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            throw new BadHttpRequestException("Email/Password salah!");

        var token = GenerateJwtToken(user);

        return token;
    }

    public async Task<User> GetCurrentUser() // Note: Returning User? (nullable) is safer
    {
        // Access the current HTTP Context
        var httpContext = _httpContextAccessor.HttpContext;

        // Check if there is an authenticated user in the current request
        if (httpContext == null || httpContext.User.Identity?.IsAuthenticated != true)
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return null;
        }

        if (!int.TryParse(userIdClaim, out int userId)) 
        {
            return null;
        }

        var user = await _repository.GetByExpression(u => u.Id == userId);
        
        return user;
    }
    
    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings["Key"]!));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(1),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        
        return tokenHandler.WriteToken(token);
    }
}