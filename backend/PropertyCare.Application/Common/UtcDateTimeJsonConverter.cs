using System.Text.Json;
using System.Text.Json.Serialization;

namespace PropertyCare.Application.Common;

/// <summary>
/// Writes every <see cref="DateTime"/> as UTC with the Z suffix.
///
/// Everything the application stores is UTC, but the kind does not survive the round trip through
/// SQL Server: values come back as <see cref="DateTimeKind.Unspecified"/> and were serialized
/// without any timezone at all. The frontend then had to guess, and it guessed differently on
/// different screens - two templates appended "Z" by hand while the others did not, so the same
/// timestamp was shown in two different zones.
///
/// One rule here means nothing has to be added on the client.
/// </summary>
public sealed class UtcDateTimeJsonConverter : JsonConverter<DateTime>
{
    public override DateTime Read(
        ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => AsUtc(reader.GetDateTime());

    public override void Write(
        Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => writer.WriteStringValue(AsUtc(value));

    /// <summary>
    /// A value that never carried a kind is a UTC value that lost its label on the way out of the
    /// database, so it is labelled rather than shifted. A local one is genuinely converted.
    /// </summary>
    private static DateTime AsUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
    };
}
