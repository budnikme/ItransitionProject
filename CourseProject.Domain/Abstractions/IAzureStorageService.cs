using Microsoft.AspNetCore.Http;

namespace CourseProject.Domain.Abstractions;

public interface IAzureStorageService
{
    Task<string> UploadImageAsync(IFormFile image);
    Task DeleteImageAsync(string imageUrl);
}