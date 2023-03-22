namespace WeatherApp.Applicazione.Code.Base;

public abstract class BasePage : ContentPage
{
    public BasePage()
    {
        NavigationPage.SetHasNavigationBar(this, false);
    }

    protected Application Instance { get; } = Application.Current;
}