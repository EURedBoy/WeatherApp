using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WeatherApp.Applicazione.Code.Model;
using WeatherApp.Applicazione.Code.Service;
using WeatherApp.Applicazione.Code.Utils;

namespace WeatherApp.Applicazione.Code.ViewModel;

public partial class SearchViewModel : ObservableObject
{
    private WeatherService service;
    public delegate Task UpdateEventHandler(); //😀😀😀😀
    public event UpdateEventHandler Update;
    

    [ObservableProperty]
    public List<Locations> locationsList;

    private string searchText;
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

    public SearchViewModel()
    {
        LocationsList = new List<Locations>();
        
        service = App.WeatherService;
    }

    private async Task TextChange()
    {
        LocationsList = await service.GetLocations(SearchText); //.DistinctBy(loc => loc.Name && loc.CountryCode).ToList();
    }

    [RelayCommand]
    private async void Back()
    {
        await App.Current.MainPage.Navigation.PopAsync();
    }

    [RelayCommand]
    private async void Select(Locations locations)
    {
        service.SelectedLocation = locations;
        
        //TODO: Temporaneo
        var json = JsonSerializer.Serialize(locations);
        await File.WriteAllTextAsync(WeatherApiUtils.PreferencePath, json);
        
        await Update();
        await App.Current.MainPage.Navigation.PopAsync();
    }
}