namespace TechMoveGLMS.Services
{
    public interface IFileService
    {
        Task<string> SavePdfFile(IFormFile file);
        bool IsValidPdfFile(IFormFile file);
    }
}