using PropertyCare.Application.Abstractions;

namespace PropertyCare.Tests.Common;

/// <summary>In-memory stand-in for file storage; records the last call and returns a deterministic path.</summary>
public sealed class FakeFileStorageService : IFileStorageService
{
    public string? LastSubfolder { get; private set; }
    public string? LastFileName { get; private set; }

    public Task<string> SaveAsync(string subfolder, string originalFileName, Stream content, CancellationToken ct)
    {
        LastSubfolder = subfolder;
        LastFileName = originalFileName;
        return Task.FromResult($"{subfolder}/stored.jpg");
    }

    public void Delete(string relativePath) { }
}
