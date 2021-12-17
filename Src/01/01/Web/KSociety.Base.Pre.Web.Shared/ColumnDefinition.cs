using Blazorise.DataGrid;

namespace KSociety.Base.Pre.Web.Shared;

public class ColumnDefinition
{
    public ColumnDefinition()
    {
        DataType = DataType.NotSet;
        Alignment = Alignment.NotSet;
        ColumnType = DataGridColumnType.Text;
    }

    public DataGridColumnType ColumnType { get; set; }

    public string DataField { get; set; }

    public string Caption { get; set; }

    public DataType DataType { get; set; }

    public string Format { get; set; }

    public Alignment Alignment { get; set; }
}