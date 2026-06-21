// Digunakan untuk fitur authorization/authentication
using Microsoft.AspNetCore.Authorization;

namespace tubes_KPL_backend.Controllers
{
 
    using tubes_KPL_backend.Models;
    using Microsoft.AspNetCore.Mvc;
    using tubes_KPL_backend.Services;

    [ApiController]

    // Route endpoint menjadi: api/campaign
    [Route("api/[controller]")]
    public class CampaignController : ControllerBase
    {
        // Service untuk mengelola logic Campaign
        private readonly CampaignService _campaignService;

        // Service authentication user
        private readonly AuthService _authService;

        // Constructor Dependency Injection
        public CampaignController(CampaignService campaignService)
        {
            // Inject CampaignService ke controller
            _campaignService = campaignService;
        }

       
        // GET ALL CAMPAIGN
        [HttpGet]
        public async Task<IResult> GetAll()
        {
        
            var campaigns = await _campaignService.GetAllCampaigns();
            return Results.Ok(campaigns);
        }

        // GET CAMPAIGN BY ID
        // Endpoint: GET api/campaign/1
        [HttpGet("{id}")]
        public async Task<IResult> GetById(int id)
        {

            var campaign = await _campaignService.GetCampaignById(id);

            // Jika campaign tidak ditemukan
            if (campaign == null)

                // Mengembalikan response 404
                return Results.NotFound(new { message = "Campaign tidak ditemukan" });

        
            return Results.Ok(campaign);
        }

 
        [HttpPost]

        // Hanya Admin yang bisa membuat campaign
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<IResult> Create(Campaign campaign)
        {
            // Menambahkan campaign baru melalui service
            var newCampaign = await _campaignService.CreateCampaign(campaign);

            // Jika gagal membuat campaign
            if (newCampaign == null)

                // Mengembalikan null
                return Results.BadRequest(new { message = "Gagal membuat campaign" });

            // Mengembalikan data campaign yang berhasil dibuat
            return Results.Ok(campaign);
        }

        // Endpoint: PUT api/campaign/1
        [HttpPut("{id}")]


        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<IResult> Update(int id, Campaign campaign)
        {
           
            var result = await _campaignService.UpdateCampaign(id, campaign);

            if (!result)

                
                return Results.NotFound(new { message = "Campaign tidak ditemukan" });

            // Mengembalikan response sukses
            return Results.Ok(new { message = "Campaign berhasil diupdate" });
        }

     
   
     
        [HttpDelete("{id}")]

    
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<IResult> Delete(int id)
        {
          
            var result = await _campaignService.DeleteCampaign(id);

       
            if (!result)

             
                return Results.NotFound(new { message = "Campaign tidak ditemukan" });

         
            return Results.Ok(new { message = "Campaign berhasil dihapus" });
        }
    }
}