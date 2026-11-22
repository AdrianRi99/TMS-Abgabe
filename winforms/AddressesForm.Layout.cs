namespace TMS.WinForms;

public partial class AddressesForm
{
    private void BuildLayout()
    {
        Width = 1050;
        Height = 680;
        StartPosition = FormStartPosition.CenterScreen;
        Font = Theme.BaseFont;
        BackColor = Theme.Background;
        Icon = SystemIcons.Application;

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 3,
            ColumnCount = 1,
            Padding = new Padding(16)
        };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        Controls.Add(root);

        root.Controls.Add(BuildFilterPanel(), 0, 0);
        root.Controls.Add(BuildListPanel(), 0, 1);
        root.Controls.Add(BuildPagingPanel(), 0, 2);
    }

    private FlowLayoutPanel BuildFilterPanel()
    {
        var filterPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            WrapContents = false,
            Padding = new Padding(16),
            BackColor = Color.White,
            Margin = new Padding(0, 0, 0, 16)
        };

        filterPanel.Controls.Add(LabeledTextBox("Straße", out _filterStreet));
        filterPanel.Controls.Add(LabeledTextBox("Stadt", out _filterCity));
        filterPanel.Controls.Add(LabeledTextBox("Land", out _filterCountry));

        _searchButton = new Button { Text = "Suchen", Width = 100, Margin = new Padding(8, 21, 4, 0) };
        Theme.StylePrimaryButton(_searchButton);
        filterPanel.Controls.Add(_searchButton);

        _resetButton = new Button { Text = "Zurücksetzen", Width = 110, Margin = new Padding(4, 21, 0, 0) };
        Theme.StyleSecondaryButton(_resetButton);
        filterPanel.Controls.Add(_resetButton);

        return filterPanel;
    }

    private TableLayoutPanel BuildListPanel()
    {
        var listPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            BackColor = Color.White,
            Padding = new Padding(16)
        };
        listPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        listPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var headerRow = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 12)
        };
        headerRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        headerRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        var titleLabel = new Label
        {
            Text = "Adressen",
            Font = Theme.HeaderFont,
            ForeColor = Theme.Steel,
            AutoSize = true,
            Anchor = AnchorStyles.Left
        };
        headerRow.Controls.Add(titleLabel, 0, 0);

        _newButton = new Button { Text = "+ Neue Adresse", Width = 150, Anchor = AnchorStyles.Right };
        Theme.StylePrimaryButton(_newButton);
        headerRow.Controls.Add(_newButton, 1, 0);

        listPanel.Controls.Add(headerRow, 0, 0);

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = false,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false
        };
        Theme.StyleGrid(_grid);

        AddColumn("Name", nameof(Models.AddressDto.Name), 170);
        AddColumn("Straße", nameof(Models.AddressDto.Street), 150);
        AddColumn("Hausnummer", nameof(Models.AddressDto.HouseNumber), 95);
        AddColumn("Zusatz", nameof(Models.AddressDto.Supplement), 110);
        AddColumn("PLZ", nameof(Models.AddressDto.ZipCode), 75);
        AddColumn("Stadt", nameof(Models.AddressDto.CityName), 120);
        AddColumn("Land", nameof(Models.AddressDto.CountryName), 130);

        listPanel.Controls.Add(_grid, 0, 1);
        return listPanel;
    }

    private FlowLayoutPanel BuildPagingPanel()
    {
        var pagingPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true,
            Padding = new Padding(0, 12, 0, 0)
        };

        _nextButton = new Button { Text = "Vor", Width = 90 };
        Theme.StylePrimaryButton(_nextButton);

        _prevButton = new Button { Text = "Zurück", Width = 90, Margin = new Padding(8, 0, 0, 0) };
        Theme.StyleSecondaryButton(_prevButton);

        _pageLabel = new Label
        {
            Text = "Seite 1 von 1",
            AutoSize = true,
            ForeColor = Theme.TextMuted,
            Margin = new Padding(8, 9, 8, 0)
        };

        pagingPanel.Controls.Add(_nextButton);
        pagingPanel.Controls.Add(_prevButton);
        pagingPanel.Controls.Add(_pageLabel);

        return pagingPanel;
    }

    private static Panel LabeledTextBox(string label, out TextBox textBox)
    {
        var panel = new Panel { Width = 170, Height = 55, Margin = new Padding(0, 0, 10, 0) };
        var lbl = new Label
        {
            Text = label,
            AutoSize = true,
            Top = 0,
            Left = 2,
            ForeColor = Theme.TextMuted,
            Font = new Font("Segoe UI", 8f)
        };
        var box = new TextBox { Top = 20, Left = 0, Width = 170 };
        Theme.StyleTextBox(box);
        panel.Controls.Add(lbl);
        panel.Controls.Add(box);
        textBox = box;
        return panel;
    }

    private void AddColumn(string header, string propertyName, int width)
    {
        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = header,
            DataPropertyName = propertyName,
            Width = width,
            SortMode = DataGridViewColumnSortMode.Programmatic
        });
    }
}