using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WeatherApp.Applicazione.Code.Model;
using WeatherApp.Applicazione.Code.Service;
using WeatherApp.Applicazione.Design.Ios;

namespace WeatherApp.Applicazione.Code.ViewModel;

public partial class WeatherViewModel : BaseViewModel
{
    private WeatherService service;

    private string citt = "Costamasnaga";
    private double lati = 45.8;
    private double longi = 9.279999;

    [ObservableProperty] 
    public ImageSource background = ImageSource.FromFile("day_background.jpg");

    [ObservableProperty] 
    public WeatherDay currentWeather;

    [ObservableProperty] 
    public bool isReloading;

    [ObservableProperty] 
    public List<WeatherDay> weeklyWeather;
    
    public WeatherViewModel()
    {
        service = App.WeatherService;

        Task.Run(async () => await OnLoad());
    }

    public async Task Update()
    {

        Locations loc = service.SelectedLocation;
        (List<WeatherDay> week, WeatherDay current) apiResponse = await service.GetWeatherAsync(loc.Name, loc.Longitude, loc.Latitude);
        

        WeeklyWeather = apiResponse.week 
                        ?? new List<WeatherDay>();
        CurrentWeather = apiResponse.current 
                         ?? new WeatherDay("NaN", 0, 0, 0, 0, "CET", 0, new List<WeatherHour>());

        Background = CurrentWeather.Date.Hour switch
        {
            >= 6 and < 17 => ImageSource.FromFile("day_background.jpg"),
            >= 17 and < 20 => ImageSource.FromFile("sunset_background.jpg"),
            _ => ImageSource.FromFile("night_background.jpg")
        };
        
    }

    private async Task OnLoad()
    {
        IsBusy = true;
        await Update();
        IsBusy = false;
    }
    
    [RelayCommand]
    private async void OnRefresh()
    {
        IsReloading = true;
        await Update();
        IsReloading = false;
    }

    [RelayCommand]
    private async void GotoSearch()
    {
        SearchViewModel viewModel = new SearchViewModel();
        viewModel.Update += Update;
        await App.Current.MainPage.Navigation.PushAsync(new SearchPage(viewModel));
    }
    
    
}