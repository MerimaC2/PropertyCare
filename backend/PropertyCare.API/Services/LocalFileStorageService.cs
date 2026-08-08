using PropertyCare.Application.Abstractions;

namespace PropertyCare.API.Services;

/// <summary>Stores uploaded files under the application's wwwroot folder so they are served as static files.</summary>
public sealed class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _env;

    public LocalFileStorageService(IWebHostEnvironment env) => _env = env;

    private string WebRoot => _env.WebRootPath
        ?? Path.Combine(_env.ContentRootPath, "wwwroot");

    public async Task<string> SaveAsync(
        string subfolder, string originalFileName, Stream content, CancellationToken ct)
    {
        var extension = Path.GetExtension(originalFileName);
        var storedName = $"{Guid.NewGuid():N}{extension}";

        // Web-root-relative path uses forward slashes so it can be used directly as a URL.
        var relativePath = $"{subfolder.Trim('/')}/{storedName}";

        var absoluteFolder = Path.Combine(WebRoot, subfolder.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(absoluteFolder);

        var absolutePath = Path.Combine(absoluteFolder, storedName);
        await using (var target = new FileStream(absolutePath, FileMode.CreateNew))
        {
            await content.CopyToAsync(target, ct);
        }

        return relativePath;
    }

    public void Delete(string relativePath)
    {
        var absolutePath = Path.Combine(
            WebRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));

        if (File.Exists(absolutePath))
            File.Delete(absolutePath);
    }
}
