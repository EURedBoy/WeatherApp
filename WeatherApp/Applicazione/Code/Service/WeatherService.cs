using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WeatherApp.Applicazione.Code.Enum;
using WeatherApp.Applicazione.Code.Model;
using WeatherApp.Applicazione.Code.Utils;

namespace WeatherApp.Applicazione.Code.Service;

public class WeatherService
{
    private readonly Locations defaultLocation = new("Costamasnaga", "Lombardia",45.8, 9.279999, false, "IT");
    private readonly IGeolocation geolocation;
    private readonly IGeocoding geocoding;
    
    private readonly HttpClient httpClient;

    public bool isGeoActive;

    public WeatherService()
    {
        
        
        var handler = new HttpClientHandler();
        geolocation = Geolocation.Default;
        geocoding = Geocoding.Default;

        handler.DefaultProxyCredentials = CredentialCache.DefaultCredentials;
        httpClient = new HttpClient(handler, true);


        SelectedLocation = defaultLocation;

        isGeoActive = true;
        Init();
    }

    public Locations SelectedLocation { get; set; }

    private void Init()
    {
        if (!Preferences.Default.ContainsKey("geo-location"))
        {
            //TODO: Temporaneo
            if (File.Exists(WeatherApiUtils.PreferencePath))
                SelectedLocation = JsonSerializer.Deserialize<Locations>(
                    File.ReadAllText(WeatherApiUtils.PreferencePath)) ?? defaultLocation;
        }
    }

    public async Task<(List<WeatherDay> week, WeatherDay current)> GetWeatherAsync()
    {
        isGeoActive = false;
        if (Preferences.Default.ContainsKey("geo-location"))
        {
            isGeoActive = true;
            var currentLoc = await GetCurrentLocation();
            if (currentLoc == null) return (null, null);

            SelectedLocation = currentLoc;
        }


        FormattableString formattable =
            $"https://api.open-meteo.com/v1/forecast?latitude={SelectedLocation.Latitude}&longitude={SelectedLocation.Longitude}&&current_weather=true&hourly=temperature_2m,precipitation_probability,windspeed_10m&daily=weathercode,temperature_2m_max,temperature_2m_min,precipitation_probability_max,windspeed_10m_max&timeformat=unixtime&timezone=auto";
        var url = FormattableString.Invariant(formattable);

        var response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            Debug.Print("Errore");
            return (null, null);
        }

        var weatherApi = await response.Content.ReadFromJsonAsync<WeatherAPI>();

        var weatherDays = new List<WeatherDay>();

        var week = weatherApi.Daily;
        var hourly = weatherApi.Hourly;

        int j = 0;
        for (var i = 0; i < week.Time.Count; i++)
        {
            List<WeatherHour> hour = new List<WeatherHour>();

            int k = j + 24;
            for (; j < k; j++)
                hour.Add(new WeatherHour(
                    hourly.Time[j],
                    hourly.Windspeed10m[j],
                    hourly.PrecipitationProbability[j],
                    hourly.Temperature2m[j])
                );

                //Gestire quelli orari
            weatherDays.Add(new WeatherDay(SelectedLocation.Name,
                (week.Temperature2mMax[i] + week.Temperature2mMin[i]) / 2, //Temperatura media
                week.Windspeed10mMax[i],
                week.PrecipitationProbabilityMax[i],
                week.Time[i],
                weatherApi.Timezone,
                week.Weathercode[i],
                hour)
            );
        }

        var current = weatherApi.CurrentWeather;
        var currentWeather = new WeatherDay(SelectedLocation.Name, current.Temperature, current.Windspeed, weatherDays[0].RainFall,
            current.Time,
            weatherApi.Timezone,
            current.Weathercode, new List<WeatherHour>());

        return (weatherDays, currentWeather);
    }

    public async Task<List<Locations>> GetLocationsFromCity(string city)
    {
        var url = $"https://geocoding-api.open-meteo.com/v1/search?name={city}&language=it&count=7";

        var response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            Debug.Print("Errore");
            return null;
        }

        var api = await response.Content.ReadFromJsonAsync<LocationsAPI>();

        return api.Results;
    }

    public async Task<Locations> GetCurrentLocation()
    {
        try
        {
            if (!await CheckPermission())
                await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            if (!await CheckPermission())
                await Alerts.Send(AlertTypes.Permission,
                    "Perfavore imposta il permesso tramite le impostazioni");

            var location = await geolocation.GetLastKnownLocationAsync();

            if (location == null)
                location = await geolocation.GetLocationAsync(
                    new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.Medium,
                        Timeout = TimeSpan.FromSeconds(30)
                    });

            if (location == null)
            {
                await Alerts.Send(AlertTypes.Error, "C'è stato un errore");
                return null;
            }

            var placemarks = await geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
            var placemark = placemarks?.FirstOrDefault();

            if (placemark == null) return null;

            return new Locations(placemark.Locality, placemark.SubLocality, location.Latitude, location.Longitude,
                false, placemark.CountryCode); //Cambiare placemark probabilmente
        }
        // Catch one of the following exceptions:
        //   FeatureNotSupportedException
        //   FeatureNotEnabledException
        //   PermissionException
        catch (Exception ex)
        {
            await Alerts.Send(AlertTypes.Error, "C'è stato un errore " + ex.Message);
            return null;
        }
    }

    private async Task<bool> CheckPermission()
    {
        return await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>() == PermissionStatus.Granted;
    }
}