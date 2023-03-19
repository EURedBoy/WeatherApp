using System.Text.Json;
using WeatherApp.Applicazione.Code.Model;

namespace WeatherApp.Applicazione.Code.Utils;

public class WeatherApiUtils
{
    public static string PreferencePath = FileSystem.AppDataDirectory + "/preference.json";
    
    public static DateTime UnixToDate(long unix, string timezone)
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unix);
        
        var tz = TimeZoneInfo.FindSystemTimeZoneById(timezone);

        return TimeZoneInfo.ConvertTime(dateTime, tz);
    }

    public static (ImageSource img, string text) CodeToImage(int code)
    {
        return code switch
        {
            0 => (ImageSource.FromFile("cloud_icon.png"), "sereno"),
            1 => (ImageSource.FromFile("cloud_icon.png"), "parzialmente soleggiato"),
            2 => (ImageSource.FromFile("cloud_icon.png"), "clear sky"),            
            3 => (ImageSource.FromFile("cloud_icon.png"), "coperto"),
            45 => (ImageSource.FromFile("cloud_icon.png"), "nebbia"),           
            48 => (ImageSource.FromFile("cloud_icon.png"), "deposito di brina"),
            51 => (ImageSource.FromFile("cloud_icon.png"), "pioviggine"),           
            53 => (ImageSource.FromFile("cloud_icon.png"), "pioviggine"),
            55 => (ImageSource.FromFile("cloud_icon.png"), "pioviggine"),
            56 => (ImageSource.FromFile("cloud_icon.png"), "nevischio"),
            57 => (ImageSource.FromFile("cloud_icon.png"), "nevischio"),
            61 => (ImageSource.FromFile("cloud_icon.png"), "pioggia leggera"),
            63 => (ImageSource.FromFile("cloud_icon.png"), "pioggia moderata"),
            65 => (ImageSource.FromFile("cloud_icon.png"), "diluvio"),
            66 => (ImageSource.FromFile("cloud_icon.png"), "pioggia ghiacciata"),
            67 => (ImageSource.FromFile("cloud_icon.png"), "pioggia ghiacciata"),
            71 => (ImageSource.FromFile("cloud_icon.png"), "nevischio"),
            73 => (ImageSource.FromFile("cloud_icon.png"), "neve"),
            75 => (ImageSource.FromFile("cloud_icon.png"), "tormenta"),
            _ => (ImageSource.FromFile("cloud_icon.png"), "developer pigro :)")
        };
    }
}