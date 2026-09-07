using System.Text.Json;
using PropertyCare.Application.Common;

namespace PropertyCare.Tests.Modules;

/// <summary>
/// The point of the converter: one rule for every timestamp, so no screen has to append a Z.
/// </summary>
public class UtcDateTimeJsonConverterTests
{
    private static readonly JsonSerializerOptions Options =
        new() { Converters = { new UtcDateTimeJsonConverter() } };

    /// <summary>
    /// This is the case that mattered: SQL Server hands values back without a kind, and before
    /// the converter they were written with no timezone at all.
    /// </summary>
    [Fact]
    public void Write_UnspecifiedKind_IsLabelledAsUtcWithoutShiftingTheClock()
    {
        var value = new DateTime(2026, 9, 6, 14, 30, 0, DateTimeKind.Unspecified);

        var json = JsonSerializer.Serialize(value, Options);

        Assert.Equal("\"2026-09-06T14:30:00Z\"", json);
    }

    [Fact]
    public void Write_UtcKind_KeepsTheZSuffix()
    {
        var value = new DateTime(2026, 9, 6, 14, 30, 0, DateTimeKind.Utc);

        Assert.Equal("\"2026-09-06T14:30:00Z\"", JsonSerializer.Serialize(value, Options));
    }

    [Fact]
    public void Write_LocalKind_IsConvertedRatherThanRelabelled()
    {
        var local = new DateTime(2026, 9, 6, 14, 30, 0, DateTimeKind.Local);

        var json = JsonSerializer.Serialize(local, Options);

        Assert.EndsWith("Z\"", json);
        Assert.Equal(
            $"\"{local.ToUniversalTime():yyyy-MM-ddTHH:mm:ss}Z\"",
            json);
    }

    [Fact]
    public void Write_NullableProperty_UsesTheSameRule()
    {
        var payload = new Payload { CreatedAtUtc = new DateTime(2026, 9, 6, 8, 0, 0), ClosedAtUtc = null };

        var json = JsonSerializer.Serialize(payload, Options);

        Assert.Contains("\"CreatedAtUtc\":\"2026-09-06T08:00:00Z\"", json);
        Assert.Contains("\"ClosedAtUtc\":null", json);
    }

    [Fact]
    public void Read_ValueWithoutAZone_ComesBackAsUtc()
    {
        var value = JsonSerializer.Deserialize<DateTime>("\"2026-09-06T14:30:00\"", Options);

        Assert.Equal(DateTimeKind.Utc, value.Kind);
        Assert.Equal(new DateTime(2026, 9, 6, 14, 30, 0, DateTimeKind.Utc), value);
    }

    private sealed class Payload
    {
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? ClosedAtUtc { get; set; }
    }
}
