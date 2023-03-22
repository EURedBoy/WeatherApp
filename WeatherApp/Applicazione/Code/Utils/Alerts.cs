namespace WeatherApp.Applicazione.Code.Utils;

public class Alerts
{
    private static readonly Application? app = Application.Current;

    public static async Task Send(string types, string message)
    {
        if (app?.MainPage == null) return;
        await app.MainPage.DisplayAlert(types, message, "ok");
    }
}