using CarRental_BE.Models.Auth;
using CarRental_BE.Models.User;

namespace CarRental_BE.Repositories.User
{
    public interface IUserRepository
    {
        Task<bool> Register(RegisterVM request);

        Task<Entities.User> Login(LoginVM request);
        Task<bool> EditInfoUser(UserEditVM request);

        Task<Entities.User> GetById(long id);
        Task<List<Entities.User>> GetAll();

        Task CreateApprovalApplication(ApprovalApplicationVM vm, long userId);
        Task<bool> IsApproving(long userId);
    }
}
