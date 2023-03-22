using System.Text.Json.Serialization;

namespace WeatherApp.Applicazione.Code.Model;

// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
public class Locations
{
    public Locations(string name, double latitude, double longitude, string countryCode)
    {
        Id = -1;
        Name = name;
        Latitude = latitude;
        Longitude = longitude;
        CountryCode = countryCode;
    }

    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("latitude")] public double Latitude { get; set; }

    [JsonPropertyName("longitude")] public double Longitude { get; set; }

    [JsonPropertyName("country_code")] public string CountryCode { get; set; }
}

public class LocationsAPI
{
    [JsonPropertyName("results")] public List<Locations> Results { get; set; }

    [JsonPropertyName("generationtime_ms")]
    public double GenerationtimeMs { get; set; }
}