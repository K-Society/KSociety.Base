namespace KSociety.Base.Pre.Web.Shared
{
    using Blazorise.DataGrid;

    public class ColumnDefinition
    {
        public ColumnDefinition()
        {
            this.DataType = DataType.NotSet;
            this.Alignment = Alignment.NotSet;
            this.ColumnType = DataGridColumnType.Text;
        }

        public DataGridColumnType ColumnType { get; set; }

        public string DataField { get; set; }

        public string Caption { get; set; }

        public DataType DataType { get; set; }

        public string Format { get; set; }

        public Alignment Alignment { get; set; }
    }
}
