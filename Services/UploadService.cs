using CarRental_BE.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Net.Http.Headers;
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

        public string GetFullPath(string filename)
        {
            var path = Path.Combine(USER_CONTENT_FOLDER, filename);
            return path;
        }

        public async Task DeleteFile(string filename)
        {
            string path = Path.Combine(_userContent, filename);
            if (File.Exists(path))
            {
                await Task.Run(() => File.Delete(path));
            }
        }

        private async Task<string> ConfirmSave(Stream stream, string fileName)
        {
            string filePath = Path.Combine(_userContent, fileName);
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fs);
            }
            return Path.Combine(USER_CONTENT_FOLDER, fileName);
        }

        public async Task<string> SaveFile(IFormFile file)
        {
            string originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";

            return await ConfirmSave(file.OpenReadStream(), fileName);
        }
    }
}
