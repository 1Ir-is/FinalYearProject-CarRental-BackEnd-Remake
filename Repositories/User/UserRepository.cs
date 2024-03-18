using CarRental_BE.Common.Enums;
using CarRental_BE.Models.Auth;
using CarRental_BE.Repositories.DBContext;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;


namespace CarRental_BE.Repositories.User
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        private static string key { get; set; } = "A!9HHhi%XjjYY4YP2@Nob009X";

        public UserRepository(AppDbContext context)
        {
            _context = context;
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
            var u = await _context.Users.Include(x => x.ApprovalApplication).FirstOrDefaultAsync(x => x.Email == request.Email);
            if (u == null)
                return null;
            if (!u.Status)
                return null;
            if (u.Password == Encrypt(request.Password))
            {
                return u;
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

    }
}

