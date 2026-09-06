using PropertyCare.Application.Abstractions;

namespace PropertyCare.API.Services;

/// <summary>
/// Stores uploaded files under a private folder outside wwwroot, so no static-file middleware can
/// serve them. Reading them back goes through an authorized controller action instead.
/// </summary>
public sealed class LocalFileStorageService : IFileStorageService
{
    private const string DefaultRootFolder = "App_Data/uploads";

    private readonly string _rootPath;

    public LocalFileStorageService(IWebHostEnvironment env, IConfiguration config)
    {
        var configured = config["Storage:RootPath"];
        var relativeOrAbsolute = string.IsNullOrWhiteSpace(configured) ? DefaultRootFolder : configured;

        _rootPath = Path.GetFullPath(Path.IsPathRooted(relativeOrAbsolute)
            ? relativeOrAbsolute
            : Path.Combine(env.ContentRootPath, relativeOrAbsolute));
    }

    public async Task<string> SaveAsync(
        string subfolder, string extension, Stream content, CancellationToken ct)
    {
        var storedName = $"{Guid.NewGuid():N}{extension}";

        // Storage-root-relative path uses forward slashes so it is portable across platforms.
        var relativePath = $"{subfolder.Trim('/')}/{storedName}";

        var absolutePath = ToAbsolutePath(relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);

        await using (var target = new FileStream(absolutePath, FileMode.CreateNew))
        {
            await content.CopyToAsync(target, ct);
        }

        return relativePath;
    }

    public Stream? OpenRead(string relativePath)
    {
        var absolutePath = ToAbsolutePath(relativePath);

        return File.Exists(absolutePath)
            ? new FileStream(absolutePath, FileMode.Open, FileAccess.Read, FileShare.Read)
            : null;
    }

    public void Delete(string relativePath)
    {
        var absolutePath = ToAbsolutePath(relativePath);

        if (File.Exists(absolutePath))
            File.Delete(absolutePath);
    }

    /// <summary>Resolves a relative path inside the storage root, rejecting anything that escapes it.</summary>
    private string ToAbsolutePath(string relativePath)
    {
        var combined = Path.GetFullPath(Path.Combine(
            _rootPath, relativePath.Replace('/', Path.DirectorySeparatorChar)));

        if (!combined.StartsWith(_rootPath + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            throw new UnauthorizedAccessException("The resolved path is outside the storage root.");

        return combined;
    }
}
