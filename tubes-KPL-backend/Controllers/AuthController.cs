using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tubes_KPL_backend.DTOs;
using tubes_KPL_backend.Models;
using tubes_KPL_backend.Services;
using System.ComponentModel.DataAnnotations;

namespace tubes_KPL_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpGet("me")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponseDTO))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IResult> GetMe()
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
            
            return Results.Ok(new
            {
                User = response
            });
        } catch (Exception e)
        {
            return Results.BadRequest(e.Message);
        }
    }

    [HttpPost("register")]
    public async Task<IResult> Register(RegisterDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || !new EmailAddressAttribute().IsValid(request.Email))
        {
            return Results.BadRequest("Invalid email format.");
        }

        try
        {
            await _authService.RegisterUser(request.Name, request.Email, request.Password);
            return Results.Ok(new
            {
                Message = "User has been registered successfully!"
            });
        }
        catch (Exception e)
        {
            return Results.BadRequest(e.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IResult> Login(LoginDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || !new EmailAddressAttribute().IsValid(request.Email))
        {
            return Results.BadRequest("Invalid email format.");
        }

        try
        {
            ActionResult<string> jwt = await _authService.Login(request.Email, request.Password);
            
            return Results.Ok(new
            {
                Message = "Successfully logged in!",
                Token = jwt.Value
            });
        }
        catch (Exception e)
        {
            return Results.BadRequest(e.Message);
        }
    }
}