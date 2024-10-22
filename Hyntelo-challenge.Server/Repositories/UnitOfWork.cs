using Hyntelo_challenge.Server.DB;
using Hyntelo_challenge.Server.Models;

namespace Hyntelo_challenge.Server.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IPostRepository PostRepository { get; }
        ICommentRepository CommentRepository { get; }
        IUserRepository UserRepository { get; }
        Task SaveAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly BlogDbContext _context;
        private IPostRepository? _postRepository;
        private ICommentRepository? _commentRepository;
        private IUserRepository? _userRepository;

        public UnitOfWork(BlogDbContext context)
        {
            _context = context;
        }

        public IPostRepository PostRepository
        {
            get
            {
                _postRepository ??= new PostRepository(_context);
                return _postRepository;
            }
        }

        public ICommentRepository CommentRepository
        {
            get
            {
                _commentRepository ??= new CommentRepository(_context);
                return _commentRepository;
            }
        }

        public IUserRepository UserRepository {
            get
            {
                _userRepository ??= new UserRepository(_context);
                return _userRepository;
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
