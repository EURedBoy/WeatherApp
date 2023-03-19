using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WeatherApp.Applicazione.Code.Model;
using WeatherApp.Applicazione.Code.Utils;

namespace WeatherApp.Applicazione.Code.Service;

public class WeatherService
{
    private HttpClient httpClient;

    public Locations SelectedLocation { get; set; } = new()
        { Name = "Costamasnaga", Latitude = 45.8, Longitude = 9.279999, CountryCode = "IT" };
     
    public WeatherService()
    {
        var handler = new HttpClientHandler();
        handler.DefaultProxyCredentials = CredentialCache.DefaultCredentials;
        httpClient = new HttpClient(handler, disposeHandler: true);
        
        //TODO: Temporaneo
        if (File.Exists(WeatherApiUtils.PreferencePath))
            SelectedLocation = JsonSerializer.Deserialize<Locations>(File.ReadAllText(WeatherApiUtils.PreferencePath));
    }

    public async Task<(List<WeatherDay> week, WeatherDay current)> GetWeatherAsync(string city, double lon, double lat)
    {
        FormattableString formattable = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&&current_weather=true&hourly=temperature_2m,precipitation_probability,windspeed_10m&daily=weathercode,temperature_2m_max,temperature_2m_min,precipitation_probability_max,windspeed_10m_max&timeformat=unixtime&timezone=auto";
        var url = FormattableString.Invariant(formattable);
        
        var response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            Debug.Print("Errore");
            return (null, null);
        }

        WeatherAPI weatherApi = await response.Content.ReadFromJsonAsync<WeatherAPI>();

        List<WeatherDay> weatherDays = new List<WeatherDay>();

        Daily week = weatherApi.Daily;

        for (int i = 0; i < week.Time.Count; i++)
        {

            //Gestire quelli orari
            
            weatherDays.Add(new WeatherDay(city, 
                (week.Temperature2mMax[i] + week.Temperature2mMin[i]) / 2,  //Temperatura media
                week.Windspeed10mMax[i], 
                week.PrecipitationProbabilityMax[i], 
                week.Time[i], 
                weatherApi.Timezone,
                week.Weathercode[i],
                new List<WeatherHour>())
            );
        }

        CurrentWeather current = weatherApi.CurrentWeather;
        WeatherDay currentWeather = new WeatherDay(city, current.Temperature, current.Windspeed, weatherDays[0].RainFall, current.Time, 
            weatherApi.Timezone,
            current.Weathercode, new List<WeatherHour>());

        return (weatherDays, currentWeather);
    }

    public async Task<List<Locations>> GetLocations(string city)
    {
        var url = $"https://geocoding-api.open-meteo.com/v1/search?name={city}&language=it&count=7";
        
        var response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            Debug.Print("Errore");
            return null;
        }
        LocationsAPI api = await response.Content.ReadFromJsonAsync<LocationsAPI>();

        return api.Results;
    }
}