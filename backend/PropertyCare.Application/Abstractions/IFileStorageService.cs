namespace PropertyCare.Application.Abstractions;

/// <summary>
/// Stores uploaded files on the server and exposes them under the web root.
/// Keeps Application handlers free of hosting/file-system details.
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Saves <paramref name="content"/> under <paramref name="subfolder"/> with a generated unique
    /// name (preserving the extension of <paramref name="originalFileName"/>) and returns the path
    /// relative to the web root, e.g. "uploads/12/{guid}.jpg".
    /// </summary>
    Task<string> SaveAsync(string subfolder, string originalFileName, Stream content, CancellationToken ct);

    /// <summary>Deletes a file by its web-root-relative path. No-op when the file does not exist.</summary>
    void Delete(string relativePath);
}
