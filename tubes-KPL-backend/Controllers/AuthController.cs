using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tubes_KPL_backend.Data;
using tubes_KPL_backend.DTOs;
using tubes_KPL_backend.Models;
using tubes_KPL_backend.Services;

namespace tubes_KPL_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly AuthService _authService;
    

    public AuthController(AppDbContext context, AuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    [HttpGet("me")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponseDTO))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMe()
    {
        try
        {
            User user = await _authService.GetCurrentUser();
            
            // Supaya tidak return password, pake DTO
            UserResponseDTO response = new UserResponseDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            };
            
            return Ok(new
            {
                User = response
            });
        } catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDTO request)
    {
        try
        {
            await _authService.RegisterUser(request.Name, request.Email, request.Password);
            return Ok(new
            {
                Message = "User has been registered successfully!"
            });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO request)
    {
        try
        {
            string jwt = await _authService.Login(request.Email, request.Password);
            
            return Ok(new
            {
                Message = "Successfully logged in!",
                Token = jwt
            });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}