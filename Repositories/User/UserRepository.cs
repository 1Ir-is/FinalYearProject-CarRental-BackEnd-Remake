using CarRental_BE.Common.Enums;
using CarRental_BE.Entities;
using CarRental_BE.Interfaces;
using CarRental_BE.Models.Auth;
using CarRental_BE.Models.User;
using CarRental_BE.Repositories.DBContext;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using System.Net.Http;
using Microsoft.Data.SqlClient.Server;
using Google.Apis.Auth;


namespace CarRental_BE.Repositories.User
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly IMailService _mailService;

        public UserRepository(AppDbContext context, IMailService mailService)
        {
            _context = context;
            _mailService = mailService;
        }

        public async Task<List<Entities.User>> GetAll()
        {
            var users = await _context.Users.Include(x => x.ApprovalApplication).ToListAsync();
            return users;
        }


        public async Task<Entities.User> GetById(long id)
        {
            var user = await _context.Users
                .Where(x => x.Id == id)
                .Include(x => x.PostVehicles)
                .Include(x => x.FollowVehicles)
                    .ThenInclude(x => x.PostVehicle)
                .Include(x => x.UserRentVehicles)
                    .ThenInclude(x => x.PostVehicle)
                .Include(x => x.UserReviewVehicles)
                    .ThenInclude(x => x.PostVehicle)
                .Include(x => x.ApprovalApplication)
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<UserDTO> GetUserById(long userId)
        {
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.PostVehicles)
                .Include(u => u.FollowVehicles)
                    .ThenInclude(fv => fv.PostVehicle)
                .Include(u => u.UserRentVehicles)
                    .ThenInclude(urv => urv.PostVehicle)
                .Include(u => u.UserReviewVehicles)
                    .ThenInclude(urv => urv.PostVehicle)
                .Include(u => u.ApprovalApplication)
                .Select(u => new UserDTO
                {
                    Name = u.Name,
                    Avatar = u.Avatar,
                    Phone = u.Phone,
                    Email = u.Email, 
                    Address = u.Address,
                    TrustPoint = u.TrustPoint 
                                              
                })
                .FirstOrDefaultAsync();

            return user;
        }


        public async Task<Entities.User> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Entities.User> Login(LoginVM request)
        {
            var user = await _context.Users
                .Include(x => x.ApprovalApplication)
                .FirstOrDefaultAsync(x => x.Email == request.Email);

            // Check if the user object is null
            if (user == null || !user.Status)
                return null;

            // Ensure that user.Password is not null before calling BCrypt.Verify
            if (!string.IsNullOrEmpty(user.Password) && BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return user;
            }
            return null;
        }



        public async Task<Entities.User> LoginWithGoogleEmail(string googleEmail)
        {
            var user = await _context.Users
                .Include(x => x.ApprovalApplication)
                .FirstOrDefaultAsync(x => x.Email == googleEmail);
            if (user == null || !user.Status)
            {
                return null;
            }
            return user;
        }

        private static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public async Task<bool> Register(RegisterVM request)
        {
            var u = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
            if (u != null)
                return false;

            var user = new Entities.User()
            {
                Email = request.Email,
                Name = request.Name,
                Password = HashPassword(request.Password),
                Role = ROLE_TYPE.USER,
                Avatar = "/user-content/default-user.png",
                ResetKey = "", // Provide a default value for ResetKey
                ResetKeyTimestamp = null // Set ResetKeyTimestamp to null or provide a default value if needed
            };

            await _context.Users.AddAsync(user);
            var res = await _context.SaveChangesAsync() > 0;
            return res;
        }

        public async Task<bool> Toggle(long id)
        {
            var u = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            u.Status = !u.Status;
            _context.Users.Update(u);

            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<bool> EditInfoUser(UserEditVM request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.userId);
            if (user == null)
            {
                return false;
            }
            user.Name = request.Name;
            user.Address = request.Address;
            user.Phone = request.Phone;
            user.Avatar = request.Avatar;
            _context.Users.Update(user);
            await _context.SaveChangesAsync(); 
            return true;
        }

        public async Task<bool> ChangePasswordUser(ChangePasswordVM vm)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == vm.Id);
            if (user == null)
            {
                return false; 
            }
            if (!BCrypt.Net.BCrypt.Verify(vm.OldPassword, user.Password))
            {
                return false; 
            }
            user.Password = HashPassword(vm.NewPassword);
            _context.Users.Update(user);
            var success = await _context.SaveChangesAsync() > 0;
            return success;
        }


        public async Task StoreResetKey(string email, string resetKey, DateTime timestamp)
        {
            // Store the reset key in the database along with the user's email and a timestamp
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                user.ResetKey = resetKey;
                user.ResetKeyTimestamp = timestamp;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<(string ResetKey, DateTime? ResetKeyTimestamp)> GetResetKeyInfo(string email, string resetKey)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.ResetKey == resetKey);
            if (user != null)
            {
                return (user.ResetKey, user.ResetKeyTimestamp);
            }
            else
            {
                // User not found or reset key does not match
                return (null, null);
            }
        }


        public async Task<bool> VerifyResetKey(string email, string resetKey)
        {
            // Verify the reset key from the database and check if it's still valid
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.ResetKey == resetKey);
            if (user != null && user.ResetKeyTimestamp.HasValue && DateTime.UtcNow.Subtract(user.ResetKeyTimestamp.Value).TotalHours < 1)
            {
                // Reset key is valid and within 1 hour of generation
                return true;
            }

            return false;
        }

        public async Task ResetPassword(string email, string newPassword)
        {
            try
            {
                // Retrieve the user from the database
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user != null)
                {
                    // Reset the user's password
                    user.Password = HashPassword(newPassword);

                    // Save changes to the database
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error resetting password for {email}: {ex.Message}");
                throw; // Rethrow the exception to handle it at a higher level
            }
        }

        public async Task CreateApprovalApplication(ApprovalApplicationVM vm, long userId)
        {
            var app = new ApprovalApplication
            {
                UserId = userId,
                Address = vm.Address,
                Description = vm.Description,
                Email = vm.Email,
                Identity = vm.Identity,
                Name = vm.Name,
                Title = vm.Title,
                Phone = vm.Phone,
                Type = vm.Type,
                RequestStatus = REQUEST_STATUS.Pending
            };

            await _context.ApprovalApplications.AddAsync(app);
            await _context.SaveChangesAsync();
        }

        public async Task<string> GetRequestStatus(long userId)
        {
            try
            {
                var approvalApplication = await _context.ApprovalApplications.FirstOrDefaultAsync(app => app.UserId == userId);

                if (approvalApplication != null)
                {
                    return approvalApplication.RequestStatus.ToString(); 
                }

                return REQUEST_STATUS.NotApprovedYet.ToString(); 
            }
            catch (Exception ex)
            {
                return REQUEST_STATUS.NotApprovedYet.ToString(); 
            }
        }

        public async Task<string> GetUserAvatar(long userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                return user != null ? user.Avatar : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving user avatar: {ex.Message}");
                return null;
            }
        }


        /* public async Task<bool> RegisterWithGoogle(string name, string email)
         {
             try
             {
                 // Check if the user already exists in the database
                 var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                 if (existingUser != null)
                 {
                     // User already exists, update information
                     existingUser.Name = name;
                     // You can update other properties here if needed

                     _context.Users.Update(existingUser);
                     var result = await _context.SaveChangesAsync();

                     // Check if the user information was successfully updated
                     return result > 0;
                 }

                 // Create a new user entry in the database
                 var newUser = new Entities.User
                 {
                     Name = name,
                     Email = email,
                     Role = ROLE_TYPE.USER,
                     Avatar = "/user-content/default-user.png"
                     // You can set additional properties here if needed
                 };

                 _context.Users.Add(newUser);
                 var result = await _context.SaveChangesAsync();

                 // Check if the user was successfully added to the database
                 return result > 0;
             }
             catch (Exception ex)
             {
                 // Log the exception
                 Console.WriteLine($"Error registering user with Google: {ex.Message}");
                 return false;
             }
         }

 */

        public async Task<Entities.User> LoginWithGoogle(string token)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(token);
                var email = payload.Email;
                var name = payload.Name;
                var avatar = payload.Picture;

                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (existingUser != null)
                {
                    existingUser.Name = name;
                    existingUser.Avatar = avatar;

                    // Check if ResetKey is null before updating
                    if (existingUser.ResetKey == null)
                    {
                        existingUser.ResetKey = ""; // Set an empty string or any default value
                    }

                    _context.Users.Update(existingUser);
                    await _context.SaveChangesAsync();

                    return existingUser;
                }

                // Create a new user entry in the database
                var newUser = new Entities.User
                {
                    Name = name,
                    Email = email,
                    Role = ROLE_TYPE.USER,
                    Avatar = avatar ?? "/user-content/default-user.png",
                    ResetKey = "" // Set an empty string or any default value
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return newUser;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging in with Google: {ex.Message}");
                return null;
            }
        }
    }
}
