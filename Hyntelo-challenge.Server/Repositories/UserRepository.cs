using Hyntelo_challenge.Server.DB;
using Hyntelo_challenge.Server.DTO;
using Hyntelo_challenge.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hyntelo_challenge.Server.Repositories
{

    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByUserName(string username);
    }

    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly BlogDbContext _context;

        public UserRepository(BlogDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<User?> GetByUserName(string username)
        {
            var query = from user in _context.Users
                        where user.Username == username
                        select user;

            return await query.FirstAsync();
        }
    }
}
