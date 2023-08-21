namespace KSociety.Base.Pre.Form.View.Forms
{
    using InfraSub.Shared.Interface;
    using System.ComponentModel;

    public partial class DataGridViewReadOnly<T, TList>
        where T : IObject where TList : IList<T>
    {
        public DataGridViewReadOnly()
        {
            this.InitializeComponent();
            this.Initialize();
        }

        public DataGridViewReadOnly(IContainer container)
        {
            container.Add(this);

            this.InitializeComponent();
            this.Initialize();
        }

        private void Initialize()
        {
            this.AutoGenerateColumns = false;
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.AllowDrop = false;

            foreach (var propertyInfo in typeof(TList).GetProperties())
            {
                if (!propertyInfo.Name.Equals("List"))
                {
                    this.BindingSourcesComboBox.Add(propertyInfo.Name, null);
                }
            }
            //AutoGenerateColumns = true;
            //AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;



            //DataGridViewColumn col = new StdDataGridViewFunctionColumn();
            //Columns.Add(col);

            this.CreateColumns();
        }
    }
}
