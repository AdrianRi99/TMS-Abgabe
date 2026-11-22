using TMS.WinForms.Models;

namespace TMS.WinForms;

public class AddressFormDialog : Form
{
    private readonly TextBox _name = new() { Width = 300 };
    private readonly TextBox _street = new() { Width = 300 };
    private readonly TextBox _houseNumber = new() { Width = 300 };
    private readonly TextBox _supplement = new() { Width = 300 };
    private readonly TextBox _cityName = new() { Width = 300 };
    private readonly TextBox _zipCode = new() { Width = 300 };
    private readonly TextBox _countryName = new() { Width = 300 };
    private readonly Label _errorLabel = new() { ForeColor = Theme.Danger, AutoSize = true };

    public CreateAddressRequest? Result { get; private set; }

    public AddressFormDialog(AddressDto? existing)
    {
        Text = "";
        Width = 420;
        Height = 520;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;
        Font = Theme.BaseFont;

        Theme.StyleTextBox(_name);
        Theme.StyleTextBox(_street);
        Theme.StyleTextBox(_houseNumber);
        Theme.StyleTextBox(_supplement);
        Theme.StyleTextBox(_cityName);
        Theme.StyleTextBox(_zipCode);
        Theme.StyleTextBox(_countryName);

        var titleLabel = new Label
        {
            Text = existing is null ? "Neue Adresse" : "Adresse bearbeiten",
            Font = new Font("Segoe UI", 13f, FontStyle.Bold),
            ForeColor = Theme.Steel,
            AutoSize = true,
            Location = new Point(16, 14)
        };
        Controls.Add(titleLabel);

        var layout = new TableLayoutPanel
        {
            ColumnCount = 2,
            AutoSize = true,
            Location = new Point(16, 50),
            Width = 380
        };

        AddRow(layout, "Name / Firma", _name);
        AddRow(layout, "Straße", _street);
        AddRow(layout, "Hausnummer", _houseNumber);
        AddRow(layout, "Zusatz", _supplement);
        AddRow(layout, "Stadt", _cityName);
        AddRow(layout, "PLZ", _zipCode);
        AddRow(layout, "Land", _countryName);

        _errorLabel.MaximumSize = new Size(360, 0);
        layout.Controls.Add(_errorLabel);
        layout.SetColumnSpan(_errorLabel, 2);

        Controls.Add(layout);

        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(16),
            Height = 60
        };

        var saveButton = new Button { Text = "Speichern", Width = 110 };
        Theme.StylePrimaryButton(saveButton);
        saveButton.Click += SaveButton_Click;

        var cancelButton = new Button { Text = "Abbrechen", Width = 110, Margin = new Padding(0, 0, 8, 0) };
        Theme.StyleSecondaryButton(cancelButton);
        cancelButton.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

        buttonPanel.Controls.Add(saveButton);
        buttonPanel.Controls.Add(cancelButton);
        Controls.Add(buttonPanel);

        if (existing is not null)
        {
            _name.Text = existing.Name;
            _street.Text = existing.Street;
            _houseNumber.Text = existing.HouseNumber;
            _supplement.Text = existing.Supplement ?? "";
            _cityName.Text = existing.CityName;
            _zipCode.Text = existing.ZipCode;
            _countryName.Text = existing.CountryName;
        }
    }

    private static void AddRow(TableLayoutPanel layout, string label, Control input)
    {
        layout.Controls.Add(new Label
        {
            Text = label,
            AutoSize = true,
            ForeColor = Theme.TextMuted,
            Font = new Font("Segoe UI", 8f),
            Margin = new Padding(0, 10, 8, 0)
        });
        layout.Controls.Add(input);
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_name.Text) ||
            string.IsNullOrWhiteSpace(_street.Text) ||
            string.IsNullOrWhiteSpace(_houseNumber.Text) ||
            string.IsNullOrWhiteSpace(_cityName.Text) ||
            string.IsNullOrWhiteSpace(_zipCode.Text) ||
            string.IsNullOrWhiteSpace(_countryName.Text))
        {
            _errorLabel.Text = "Bitte alle Pflichtfelder ausfüllen.";
            return;
        }

        Result = new CreateAddressRequest(
            _name.Text.Trim(),
            _street.Text.Trim(),
            _houseNumber.Text.Trim(),
            string.IsNullOrWhiteSpace(_supplement.Text) ? null : _supplement.Text.Trim(),
            _cityName.Text.Trim(),
            _zipCode.Text.Trim(),
            _countryName.Text.Trim());

        DialogResult = DialogResult.OK;
        Close();
    }
}