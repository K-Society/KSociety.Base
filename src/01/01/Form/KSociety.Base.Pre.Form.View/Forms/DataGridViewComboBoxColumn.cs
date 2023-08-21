namespace KSociety.Base.Pre.Form.View.Forms
{
    using System.ComponentModel;

    public partial class DataGridViewComboBoxColumn : System.Windows.Forms.DataGridViewComboBoxColumn
    {
        public DataGridViewComboBoxColumn()
        {
            this.InitializeComponent();
        }

        public DataGridViewComboBoxColumn(IContainer container)
        {
            container.Add(this);

            this.InitializeComponent();
        }
    }
}
