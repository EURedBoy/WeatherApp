using CommunityToolkit.Mvvm.ComponentModel;

namespace WeatherApp.Applicazione.Code.Base;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool isBusy;

    [ObservableProperty] private string title;

    public bool IsNotBusy => !IsBusy;
}