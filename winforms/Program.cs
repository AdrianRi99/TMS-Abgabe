using TMS.WinForms.Services;

namespace TMS.WinForms;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5078/") };
        var auth = new AuthSession();
        ITmsApiClient apiClient = new TmsApiClient(httpClient, auth);
        var viewModel = new AddressesViewModel(apiClient);

        Application.Run(new AddressesForm(viewModel));
    }
}