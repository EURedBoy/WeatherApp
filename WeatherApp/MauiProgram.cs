using CommunityToolkit.Maui;
using Maui.FreakyControls.Extensions;
using Microsoft.Extensions.Logging;

namespace WeatherApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiCommunityToolkit()
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Roboto-Regular.ttf", "RobotoRegular");
                fonts.AddFont("Roboto-Medium.ttf", "RobotoMedium");

            }).ConfigureMauiHandlers(handlers =>
            {
                handlers.AddFreakyHandlers();
            });
        builder.InitSkiaSharp();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}