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
        private readonly IUploadService _uploadService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;

        private static string key { get; set; } = "A!9HHhi%XjjYY4YP2@Nob009X";

        public UserRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor,
            HttpClient httpClient)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
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



        public async Task<Entities.User> Login(LoginVM request)
        {
            var user = await _context.Users
                .Include(x => x.ApprovalApplication)
                .FirstOrDefaultAsync(x => x.Email == request.Email);

            if (user == null || !user.Status)
                return null;

            // Check if the password retrieved from the database is not null
            if (user.Password != null && BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return user;
            }

            return null;
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
                // You can set Address to null or empty string here if needed
            };

            await _context.Users.AddAsync(user);

            var res = await _context.SaveChangesAsync() > 0;

            return res;
        }


        public async Task<bool> EditInfoUser(UserEditVM request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.userId);

            if (user == null)
            {
                return false; // User not found
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
                return false; // User not found
            }

            // Verify the old password using bcrypt
            if (!BCrypt.Net.BCrypt.Verify(vm.OldPassword, user.Password))
            {
                return false; // Old password doesn't match
            }

            // Hash the new password using bcrypt
            user.Password = HashPassword(vm.NewPassword);

            _context.Users.Update(user);

            var success = await _context.SaveChangesAsync() > 0;

            return success;
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
                // Fetch the user's approval application
                var approvalApplication = await _context.ApprovalApplications.FirstOrDefaultAsync(app => app.UserId == userId);

                if (approvalApplication != null)
                {
                    return approvalApplication.RequestStatus.ToString(); // Return the request status
                }

                return REQUEST_STATUS.NotApprovedYet.ToString(); // If no application found, return "NotApprovedYet"
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                return REQUEST_STATUS.NotApprovedYet.ToString(); // Return "NotApprovedYet" in case of an error
            }
        }

        public async Task<string> GetUserAvatar(long userId)
        {
            try
            {
                // Retrieve the user by their ID
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                // If the user is found, return their avatar URL
                return user != null ? user.Avatar : null;
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return null in case of an error
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

        public async Task<string> LoginWithGoogle(string token)
        {
            try
            {
                // Verify the Google token
                var payload = await GoogleJsonWebSignature.ValidateAsync(token);

                // Extract user information from the verified token
                var email = payload.Email;
                var name = payload.Name;
                var avatar = payload.Picture;

                // Check if the user already exists in the database
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (existingUser != null)
                {
                    // User already exists, update information
                    existingUser.Name = name;
                    existingUser.Avatar = avatar;
                    // You can update other properties here if needed

                    _context.Users.Update(existingUser);
                    await _context.SaveChangesAsync();

                    return existingUser.Id.ToString(); // Return user ID or token if applicable
                }

                // Create a new user entry in the database
                var newUser = new Entities.User
                {
                    Name = name,
                    Email = email,
                    Role = ROLE_TYPE.USER,
                    Avatar = avatar ?? "/user-content/default-user.png"
                    // You can set additional properties here if needed
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return newUser.Id.ToString(); // Return user ID or token if applicable
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error logging in with Google: {ex.Message}");
                return null;
            }
        }

    }
}
