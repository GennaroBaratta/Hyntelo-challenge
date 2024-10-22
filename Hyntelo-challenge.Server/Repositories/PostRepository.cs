using Hyntelo_challenge.Server.DB;
using Hyntelo_challenge.Server.DTO;
using Hyntelo_challenge.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Hyntelo_challenge.Server.Repositories
{
    public interface IPostRepository : IRepository<Post>
    {
        new Task<PaginatedResult<PostDto>> GetAllAsync(int page, int pageSize);
        new Task<PostDto?> GetByIdAsync(int id);
    }

    public class PostRepository : Repository<Post>, IPostRepository
    {
        private readonly BlogDbContext _context;

        public PostRepository(BlogDbContext context) : base(context)
        {
            _context = context;
        }

        public new async Task<PaginatedResult<PostDto>> GetAllAsync(int page, int pageSize)
        {
            var query = from post in _context.Posts
                        join user in _context.Users on post.UserId equals user.Id
                        orderby post.Id descending
                        select new PostDto
                        {
                            Id = post.Id,
                            UserId = post.UserId,
                            Title = post.Title,
                            Body = post.Body.Length > 50 ? post.Body.Substring(0, 500) + "..." : post.Body, // Body preview (first 50 characters),
                            AuthorName = user.Name,
                        };

            var totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PaginatedResult<PostDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public new async Task<PostDto?> GetByIdAsync(int id)
        {
            var query = from post in _context.Posts
                        where post.Id == id
                        join user in _context.Users on post.UserId equals user.Id
                        orderby post.Id descending
                        select new PostDto
                        {
                            Id = post.Id,
                            UserId = post.UserId,
                            Title = post.Title,
                            Body = post.Body,
                            AuthorName = user.Name,
                        };

            return await query.FirstAsync();
        }
    }
}
