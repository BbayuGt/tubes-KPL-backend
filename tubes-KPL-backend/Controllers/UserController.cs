using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tubes_KPL_backend.Models;
using tubes_KPL_backend.Repositories;

namespace tubes_KPL_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IGenericRepository<User> _userRepository;

    public UserController(IGenericRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<object>>> GetAllUsers()
    {
        var users = await _userRepository.GetAllAsync();
        return Ok(users.Select(u => new
        {
            u.Id,
            u.Name,
            u.Email,
            u.Role
        }));
    }

    [HttpPut("{id}/role")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
    public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateRoleDto dto)
    {
        if (id == 1) return BadRequest("Cannot modify super admin role.");
        
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return NotFound("User not found.");

        if (user.Role == "Admin") return BadRequest("Cannot modify the role of an admin.");

        if (dto.Role != "User" && dto.Role != "Penyelenggara" && dto.Role != "penyelenggara") 
            return BadRequest("Invalid role. Must be 'User' or 'Penyelenggara'.");

        user.Role = dto.Role;
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        return Ok(new { Message = "User role updated successfully." });
    }
}

public class UpdateRoleDto
{
    public string Role { get; set; } = string.Empty;
}
