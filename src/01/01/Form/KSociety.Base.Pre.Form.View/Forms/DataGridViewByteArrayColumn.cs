namespace KSociety.Base.Pre.Form.View.Forms
{
    using System.ComponentModel;
    using System.Windows.Forms;

    public partial class DataGridViewByteArrayColumn : DataGridViewTextBoxColumn
    {
        public DataGridViewByteArrayColumn()
        {
            this.InitializeComponent();
            this.Initialize();
        }

        public DataGridViewByteArrayColumn(IContainer container)
        {
            container.Add(this);

            this.InitializeComponent();
            this.Initialize();
        }

        private void Initialize()
        {
            this.CellTemplate = new DataGridViewByteArrayCell();
        }
    }
}
