using PropertyCare.Application.Common;

namespace PropertyCare.Tests.Modules;

public class ImageSignatureTests
{
    [Fact]
    public void Detect_JpegHeader_ReturnsJpeg()
        => Assert.Equal("image/jpeg", ImageSignature.Detect([0xFF, 0xD8, 0xFF, 0xE0]));

    [Fact]
    public void Detect_PngHeader_ReturnsPng()
        => Assert.Equal(
            "image/png",
            ImageSignature.Detect([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]));

    [Fact]
    public void Detect_WebPHeader_ReturnsWebP()
        => Assert.Equal(
            "image/webp",
            ImageSignature.Detect("RIFF\0\0\0\0WEBP"u8));

    [Fact]
    public void Detect_HtmlPayload_ReturnsNull()
        => Assert.Null(ImageSignature.Detect("<html><body>"u8));

    [Fact]
    public async Task DetectAsync_RewindsStreamSoContentCanStillBeStored()
    {
        await using var stream = new MemoryStream([0xFF, 0xD8, 0xFF, 0xE0, 1, 2, 3]);

        var detected = await ImageSignature.DetectAsync(stream, CancellationToken.None);

        Assert.Equal("image/jpeg", detected);
        Assert.Equal(0, stream.Position);
    }
}
