using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace trading_platform.KoreaInvestment;

public class StringToBooleanConverter : JsonConverter<bool> {
  public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
    return reader.GetString() != "N";
  }
  public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options) {
    writer.WriteStringValue(value ? "Y" : "N");
  }
}

public class DateToStringConverter : JsonConverter<DateOnly> {
  public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
    return DateOnly.ParseExact(
      reader.GetString() ?? "19700101",
      "yyyyMMdd",
      CultureInfo.CreateSpecificCulture("ko-KR"),
      DateTimeStyles.AssumeLocal | DateTimeStyles.AdjustToUniversal
    );
  }
  public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options) {
    writer.WriteStringValue(value.ToString("yyyyMMdd"));
  }
}

public class TimeToStringConverter : JsonConverter<TimeOnly> {
  public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
    return TimeOnly.ParseExact(
      reader.GetString() ?? "000000",
      "hhmmss",
      CultureInfo.CreateSpecificCulture("ko-KR"),
      DateTimeStyles.AssumeLocal | DateTimeStyles.AdjustToUniversal
    );
  }
  public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options) {
    writer.WriteStringValue(value.ToString("hhmmss"));
  }
}