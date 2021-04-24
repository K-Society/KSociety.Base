using KSociety.Base.InfraSub.Shared.Interface;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KSociety.Base.Pre.Form.View.Forms
{
    public partial class DataGridView<T, TList> where T : IObject where TList : IList<T>
    {
        public DataGridView()
        {
            InitializeComponent();
            Initialize();
        }

        public DataGridView(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            foreach (var propertyInfo in typeof(TList).GetProperties())
            {
                if (!propertyInfo.Name.Equals("List"))
                {
                    BindingSourcesComboBox.Add(propertyInfo.Name, null);
                }
            }

            AutoGenerateColumns = false;

            DataGridViewColumn col = new DataGridViewFunctionColumn();
            Columns.Add(col);

            CreateColumns();
        }

        public event EventHandler<T> Remove;
        public event EventHandler<T> Add;
        public event EventHandler<T> UpdateEntity;
        public event EventHandler<T> Copy;
        public event EventHandler<DataGridViewCellEventArgs> ModifyField;

        private async void StdDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            await Task.Factory.StartNew(() =>
            {
                DataGridView senderGrid = (DataGridView)sender;
                if (senderGrid.Columns[e.ColumnIndex] is DataGridViewFunctionColumn || e.RowIndex < 0 ||
                    !senderGrid.Rows[e.RowIndex].Cells["Function"].Value.Equals("Remove")) return;

                ModifyField?.Invoke(sender, e);
            }).ConfigureAwait(false);
        }

        private async void StdDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            await Task.Factory.StartNew(() =>
            {
                DataGridView senderGrid = (DataGridView)sender;
                if (!(senderGrid.Columns[e.ColumnIndex] is DataGridViewFunctionColumn) || e.RowIndex < 0) return;
                switch (senderGrid.Rows[e.RowIndex].Cells["Function"].Value)
                {
                    case "Add":
                        var myRow = (T)senderGrid.Rows[e.RowIndex].DataBoundItem;
                        if (myRow is null)
                        {
                            MessageBox.Show("" + e.RowIndex + " myRow is null");
                        }
                        else
                        {
                            Add?.Invoke(sender, myRow);
                        }
                        break;

                    case "Remove":
                        StdDataGridView_UserDeletingRow(sender, new DataGridViewRowCancelEventArgs(senderGrid.Rows[e.RowIndex]));
                        break;

                    case "Update":
                        var myUpdatedRow = (T)senderGrid.Rows[e.RowIndex].DataBoundItem;
                        UpdateEntity?.Invoke(sender, myUpdatedRow);
                        break;

                    default:
                        MessageBox.Show(@"??? ");
                        break;
                }
            }).ConfigureAwait(false);
        }

        private async void StdDataGridView_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    DataGridViewRow row = Rows[e.Row.Index - 1];
                    row.Cells["Function"].Value = "Add";
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }).ConfigureAwait(false);
        }

        private void StdDataGridView_DataSourceChanged(object sender, EventArgs e)
        {
            //CustomizeGrid(sender as Control);
            //MessageBox.Show(@"DataSourceChanged");
        }

        //No async!
        private /*async*/ void StdDataGridView_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            //await Task.Factory.StartNew(() =>
            //{ 
            DataGridView senderGrid = (DataGridView)sender;

            if (e.Row.Index < 0) return;
            senderGrid.CellValueChanged -= StdDataGridView_CellValueChanged;
            foreach (DataGridViewCell cell in e.Row.Cells)
            {
                if (!BindingSourcesComboBox.ContainsKey(senderGrid.Columns[cell.ColumnIndex].DataPropertyName))
                    continue;
                dynamic data = BindingSourcesComboBox[senderGrid.Columns[cell.ColumnIndex].DataPropertyName]
                    .Current;

                //PropertyInfo field = GetType().GetProperty(fieldName);
                //if (field != null) field.SetValue(this, Convert.ChangeType(value, field.PropertyType));

                //var data = _bindingSourcesComboBox[senderGrid.Columns[cell.ColumnIndex].DataPropertyName]
                //    .Current;

                //Type t = typeof(StandardKeyValuePair<int, string>);

                //data = Convert.ChangeType(data, t);

                cell.Value = data.Key;
            }
            senderGrid.CellValueChanged += StdDataGridView_CellValueChanged;
            //});
        }

        private async void StdDataGridView_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            await Task.Factory.StartNew(() =>
            {
                if (CurrentRow == null) return;
                if (CurrentRow.Index >= RowCount - 1) return;
                var myRow = (T)CurrentRow.DataBoundItem;
                Copy?.Invoke(sender, myRow);
            }).ConfigureAwait(false);
        }

        private async void StdDataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            await Task.Factory.StartNew(() =>
            {
                if (CurrentRow == null) return;
                if (CurrentRow.Index >= RowCount - 1) return;
                var myRow = (T)CurrentRow.DataBoundItem;
                //Console.WriteLine("StdDataGridView_UserDeletingRow: " + myRow.);
                Remove?.Invoke(sender, myRow);
            }).ConfigureAwait(false);
        }
    }
}
