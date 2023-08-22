// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Pre.Form.View.Forms
{
    using InfraSub.Shared.Interface;
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public partial class DataGridView<T, TList> where T : IObject where TList : IList<T>
    {
        public DataGridView()
        {
            this.InitializeComponent();
            this.Initialize();
        }

        public DataGridView(IContainer container)
        {
            container.Add(this);

            this.InitializeComponent();
            this.Initialize();
        }

        private void Initialize()
        {
            foreach (var propertyInfo in typeof(TList).GetProperties())
            {
                if (!propertyInfo.Name.Equals("List"))
                {
                    this.BindingSourcesComboBox.Add(propertyInfo.Name, null);
                }
            }

            this.AutoGenerateColumns = false;

            DataGridViewColumn col = new DataGridViewFunctionColumn();
            this.Columns.Add(col);

            this.CreateColumns();
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
                    !senderGrid.Rows[e.RowIndex].Cells["Function"].Value.Equals("Remove"))
                {
                    return;
                }

                this.ModifyField?.Invoke(sender, e);
            }).ConfigureAwait(false);
        }

        private async void StdDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            await Task.Factory.StartNew(() =>
            {
                DataGridView senderGrid = (DataGridView)sender;
                if (!(senderGrid.Columns[e.ColumnIndex] is DataGridViewFunctionColumn) || e.RowIndex < 0)
                {
                    return;
                }

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
                            this.Add?.Invoke(sender, myRow);
                        }

                        break;

                    case "Remove":
                        this.StdDataGridView_UserDeletingRow(sender,
                            new DataGridViewRowCancelEventArgs(senderGrid.Rows[e.RowIndex]));
                        break;

                    case "Update":
                        var myUpdatedRow = (T)senderGrid.Rows[e.RowIndex].DataBoundItem;
                        this.UpdateEntity?.Invoke(sender, myUpdatedRow);
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
                    DataGridViewRow row = this.Rows[e.Row.Index - 1];
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

            if (e.Row.Index < 0)
            {
                return;
            }

            senderGrid.CellValueChanged -= this.StdDataGridView_CellValueChanged;
            foreach (DataGridViewCell cell in e.Row.Cells)
            {
                if (!this.BindingSourcesComboBox.ContainsKey(senderGrid.Columns[cell.ColumnIndex].DataPropertyName))
                {
                    continue;
                }

                dynamic data = this.BindingSourcesComboBox[senderGrid.Columns[cell.ColumnIndex].DataPropertyName]
                    .Current;

                //PropertyInfo field = GetType().GetProperty(fieldName);
                //if (field != null) field.SetValue(this, Convert.ChangeType(value, field.PropertyType));

                //var data = _bindingSourcesComboBox[senderGrid.Columns[cell.ColumnIndex].DataPropertyName]
                //    .Current;

                //Type t = typeof(StandardKeyValuePair<int, string>);

                //data = Convert.ChangeType(data, t);

                cell.Value = data.Key;
            }

            senderGrid.CellValueChanged += this.StdDataGridView_CellValueChanged;
            //});
        }

        private async void StdDataGridView_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            await Task.Factory.StartNew(() =>
            {
                if (this.CurrentRow == null)
                {
                    return;
                }

                if (this.CurrentRow.Index >= this.RowCount - 1)
                {
                    return;
                }

                var myRow = (T)this.CurrentRow.DataBoundItem;
                this.Copy?.Invoke(sender, myRow);
            }).ConfigureAwait(false);
        }

        private async void StdDataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            await Task.Factory.StartNew(() =>
            {
                if (this.CurrentRow == null)
                {
                    return;
                }

                if (this.CurrentRow.Index >= this.RowCount - 1)
                {
                    return;
                }

                var myRow = (T)this.CurrentRow.DataBoundItem;
                //Console.WriteLine("StdDataGridView_UserDeletingRow: " + myRow.);
                this.Remove?.Invoke(sender, myRow);
            }).ConfigureAwait(false);
        }
    }
}
