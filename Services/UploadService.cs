using CarRental_BE.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CarRental_BE.Services
{
    public class UploadService : IUploadService
    {
        private readonly string _userContent;
        private const string USER_CONTENT_FOLDER = "user-content";

        public UploadService(IWebHostEnvironment webHostEnvironment)
        {
            _userContent = Path.Combine(webHostEnvironment.ContentRootPath, USER_CONTENT_FOLDER);
            if (!Directory.Exists(_userContent))
            {
                Directory.CreateDirectory(_userContent);
            }
        }

        public async Task<string> SaveFile(IFormFile file)
        {
            string originalFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(_userContent, originalFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return originalFileName;
        }

        public async Task DeleteFile(string fileName)
        {
            string filePath = Path.Combine(_userContent, fileName);
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
            }
        }

        public string GetFullPath(string filename)
        {
            return Path.Combine(_userContent, filename);
        }
    }
}
