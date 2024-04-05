using CarRental_BE.Common.Enums;
using CarRental_BE.Entities;
using CarRental_BE.Interfaces;
using CarRental_BE.Models.Auth;
using CarRental_BE.Models.User;
using CarRental_BE.Repositories.DBContext;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;


namespace CarRental_BE.Repositories.User
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly IUploadService _uploadService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static string key { get; set; } = "A!9HHhi%XjjYY4YP2@Nob009X";

        public UserRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Entities.User>> GetAll()
        {
            var users = await _context.Users.Include(x => x.ApprovalApplication).ToListAsync();

            return users;
        }

        public async Task<Entities.User> GetById(long id)
        {
            var user = await _context.Users
                .Where(x => x.Id == id).FirstOrDefaultAsync(); // Convert Id to string

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
            if (user.Password != null && user.Password == Encrypt(request.Password))
            {
                return user;
            }

            return null;
        }


        private static string Encrypt(string text)
        {
            using var md5 = new MD5CryptoServiceProvider();
            using var tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            using (var transform = tdes.CreateEncryptor())
            {
                byte[] textBytes = UTF8Encoding.UTF8.GetBytes(text);
                byte[] bytes = transform.TransformFinalBlock(textBytes, 0, textBytes.Length);
                return Convert.ToBase64String(bytes, 0, bytes.Length);
            }
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
                Password = Encrypt(request.Password),
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

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Access HttpContext through IHttpContextAccessor
            /*  var httpContext = _httpContextAccessor.HttpContext;
              if (httpContext != null)
              {
                  // Access session or other HttpContext properties
                  httpContext.Session.SetString("Name", user.Name);
              }*/
            return true;
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



    }
}
