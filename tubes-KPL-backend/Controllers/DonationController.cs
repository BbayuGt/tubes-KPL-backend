using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using tubes_KPL_backend.DTOs;
using tubes_KPL_backend.Models;
using tubes_KPL_backend.Services;

namespace tubes_KPL_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DonationController : ControllerBase
{
    private readonly DonationService _donationService;

    public DonationController(DonationService donationService)
    {
        _donationService = donationService;
    }

    [HttpGet("{id}")]
    public async Task<IResult> GetDonationById(int id)
    {
        var donation = await _donationService.GetDonationByIdAsync(id);
        if (donation == null)
        {
            return Results.NotFound();
        }
        return Results.Ok(donation);
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin,Penyelenggara,penyelenggara")]
    public async Task<IResult> GetAllDonations()
    {
        var donations = await _donationService.GetAllDonationsAsync();
        return Results.Ok(donations);
    }

    [HttpPost]
    public async Task<IResult> CreateDonation(CreateDonationRequestDTO request)
    {
        try
        {
            // Endpoint utama transaksi donasi: mencatat donasi dan update total campaign.
            var result = await _donationService.CreateDonationAsync(request);
            return Results.Created($"/api/Donation/{result.DonationId}", result);
        }
        catch (ArgumentException ex)
        {
            // Error validasi input dari client (contoh: nominal <= 0).
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            // Error referensi data tidak valid (user/campaign tidak ditemukan).
            return Results.NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
    public async Task<IResult> DeleteDonation(int id)
    {
        try
        {
            var deleted = await _donationService.DeleteDonationAsync(id);
            if (!deleted)
            {
                return Results.NotFound(new { message = "Donation tidak ditemukan." });
            }

            return Results.NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
    }
}
