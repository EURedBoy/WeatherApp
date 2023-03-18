using WeatherApp.Applicazione.Code.Utils;

namespace WeatherApp.Applicazione.Code.Model;

public class WeatherDay
{
    public string City { get; set; }
    public double Temperature { get; set; }
    public double WindSpeed { get; set; }
    public double RainFall { get; set; }
    public DateTime Date { get; set; }
    public ImageSource Icon { get; set; }
    public string Description { get; set; }
    public List<WeatherHour> WeatherHours { get; set; } = new List<WeatherHour>();

    public WeatherDay(string city, double temperature, double windSpeed, double rainFall, long unix, int code, List<WeatherHour> weatherHours)
    {
        City = city;
        Temperature = temperature;
        WindSpeed = windSpeed;
        RainFall = rainFall;

        Date = WeatherApiUtils.UnixToDate(unix);
        
        var tempCode = WeatherApiUtils.CodeToImage(code);
        Description = tempCode.text;
        Icon = tempCode.img;

        WeatherHours = weatherHours;
    }

    public override string ToString()
    {
        return $"City: {City}, Temperature: {Temperature}, WindSpeed {WindSpeed}, RainFall {RainFall}, Date: {Date}";
    }
}

public class WeatherHour
{
    public int Hour { get; set; }
    public double WindSpeed { get; set; }
    public double RainFall { get; set; }

    public WeatherHour(int hour, double windSpeed, double rainFall)
    {
        Hour = hour;
        WindSpeed = windSpeed;
        RainFall = rainFall;
    }
}