using PropertyCare.Application.Abstractions;

namespace PropertyCare.Tests.Common;

/// <summary>In-memory stand-in for file storage; records what was written, read and deleted.</summary>
public sealed class FakeFileStorageService : IFileStorageService
{
    private readonly Dictionary<string, byte[]> _files = [];

    public string? LastSubfolder { get; private set; }
    public string? LastExtension { get; private set; }
    public List<string> SavedPaths { get; } = [];
    public List<string> DeletedPaths { get; } = [];

    public async Task<string> SaveAsync(
        string subfolder, string extension, Stream content, CancellationToken ct)
    {
        LastSubfolder = subfolder;
        LastExtension = extension;

        using var buffer = new MemoryStream();
        await content.CopyToAsync(buffer, ct);

        var relativePath = $"{subfolder}/stored{extension}";
        _files[relativePath] = buffer.ToArray();
        SavedPaths.Add(relativePath);

        return relativePath;
    }

    public Stream? OpenRead(string relativePath)
        => _files.TryGetValue(relativePath, out var bytes) ? new MemoryStream(bytes) : null;

    public void Delete(string relativePath)
    {
        _files.Remove(relativePath);
        DeletedPaths.Add(relativePath);
    }
}
