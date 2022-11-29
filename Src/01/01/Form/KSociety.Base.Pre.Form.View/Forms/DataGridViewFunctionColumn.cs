using System.ComponentModel;
using System.Windows.Forms;

namespace KSociety.Base.Pre.Form.View.Forms
{
    public partial class DataGridViewFunctionColumn : DataGridViewButtonColumn
    {
        public DataGridViewFunctionColumn()
        {
            InitializeComponent();
            Initialize();
        }

        public DataGridViewFunctionColumn(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {

            DataPropertyName = "Function";
            Name = "Function";
            //Text = "Remove";
            HeaderText = @"Function";
            UseColumnTextForButtonValue = false;

            Width = 60;

            CellTemplate = new DataGridViewFunctionCell();
        }
    }
}