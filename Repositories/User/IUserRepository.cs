using CarRental_BE.Models.Auth;

namespace CarRental_BE.Repositories.User
{
    public interface IUserRepository
    {
        Task<bool> Register(RegisterVM request);

        Task<Entities.User> Login(LoginVM request);

        Task<Entities.User> GetById(long id);
        Task<List<Entities.User>> GetAll();


    }
}
