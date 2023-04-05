using System.Text.Json.Serialization;
using WeatherApp.Applicazione.Code.Utils;

namespace WeatherApp.Applicazione.Code.Model;

public class Locations
{
    public Locations(string name, string provincia, double latitude, double longitude, bool favorite, string countryCode)
    {
        Id = -1;
        Name = name;
        Provincia = provincia;
        Latitude = latitude;
        Longitude = longitude;
        CountryCode = countryCode;

        Favorite = LocationUtils.GetFavorites().Contains(this);

        string path = $"https://hatscripts.github.io/circle-flags/flags/{countryCode.ToLower()}.svg";
        Task.Run(async () => Icon = await LocationUtils.DownloadSVG(path, countryCode)).Wait();
    }

    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }
    
    [JsonPropertyName("admin1")] public string Provincia { get; set; }

    [JsonPropertyName("latitude")] public double Latitude { get; set; }

    [JsonPropertyName("longitude")] public double Longitude { get; set; }
    
    [JsonIgnore]
    public ImageSource Icon { get; set; }

    private bool favorite;
    public bool Favorite
    {
        get => favorite;
        set
        {
            favorite = value;
            Heart = favorite ? ImageSource.FromFile("favorite_clicked.png") : ImageSource.FromFile("favorite_normal.png");
        }
    }
    
    [JsonIgnore]
    public ImageSource Heart { get; set; }
    [JsonPropertyName("country_code")] public string CountryCode { get; set; }

    public override bool Equals(object obj)
    {
        if (obj is not Locations) return false;
        Locations locations = (Locations)obj;

        return Latitude == locations.Latitude || Longitude == locations.Longitude; //Forse sistemare con la tolleranza
    }
}

public class LocationsAPI
{
    [JsonPropertyName("results")] public List<Locations> Results { get; set; }

    [JsonPropertyName("generationtime_ms")]
    public double GenerationtimeMs { get; set; }
}