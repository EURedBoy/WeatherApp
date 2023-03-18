using WeatherApp.Applicazione.Code.Service;
using WeatherApp.Applicazione.Design;

namespace WeatherApp;

public partial class App : Application
{
    public static WeatherService WeatherService { get; private set; }
    public App()
    {
        InitializeComponent();
        
        WeatherService = new WeatherService();
        MainPage = new NavigationPage(new HomePage());
    }
}