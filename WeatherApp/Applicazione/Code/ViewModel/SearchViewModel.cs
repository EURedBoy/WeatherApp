using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WeatherApp.Applicazione.Code.Base;
using WeatherApp.Applicazione.Code.Model;
using WeatherApp.Applicazione.Code.Service;
using WeatherApp.Applicazione.Code.Utils;

namespace WeatherApp.Applicazione.Code.ViewModel;

public partial class SearchViewModel : BaseViewModel
{
    public delegate Task UpdateEventHandler(); //😀😀😀😀


    [ObservableProperty] public List<Locations> locationsList;

    private string searchText;
    private readonly WeatherService service;

    public SearchViewModel()
    {
        LocationsList = new List<Locations>();
        service = App.WeatherService;
    }

    public string SearchText
    {
        get => searchText;
        set
        {
            Task.Run(async () => await TextChange());

            searchText = value;
            OnPropertyChanged();
        }
    }

    public event UpdateEventHandler Update;

    private async Task TextChange()
    {
        LocationsList =
            await service.GetLocationsFromCity(SearchText); //.DistinctBy(loc => loc.Name && loc.CountryCode).ToList();
    }

    [RelayCommand]
    private async void Back()
    {
        await Application.Current.MainPage.Navigation.PopAsync();
    }

    [RelayCommand]
    private async void Select(Locations locations)
    {
        if (IsBusy) return;
        IsBusy = true;

        Preferences.Default.Remove("geo-location");

        service.SelectedLocation = locations;

        //TODO: Temporaneo
        var json = JsonSerializer.Serialize(locations);
        await File.WriteAllTextAsync(WeatherApiUtils.PreferencePath, json);

        await Update();
        await Application.Current.MainPage.Navigation.PopAsync();
    }

    [RelayCommand]
    private async void GeoLocation()
    {
        var location = await service.GetCurrentLocation();
        if (location == null) return;

        Preferences.Default.Set("geo-location", true);

        service.SelectedLocation = location;

        //TODO: Temporaneo
        var json = JsonSerializer.Serialize(location);
        await File.WriteAllTextAsync(WeatherApiUtils.PreferencePath, json);

        await Update();
        await Application.Current.MainPage.Navigation.PopAsync();
    }
}