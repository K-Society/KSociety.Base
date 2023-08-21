namespace KSociety.Base.Pre.Form.View.Forms
{
    using System.ComponentModel;
    using System.Windows.Forms;

    public partial class DataGridViewFunctionColumn : DataGridViewButtonColumn
    {
        public DataGridViewFunctionColumn()
        {
            this.InitializeComponent();
            this.Initialize();
        }

        public DataGridViewFunctionColumn(IContainer container)
        {
            container.Add(this);

            this.InitializeComponent();
            this.Initialize();
        }

        private void Initialize()
        {
            this.DataPropertyName = "Function";
            this.Name = "Function";
            //Text = "Remove";
            this.HeaderText = @"Function";
            this.UseColumnTextForButtonValue = false;

            this.Width = 60;

            this.CellTemplate = new DataGridViewFunctionCell();
        }
    }
}
