using tubes_KPL_backend.Models;
using tubes_KPL_backend.Repositories;

namespace tubes_KPL_backend.Services
{
    public class UpdatePostService
    {
        private readonly IGenericRepository<UpdatePost> _repository;

        public UpdatePostService(IGenericRepository<UpdatePost> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<UpdatePost>> GetAllUpdatePosts()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<UpdatePost?> GetUpdatePostById(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<UpdatePost> CreateUpdatePost(UpdatePost updatePost)
        {
            var existing = await _repository.GetByIdAsync(updatePost.Id);
            if (existing != null)
            {
                return null;
            }
            updatePost.CreatedAt = DateTime.UtcNow;
            await _repository.AddAsync(updatePost);
            await _repository.SaveChangesAsync();
            return updatePost;
        }

        public async Task<bool> UpdateUpdatePost(int id, UpdatePost updatedPost)
        {
            var post = await _repository.GetByIdAsync(id);

            if (post == null)
                return false;

            post.Title = updatedPost.Title;
            post.Description = updatedPost.Description;
            post.CampaignId = updatedPost.CampaignId;

            _repository.Update(post);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUpdatePost(int id)
        {
            var post = await _repository.GetByIdAsync(id);

            if (post == null)
                return false;

            _repository.Delete(post);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
