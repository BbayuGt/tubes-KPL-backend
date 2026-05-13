using Microsoft.AspNetCore.Authorization;

namespace tubes_KPL_backend.Controllers
{
    using tubes_KPL_backend.Models;
    using Microsoft.AspNetCore.Mvc;
    using tubes_KPL_backend.Services;


    [ApiController]
    [Route("api/[controller]")]
    public class CampaignController : ControllerBase
    {
        private readonly CampaignService _campaignService;
        private readonly AuthService _authService;

        public CampaignController(CampaignService campaignService)
        {
            _campaignService = campaignService;
        }

        // GET: api/campaign
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var campaigns = await _campaignService.GetAllCampaigns();
            return Ok(campaigns);
        }

        // GET: api/campaign/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var campaign = await _campaignService.GetCampaignById(id);

            if (campaign == null)
                return NotFound(new { message = "Campaign tidak ditemukan" });

            return Ok(campaign);
        }

        // POST: api/campaign
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<Campaign>> Create(Campaign campaign)
        {
            User user = await _authService.GetCurrentUser();
            if (user.Role != "User")
            {
                return Forbid();
            }

            var newCampaign = await _campaignService.CreateCampaign(campaign);

            if (newCampaign == null)
                return null;

            return campaign;
                
        }

        // PUT: api/campaign/1
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> Update(int id, Campaign campaign)
        {
            User user = await _authService.GetCurrentUser();
            if (user.Role != "Admin")
            {
                return Forbid();
            }
            var result = await _campaignService.UpdateCampaign(id, campaign);

            if (!result)
                return NotFound(new { message = "Campaign tidak ditemukan" });

            return Ok(new { message = "Campaign berhasil diupdate" });
        }

        // DELETE: api/campaign/1
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> Delete(int id)
        {
            User user = await _authService.GetCurrentUser();
            if (user.Role != "Admin")
            {
                return Forbid();
            }
            var result = await _campaignService.DeleteCampaign(id);

            if (!result)
                return NotFound(new { message = "Campaign tidak ditemukan" });

            return Ok(new { message = "Campaign berhasil dihapus" });
        }
    }
}

