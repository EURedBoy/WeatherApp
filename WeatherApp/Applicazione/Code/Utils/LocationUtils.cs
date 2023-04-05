using System.Collections.ObjectModel;
using System.Text.Json;
using SkiaSharp;
using WeatherApp.Applicazione.Code.Model;

namespace WeatherApp.Applicazione.Code.Utils;

public class LocationUtils
{
    public static string favoritePath = FileSystem.AppDataDirectory + "/favorite.json";
    private static List<Locations> favoriteLocations;

    public static List<Locations> GetFavorites()
    {
        if (favoriteLocations != null) return favoriteLocations;
        
        favoriteLocations = new List<Locations>();
        if (!File.Exists(favoritePath)) File.Create(favoritePath);
        else
        {
            string text = File.ReadAllText(favoritePath);
            if (text != "") favoriteLocations = JsonSerializer.Deserialize<List<Locations>>(File.ReadAllText(favoritePath));

            favoriteLocations = favoriteLocations.Select(loc =>
            {
                loc.Favorite = true;
                return loc;
            }).ToList();
        }

        return favoriteLocations;
    }

    public static async Task UpdateFavorite(List<Locations> locations)
    {
        favoriteLocations = locations.ToList();
        Task.Run(async () =>
        {
            if (File.Exists(favoritePath))
            {
                var json = JsonSerializer.Serialize(locations);
                await File.WriteAllTextAsync(favoritePath, json);
            }
        });
    }
    
    private static async Task<bool> CheckFileExistenceAsync(string filePath)
    {
        return await Task.Run(() => File.Exists(filePath));
    }
    public static async Task<ImageSource> DownloadSVG(string url, string countryCode)
    {
        ImageSource ImageS = ImageSource.FromFile("dotnet_bot.png");
        try
        {
            // Save to Directory-Check
            string pathToFile = Path.Combine(FileSystem.CacheDirectory, $"{countryCode}.png");
            // File already downloaded --> just return the existing ImageSource from File
            if (await CheckFileExistenceAsync(pathToFile))
            {
                ImageS = ImageSource.FromFile(pathToFile);
                return ImageS;
            }
            //else try to download...
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);

                // Client reponse --> SVG-File exists
                if (response.IsSuccessStatusCode)
                {


                    var imageBytes = await response.Content.ReadAsByteArrayAsync();

                    using (var stream = new MemoryStream(imageBytes))
                    {
                        var svg = new SkiaSharp.Extended.Svg.SKSvg();
                        svg.Load(stream);


                        // Save 2 canvas
                        var image = new SkiaSharp.SKBitmap((int)svg.CanvasSize.Width, (int)svg.CanvasSize.Height);
                        using (var canvas = new SkiaSharp.SKCanvas(image))
                        {
                            canvas.DrawPicture(svg.Picture);
                        }


                        SKImage img = SKImage.FromBitmap(image);

                        //Create PNG from SKBitmap
                        SKData encodedData = img.Encode(SKEncodedImageFormat.Png, 100);
                        string imgPath = pathToFile;
                        // Open PNG
                        using (var bitmapImageStream = File.Open(imgPath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                               
                            encodedData.SaveTo(bitmapImageStream);

                            // Cleanup
                            await bitmapImageStream.FlushAsync();
                            await bitmapImageStream.DisposeAsync();

                        }
                        // Return ImageSource from File
                        ImageS = ImageSource.FromFile(imgPath);


                    }
                }


            }
        }
        // handle Exceptions
        catch (Exception exp)
        {
            // set ImageSource to null
            ImageS = null;
        }

        return ImageS;
    }
}