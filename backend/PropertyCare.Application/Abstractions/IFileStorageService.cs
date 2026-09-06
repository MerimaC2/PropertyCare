namespace PropertyCare.Application.Abstractions;

/// <summary>
/// Stores uploaded files on the server, outside the web root, so they can only be read back through
/// an authorized endpoint. Keeps Application handlers free of hosting/file-system details.
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Saves <paramref name="content"/> under <paramref name="subfolder"/> with a generated unique
    /// name and the server-chosen <paramref name="extension"/>, and returns the path relative to the
    /// storage root, e.g. "uploads/12/{guid}.jpg".
    /// </summary>
    Task<string> SaveAsync(string subfolder, string extension, Stream content, CancellationToken ct);

    /// <summary>Opens a stored file for reading, or returns null when it no longer exists.</summary>
    Stream? OpenRead(string relativePath);

    /// <summary>Deletes a file by its storage-root-relative path. No-op when the file does not exist.</summary>
    void Delete(string relativePath);
}
