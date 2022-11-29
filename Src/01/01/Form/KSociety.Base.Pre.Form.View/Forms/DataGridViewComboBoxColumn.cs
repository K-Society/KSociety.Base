using System.ComponentModel;

namespace KSociety.Base.Pre.Form.View.Forms
{
    public partial class DataGridViewComboBoxColumn : System.Windows.Forms.DataGridViewComboBoxColumn
    {
        public DataGridViewComboBoxColumn()
        {
            InitializeComponent();
        }

        public DataGridViewComboBoxColumn(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}