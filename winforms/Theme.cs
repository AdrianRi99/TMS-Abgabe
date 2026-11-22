namespace TMS.WinForms;

public static class Theme
{
    public static readonly Color Primary = ColorTranslator.FromHtml("#1f4a73");
    public static readonly Color Steel = ColorTranslator.FromHtml("#4b7195");
    public static readonly Color PrimaryLight = ColorTranslator.FromHtml("#e6eef7");
    public static readonly Color Border = ColorTranslator.FromHtml("#dde3ea");
    public static readonly Color Background = ColorTranslator.FromHtml("#f4f6fa");
    public static readonly Color TableHead = ColorTranslator.FromHtml("#eef1f6");
    public static readonly Color Danger = ColorTranslator.FromHtml("#c62828");
    public static readonly Color TextMuted = ColorTranslator.FromHtml("#6b7280");

    public static readonly Font BaseFont = new("Segoe UI", 9.5f);
    public static readonly Font HeaderFont = new("Segoe UI", 16f, FontStyle.Bold);
    public static readonly Font ButtonFont = new("Segoe UI", 9.5f, FontStyle.Bold);

    public static void StylePrimaryButton(Button button)
    {
        button.BackColor = Primary;
        button.ForeColor = Color.White;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.Font = ButtonFont;
        button.Cursor = Cursors.Hand;
        button.Height = 32;
    }

    public static void StyleSecondaryButton(Button button)
    {
        button.BackColor = Color.White;
        button.ForeColor = Primary;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 1;
        button.FlatAppearance.BorderColor = Border;
        button.Font = BaseFont;
        button.Cursor = Cursors.Hand;
        button.Height = 32;
    }

    public static void StyleDangerButton(Button button)
    {
        button.BackColor = Color.White;
        button.ForeColor = Danger;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 1;
        button.FlatAppearance.BorderColor = Danger;
        button.Font = BaseFont;
        button.Cursor = Cursors.Hand;
        button.Height = 32;
    }

    public static void StyleTextBox(TextBox box)
    {
        box.Font = BaseFont;
        box.BorderStyle = BorderStyle.FixedSingle;
        box.Height = 26;
    }

    public static void StyleGrid(DataGridView grid)
    {
        grid.BackgroundColor = Color.White;
        grid.BorderStyle = BorderStyle.None;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.RowHeadersVisible = false;
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersDefaultCellStyle.BackColor = TableHead;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Steel;
        grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
        grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 6, 8, 6);
        grid.ColumnHeadersHeight = 36;
        grid.DefaultCellStyle.Font = BaseFont;
        grid.DefaultCellStyle.SelectionBackColor = PrimaryLight;
        grid.DefaultCellStyle.SelectionForeColor = Color.Black;
        grid.DefaultCellStyle.Padding = new Padding(4, 4, 4, 4);
        grid.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#fafbfc");
        grid.RowTemplate.Height = 32;
        grid.GridColor = Border;
        grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
    }
}