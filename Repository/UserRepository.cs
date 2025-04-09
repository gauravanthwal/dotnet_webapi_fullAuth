using FullAuth.Data;
using FullAuth.Entities;
using Microsoft.EntityFrameworkCore;

namespace FullAuth.Repository
{
    public class UserRepository: IUserRepository
    {
        private readonly AuthContext _context;

        public UserRepository(AuthContext context) {
            _context = context;
        }

        public async Task<User?> GetUserByEmail(string email) {
            return await _context.users
                .Where(u => u.Email == email)
                .SingleOrDefaultAsync();
        }

        public async Task<bool> AddUser(User user)
        {
            await _context.users.AddAsync(user);
            return await _context.SaveChangesAsync() > 0 ? true: false;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _context.users.ToListAsync();
        }

        
        public async Task<bool> UpdateUser(User user)
        {
            _context.users.Attach(user);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }


    }
}
