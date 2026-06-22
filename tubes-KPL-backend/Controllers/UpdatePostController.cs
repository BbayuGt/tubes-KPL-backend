using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tubes_KPL_backend.Models;
using tubes_KPL_backend.Services;

namespace tubes_KPL_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UpdatePostController : ControllerBase
    {
        private readonly UpdatePostService _updatePostService;

        public UpdatePostController(UpdatePostService updatePostService)
        {
            _updatePostService = updatePostService;
        }

        [HttpGet]
        public async Task<IResult> GetAll()
        {
            var posts = await _updatePostService.GetAllUpdatePosts();
            return Results.Ok(posts);
        }

        [HttpGet("{id}")]
        public async Task<IResult> GetById(int id)
        {
            var post = await _updatePostService.GetUpdatePostById(id);
            if (post == null)
                return Results.NotFound(new { message = "UpdatePost tidak ditemukan" });
            
            return Results.Ok(post);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin,Penyelenggara")]
        public async Task<IResult> Create(UpdatePost updatePost)
        {
            var newPost = await _updatePostService.CreateUpdatePost(updatePost);
            if (newPost == null)
                return Results.BadRequest(new { message = "Gagal membuat UpdatePost" });

            return Results.Ok(updatePost);
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin,Penyelenggara")]
        public async Task<IResult> Update(int id, UpdatePost updatePost)
        {
            var result = await _updatePostService.UpdateUpdatePost(id, updatePost);
            if (!result)
                return Results.NotFound(new { message = "UpdatePost tidak ditemukan" });

            return Results.Ok(new { message = "UpdatePost berhasil diupdate" });
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin,Penyelenggara")]
        public async Task<IResult> Delete(int id)
        {
            var result = await _updatePostService.DeleteUpdatePost(id);
            if (!result)
                return Results.NotFound(new { message = "UpdatePost tidak ditemukan" });

            return Results.Ok(new { message = "UpdatePost berhasil dihapus" });
        }
    }
}
