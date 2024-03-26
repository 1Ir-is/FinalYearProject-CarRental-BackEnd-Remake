namespace CarRental_BE.Interfaces
{
    public interface IUploadService
    {
        Task<string> SaveFile(IFormFile image);

        string GetFullPath(string fileName);

        Task DeleteFile(string fileName);
    }
}
