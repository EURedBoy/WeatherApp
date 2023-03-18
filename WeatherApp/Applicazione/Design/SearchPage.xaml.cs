using WeatherApp.Applicazione.Code.Base;
using WeatherApp.Applicazione.Code.ViewModel;

namespace WeatherApp.Applicazione.Design;

public partial class SearchPage : BasePage
{
    public SearchPage(SearchViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}