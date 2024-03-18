using CarRental_BE.Entities;

namespace CarRental_BE.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
