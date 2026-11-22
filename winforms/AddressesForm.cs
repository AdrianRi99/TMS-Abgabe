using TMS.WinForms.Models;

namespace TMS.WinForms;

public partial class AddressesForm : Form
{
    private readonly AddressesViewModel _vm;

    private DataGridView _grid = null!;
    private TextBox _filterStreet = null!;
    private TextBox _filterCity = null!;
    private TextBox _filterCountry = null!;
    private Button _searchButton = null!;
    private Button _resetButton = null!;
    private Button _newButton = null!;
    private Label _pageLabel = null!;
    private Button _prevButton = null!;
    private Button _nextButton = null!;

    public AddressesForm(AddressesViewModel vm)
    {
        _vm = vm;
        BuildLayout();
        WireEvents();
        Load += AddressesForm_Load;
    }

    private void WireEvents()
    {
        _searchButton.Click += async (_, _) =>
        {
            _vm.FilterStreet = _filterStreet.Text;
            _vm.FilterCity = _filterCity.Text;
            _vm.FilterCountry = _filterCountry.Text;
            await _vm.SearchAsync();
            RenderGrid();
        };

        _resetButton.Click += async (_, _) =>
        {
            _filterStreet.Text = "";
            _filterCity.Text = "";
            _filterCountry.Text = "";
            await _vm.ResetFilterAsync();
            RenderGrid();
        };

        _newButton.Click += async (_, _) => await OpenCreateDialogAsync();

        _grid.CellDoubleClick += async (_, e) =>
        {
            if (e.RowIndex < 0 || e.RowIndex >= _vm.Addresses.Count) return;
            await OpenEditDialogAsync(_vm.Addresses[e.RowIndex]);
        };

        _grid.ColumnHeaderMouseClick += async (_, e) =>
        {
            if (!_vm.TryGetSortField(e.ColumnIndex, out var field)) return;
            await _vm.SortAsync(field);
            RenderGrid();
        };

        _nextButton.Click += async (_, _) => { await _vm.NextPageAsync(); RenderGrid(); };
        _prevButton.Click += async (_, _) => { await _vm.PreviousPageAsync(); RenderGrid(); };
    }

    private async void AddressesForm_Load(object? sender, EventArgs e)
    {
        Text = "TMS – Adressverwaltung (WinForms)";

        try
        {
            var connected = await _vm.InitializeAsync();
            if (!connected)
            {
                MessageBox.Show(
                    "Verbindung zum Server fehlgeschlagen. Läuft das Backend auf http://localhost:5078?",
                    "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            RenderGrid();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler beim Laden: {ex.Message}", "Fehler",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Spiegelt den ViewModel-State in die UI – einzige Stelle die das Grid aktualisiert
    private void RenderGrid()
    {
        _grid.DataSource = null;
        _grid.DataSource = _vm.Addresses;
        _pageLabel.Text = $"Seite {_vm.CurrentPage} von {_vm.TotalPages}";
        _prevButton.Enabled = _vm.CanGoPrevious;
        _nextButton.Enabled = _vm.CanGoNext;
    }

    private async Task OpenCreateDialogAsync()
    {
        using var dialog = new AddressFormDialog(null);
        if (dialog.ShowDialog(this) != DialogResult.OK || dialog.Result is null) return;

        try
        {
            await _vm.CreateAddressAsync(dialog.Result);
            RenderGrid();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler beim Anlegen: {ex.Message}", "Fehler",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task OpenEditDialogAsync(AddressDto address)
    {
        using var dialog = new AddressFormDialog(address);
        if (dialog.ShowDialog(this) != DialogResult.OK || dialog.Result is null) return;

        try
        {
            await _vm.UpdateAddressAsync(address.Id, dialog.Result);
            RenderGrid();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler beim Speichern: {ex.Message}", "Fehler",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}