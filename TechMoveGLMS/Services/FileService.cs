using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace TechMoveGLMS.Services
{
    public class FileService : IFileService
    {
        private readonly string _uploadFolder;

        public FileService(IWebHostEnvironment env)
        {
            _uploadFolder = Path.Combine(env.WebRootPath, "contracts");

            if (!Directory.Exists(_uploadFolder))
            {
                Directory.CreateDirectory(_uploadFolder);
            }
        }

        public bool IsValidPdfFile(IFormFile file)
        {
            if (file == null) return false;

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (extension != ".pdf") return false;

            if (file.ContentType != "application/pdf") return false;

            return true;
        }

        public async Task<string> SavePdfFile(IFormFile file)
        {
            if (!IsValidPdfFile(file))
            {
                throw new InvalidOperationException("Only PDF files are allowed");
            }

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(_uploadFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/contracts/{fileName}";
        }
    }
}