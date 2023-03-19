using WeatherApp.Applicazione.Code.Service;
using WeatherApp.Applicazione.Design.Ios;
using WeatherApp.Applicazione.Design.Android;

namespace WeatherApp;

public partial class App : Application
{
    public static WeatherService WeatherService { get; private set; }
    public App()
    {
        InitializeComponent();
        
        WeatherService = new WeatherService();

#if ANDROID
        MainPage = new NavigationPage(new AndroidHomePage());
#elif IOS
        MainPage = new NavigationPage(new HomePage());
#endif
    }
}