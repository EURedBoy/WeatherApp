using System.Text.Json.Serialization;

namespace WeatherApp.Applicazione.Code.Model;

// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class Locations
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("country_code")]
        public string CountryCode { get; set; }
    }

    public class LocationsAPI
    {
        [JsonPropertyName("results")]
        public List<Locations> Results { get; set; }

        [JsonPropertyName("generationtime_ms")]
        public double GenerationtimeMs { get; set; }
    }



