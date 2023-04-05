namespace WeatherApp.Applicazione.Code.Utils;

public class WeatherApiUtils
{
    public static string PreferencePath = FileSystem.AppDataDirectory + "/preference.json";

    public static DateTime UnixToDate(long unix, string timezone)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unix);

        var tz = TimeZoneInfo.FindSystemTimeZoneById(timezone);

        return TimeZoneInfo.ConvertTime(dateTime, tz);
    }

    public static (string text, ImageSource img) CodeToImage(int code, DateTime date)
    {
        return code switch
        {
            0 => ("Cielo sereno", date.Hour is > 6 and < 21 ? ImageSource.FromFile("clear_day") : ImageSource.FromFile("clear_night")),
            1 => ("Prevalentemente limpido", date.Hour is > 6 and < 21 ? ImageSource.FromFile("cloudy_day") : ImageSource.FromFile("cloudy_night")),
            2 => ("Prevalentemente nuvoloso", date.Hour is > 6 and < 21 ? ImageSource.FromFile("cloudy_day") : ImageSource.FromFile("cloudy_night")),
            3 => ("Coperto", ImageSource.FromFile("cloud")),
            45 => ("Nebbia", ImageSource.FromFile("fog")),
            48 => ("Nebbia con brina", ImageSource.FromFile("fog")),
            51 => ("Pioggerellina di scarsa intensità", ImageSource.FromFile("rain")),
            53 => ("Pioggerellina di moderata intensità", ImageSource.FromFile("rain")),
            55 => ("Pioggerellina intensa", ImageSource.FromFile("rain")),
            56 => ("Pioggerellina gelata di scarsa intensità", ImageSource.FromFile("rain")),
            57 => ("Pioggerellina gelata intensa", ImageSource.FromFile("rain")),
            61 => ("Pioggia di scarsa intensità", ImageSource.FromFile("rain")),
            63 => ("Pioggia di moderata intensità", ImageSource.FromFile("rain")),
            65 => ("Pioggia molto intensa", ImageSource.FromFile("rain")),
            66 => ("Pioggia gelata di scarsa intensità", ImageSource.FromFile("rain")),
            67 => ("Pioggia gelata intensa", ImageSource.FromFile("rain")),
            71 => ("Nevicata di lieve intensità", ImageSource.FromFile("snow")),
            73 => ("Nevicata di media intensità", ImageSource.FromFile("snow")),
            75 => ("Nevicata intensa", ImageSource.FromFile("snow")),
            77 => ("Granelli di neve", ImageSource.FromFile("snow")),
            80 => ("Deboli rovesci di pioggia", ImageSource.FromFile("rain")),
            81 => ("Moderati rovesci di pioggia", ImageSource.FromFile("rain")),
            82 => ("Violenti rovesci di pioggia", ImageSource.FromFile("rain")),
            85 => ("Leggeri rovesci di neve", ImageSource.FromFile("snow")),
            86 => ("Pesanti rovesci di neve", ImageSource.FromFile("snow")),
            95 => ("Temporale", ImageSource.FromFile("thunderstorm")),
            96 => ("Temporale con lieve grandine", ImageSource.FromFile("hail")),
            99 => ("Temporale con forte grandine", ImageSource.FromFile("hail")),
            _ => (string.Empty, ImageSource.FromFile("cloud"))
        };
    }
}