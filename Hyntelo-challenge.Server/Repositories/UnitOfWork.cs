using Hyntelo_challenge.Server.DB;
using Hyntelo_challenge.Server.Models;

namespace Hyntelo_challenge.Server.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Post> PostRepository { get; }
        IRepository<Comment> CommentRepository { get; }
        Task SaveAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly BlogDbContext _context;
        private IRepository<Post>? _postRepository;
        private IRepository<Comment>? _commentRepository;

        public UnitOfWork(BlogDbContext context)
        {
            _context = context;
        }

        public IRepository<Post> PostRepository
        {
            get
            {
                _postRepository ??= new Repository<Post>(_context);
                return _postRepository;
            }
        }

        public IRepository<Comment> CommentRepository
        {
            get
            {
                _commentRepository ??= new Repository<Comment>(_context);
                return _commentRepository;
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
