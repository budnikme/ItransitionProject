using Azure.Storage.Blobs;
using CourseProject.Domain.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace CourseProject.Service;

public class AzureStorageService : IAzureStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IConfiguration _configuration;

    public AzureStorageService(
        BlobServiceClient blobServiceClient,
        IConfiguration configuration)
    {
        _blobServiceClient = blobServiceClient;
        _configuration = configuration;
    }

    public async Task<string> UploadImageAsync(IFormFile image)
    {
        var blobName = GenerateBlobName(image.FileName);
        var blobClient = GetBlobClient(blobName);
        var fileStream = await GetFileStream(image);
        await blobClient.UploadAsync(fileStream);
        return blobClient.Uri.ToString();
    }

    private string GenerateBlobName(string fileName)
    {
        return Guid.NewGuid() + Path.GetExtension(fileName);
    }

    private async Task<MemoryStream> GetFileStream(IFormFile image)
    {
        var stream = new MemoryStream();
        await image.CopyToAsync(stream);
        stream.Position = 0;
        return stream;
    }

    public async Task DeleteImageAsync(string imageUrl)
    {
        var blobName = Path.GetFileName(imageUrl);
        var blobClient = GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync();
    }

    private BlobClient GetBlobClient(string blobName)
    {
        var containerName = _configuration["AzureStorage:ContainerName"];
        var container = _blobServiceClient.GetBlobContainerClient(containerName);
        return container.GetBlobClient(blobName);
    }
}