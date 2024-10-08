﻿
using CarRental_BE.Models.Auth;
using CarRental_BE.Models.User;

namespace CarRental_BE.Repositories.User
{
    public interface IUserRepository
    {
        Task<bool> Register(RegisterVM request);

        Task<Entities.User> Login(LoginVM request);
        Task<Entities.User> LoginWithGoogleEmail(string googleEmail);
        Task<bool> EditInfoUser(UserEditVM request);
        Task<Entities.User> GetUserByEmail(string email);
        Task<Entities.User> GetById(long id);
        Task<UserDTO> GetUserById(long userId);
        Task<List<Entities.User>> GetAll();
        Task<bool> ChangePasswordUser(ChangePasswordVM vm);
        Task CreateApprovalApplication(ApprovalApplicationVM vm, long userId);
        Task<string> GetRequestStatus(long userId);
        Task<string> GetUserAvatar(long userId);
        Task<Entities.User> LoginWithGoogle(string token);
        Task<(string ResetKey, DateTime? ResetKeyTimestamp)> GetResetKeyInfo(string email, string resetKey);

        Task StoreResetKey(string email, string resetKey, DateTime timestamp);
        Task<bool> VerifyResetKey(string email, string resetKey);
        Task ResetPassword(string email, string newPassword);
        Task<bool> Toggle(long id);
    }
}
