namespace PropertyCare.Application.Common;

/// <summary>
/// Recognises an image by its leading bytes. The Content-Type header and the file name both come
/// from the client and can be forged, so the stored file type is decided here instead.
/// </summary>
public static class ImageSignature
{
    /// <summary>Bytes needed to recognise every supported format (WebP needs the most).</summary>
    public const int HeaderLength = 12;

    /// <summary>Returns the recognised content type, or null when the bytes match no supported format.</summary>
    public static string? Detect(ReadOnlySpan<byte> header)
    {
        // JPEG: FF D8 FF
        if (header.Length >= 3
            && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
        {
            return "image/jpeg";
        }

        // PNG: 89 50 4E 47 0D 0A 1A 0A
        if (header.Length >= 8
            && header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47
            && header[4] == 0x0D && header[5] == 0x0A && header[6] == 0x1A && header[7] == 0x0A)
        {
            return "image/png";
        }

        // WebP: "RIFF" .... "WEBP"
        if (header.Length >= 12
            && header[0] == (byte)'R' && header[1] == (byte)'I'
            && header[2] == (byte)'F' && header[3] == (byte)'F'
            && header[8] == (byte)'W' && header[9] == (byte)'E'
            && header[10] == (byte)'B' && header[11] == (byte)'P')
        {
            return "image/webp";
        }

        return null;
    }

    /// <summary>
    /// Reads the leading bytes of <paramref name="content"/> and rewinds it so the caller can still
    /// store the whole stream.
    /// </summary>
    public static async Task<string?> DetectAsync(Stream content, CancellationToken ct)
    {
        var startPosition = content.CanSeek ? content.Position : 0L;

        var buffer = new byte[HeaderLength];
        var read = await content.ReadAtLeastAsync(buffer, HeaderLength, throwOnEndOfStream: false, ct);

        if (content.CanSeek)
            content.Position = startPosition;

        return Detect(buffer.AsSpan(0, read));
    }
}
