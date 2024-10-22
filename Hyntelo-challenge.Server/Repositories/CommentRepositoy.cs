using Hyntelo_challenge.Server.DB;
using Hyntelo_challenge.Server.DTO;
using Hyntelo_challenge.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Hyntelo_challenge.Server.Repositories
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<PaginatedResult<CommentDto>> GetAllAsync(int postId, int page, int pageSize);
    }

    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        private readonly BlogDbContext _context;

        public CommentRepository(BlogDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<CommentDto>> GetAllAsync(int postId, int page, int pageSize)
        {
            var query = from comment in _context.Comments
                        where comment.PostId == postId
                        join user in _context.Users on comment.UserId equals user.Id
                        //orderby comment.Id descending
                        select new CommentDto
                        {
                            Id = comment.Id,
                            PostId = comment.PostId,
                            UserId = comment.UserId,
                            Body = comment.Body,
                            AuthorName = user.Name,
                        };

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<CommentDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
        }
    }
}
