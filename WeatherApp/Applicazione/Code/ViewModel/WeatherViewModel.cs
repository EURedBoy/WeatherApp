using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WeatherApp.Applicazione.Code.Model;
using WeatherApp.Applicazione.Code.Service;
using WeatherApp.Applicazione.Design;

namespace WeatherApp.Applicazione.Code.ViewModel;

public partial class WeatherViewModel : ObservableObject
{
    private WeatherService service;

    private string citt = "Costamasnaga";
    private double lati = 45.8;
    private double longi = 9.279999;

    [ObservableProperty] 
    public ImageSource background;

    [ObservableProperty] 
    public bool isRefreshing;

    [ObservableProperty] 
    public WeatherDay currentWeather;

    [ObservableProperty] 
    public List<WeatherDay> weeklyWeather;
    
    public WeatherViewModel()
    {
        service = App.WeatherService;

        Task.Run(async () => await Update());
    }

    public async Task Update()
    {
        Locations loc = service.SelectedLocation;
        (List<WeatherDay> week, WeatherDay current) apiResponse = await service.GetWeatherAsync(loc.Name, loc.Longitude, loc.Latitude);
        

        WeeklyWeather = apiResponse.week 
                        ?? new List<WeatherDay>();
        CurrentWeather = apiResponse.current 
                         ?? new WeatherDay("NaN", 0, 0, 0, 0, 0, new List<WeatherHour>());
        
        Background = CurrentWeather.Date.Hour switch
        {
            >= 6 and < 17 => ImageSource.FromFile("day_background.jpg"),
            >= 17 and < 20 => ImageSource.FromFile("sunset_background.jpg"),
            _ => ImageSource.FromFile("night_background.jpg")
        };
    }
    
    [RelayCommand]
    private async void OnRefresh()
    {
        IsRefreshing = true;
        await Update();
        IsRefreshing = false;
    }

    [RelayCommand]
    private async void GotoSearch()
    {
        SearchViewModel viewModel = new SearchViewModel();
        viewModel.Update += Update;
        await App.Current.MainPage.Navigation.PushAsync(new SearchPage(viewModel));
    }
    
    
}