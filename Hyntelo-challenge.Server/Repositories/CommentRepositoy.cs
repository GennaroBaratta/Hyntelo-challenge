using Hyntelo_challenge.Server.DB;
using Hyntelo_challenge.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Hyntelo_challenge.Server.Repositories
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<PaginatedResult<Comment>> GetCommentsWithUserInfoAsync(int postId, int page, int pageSize);
    }

    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        private readonly BlogDbContext _context;

        public CommentRepository(BlogDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<Comment>> GetCommentsWithUserInfoAsync(int postId, int page, int pageSize)
        {
            var query = _context.Comments
                .Where(c => c.PostId == postId)
                .Select(c => new Comment
                {
                    Id = c.Id,
                    PostId = c.PostId,
                    UserId = c.UserId,
                    Body = c.Body,
                });

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Comment>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
        }
    }
}
