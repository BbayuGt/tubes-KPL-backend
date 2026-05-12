using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tubes_KPL_backend.DTOs;
using tubes_KPL_backend.Models;
using tubes_KPL_backend.Services;

namespace tubes_KPL_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DonationController : ControllerBase
{
    private readonly DonationService _donationService;
    private readonly AuthService _authService;
    public DonationController(DonationService donationService, AuthService authService)
    {
        _donationService = donationService;
        _authService = authService;
    }

    [HttpGet("{id}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<Donation>> GetDonationById(int id)
    {
        User user = await _authService.GetCurrentUser();   
        var donation = await _donationService.GetDonationByIdAsync(id);
        if (donation == null) { 
            return NotFound();
        }
        if(user.Id != donation.UserId)
        {
            return Forbid();
        }
        if (donation == null)
        {
            return NotFound();
        }
        return Ok(donation);
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<List<Donation>>> GetAllDonations()
    {
        User user = await _authService.GetCurrentUser();
        if (user.Role != "Admin")
        {
            return Forbid();
        }
        var donations = await _donationService.GetAllDonationsAsync();
        return Ok(donations);
    }

    [HttpPost]
    public async Task<ActionResult<CreateDonationResponseDTO>> CreateDonation(CreateDonationRequestDTO request)
    {
        try
        {
            // Endpoint utama transaksi donasi: mencatat donasi dan update total campaign.
            var result = await _donationService.CreateDonationAsync(request);
            return CreatedAtAction(nameof(GetDonationById), new { id = result.DonationId }, result);
        }
        catch (ArgumentException ex)
        {
            // Error validasi input dari client (contoh: nominal <= 0).
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            // Error referensi data tidak valid (user/campaign tidak ditemukan).
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> DeleteDonation(int id)
    {
        User user = await _authService.GetCurrentUser();
        if(user.Role != "Admin")
        {
            return Forbid();
        }
        try
        {
            var deleted = await _donationService.DeleteDonationAsync(id);
            if (!deleted)
            {
                return NotFound(new { message = "Donation tidak ditemukan." });
            }

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
