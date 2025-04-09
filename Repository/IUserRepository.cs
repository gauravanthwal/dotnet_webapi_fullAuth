using FullAuth.Entities;

namespace FullAuth.Repository
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmail(string email);

        Task<bool> AddUser(User user);

        Task<bool> UpdateUser(User user);

        Task<List<User>> GetAllUsers();

    }
}
