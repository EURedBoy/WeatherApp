using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WeatherApp.Applicazione.Code.Base;
using WeatherApp.Applicazione.Code.Model;
using WeatherApp.Applicazione.Code.Service;
using WeatherApp.Applicazione.Design.Ios;

namespace WeatherApp.Applicazione.Code.ViewModel;

public partial class WeatherViewModel : BaseViewModel
{
    [ObservableProperty] public ImageSource background = ImageSource.FromFile("day_background.jpg");

    private string citt = "Costamasnaga";

    [ObservableProperty] public WeatherDay currentWeather;

    [ObservableProperty] public bool isReloading;

    private double lati = 45.8;
    private double longi = 9.279999;
    private readonly WeatherService service;

    [ObservableProperty] public List<WeatherDay> weeklyWeather;

    public WeatherViewModel()
    {
        service = App.WeatherService;

        Task.Run(async () => await OnLoad());
    }

    private async Task Update()
    {
        var apiResponse = await service.GetWeatherAsync(service.SelectedLocation);


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
        await service.UpdateLocationIfPossible();
        await Update();
        IsReloading = false;
    }

    [RelayCommand]
    private async void GotoSearch()
    {
        var viewModel = new SearchViewModel();
        viewModel.Update += Update;
        await Application.Current.MainPage.Navigation.PushAsync(new SearchPage(viewModel));
    }
}