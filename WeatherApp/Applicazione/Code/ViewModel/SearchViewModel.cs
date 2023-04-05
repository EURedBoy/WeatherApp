using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Maui.Core.Extensions;
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


    [ObservableProperty] public ObservableCollection<Locations> locationsList;
    [ObservableProperty] public ObservableCollection<Locations> favoriteLocations;

    private string searchText;
    private readonly WeatherService service;

    public SearchViewModel()
    {
        LocationsList = new ObservableCollection<Locations>();
        FavoriteLocations = LocationUtils.GetFavorites().ToObservableCollection();
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
            (await service.GetLocationsFromCity(SearchText)).ToObservableCollection();
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

        locations.Icon = null; //Senza questo la serilizzazione muore
        //TODO: Temporaneo
        var json = JsonSerializer.Serialize(locations);
        await File.WriteAllTextAsync(WeatherApiUtils.PreferencePath, json);

        await Update();
        await Application.Current.MainPage.Navigation.PopAsync();
        IsBusy = false;
    }

    [RelayCommand]
    private async void AddFavorite(Locations locations)
    {
        if (locations.Favorite) FavoriteLocations.Remove(locations);
        else
        { 
            if (FavoriteLocations.Contains(locations)) return; 
            FavoriteLocations.Add(locations);
        }
        
        locations.Favorite = !locations.Favorite;

        OnPropertyChanged("LocationsList");

        await LocationUtils.UpdateFavorite(FavoriteLocations.ToList());
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