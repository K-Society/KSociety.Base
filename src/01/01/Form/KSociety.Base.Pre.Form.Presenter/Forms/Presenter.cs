namespace KSociety.Base.Pre.Form.Presenter.Forms
{
    using InfraSub.Shared.Interface;
    using Abstractions;
    using KSociety.Base.Pre.Form.View.Abstractions;
    using Srv.Agent;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Drawing;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public abstract class Presenter<TView, T, TAddReq, TAddRes, TUpdateReq, TUpdateRes, TCopyReq, TCopyRes,
            TModifyFieldReq, TModifyFieldRes, TRemoveReq, TRemoveRes, TList, TCommand, TQuery>
        : PresenterBase<TView, T, TList, TQuery>, IPresenter<TView, T, TList>
        where T : IObject, IAppDtoObject<TRemoveReq, TAddReq, TUpdateReq, TCopyReq>
        where TAddReq : class
        where TAddRes : class, IIdObject
        where TUpdateReq : class
        where TUpdateRes : class, IIdObject
        where TCopyReq : class
        where TCopyRes : class, IIdObject
        where TModifyFieldReq : class, IModifyField, new()
        where TModifyFieldRes : class, IBoolResponse
        where TRemoveReq : class
        where TRemoveRes : class, IBoolResponse
        where TList : IList<T>
        where TCommand : IAgentCommand<TAddReq, TAddRes, TUpdateReq, TUpdateRes, TCopyReq, TCopyRes, TModifyFieldReq,
            TModifyFieldRes, TRemoveReq, TRemoveRes>
        where TQuery : Srv.Agent.List.GridView.IAgentQueryModel<T, TList>
        where TView : IView<T, TList>
    {
        protected TCommand CommandModel { get; private set; }

        protected Presenter(TView view, TCommand commandModel, TQuery queryModel, ILoggerFactory loggerFactory)
            : base(view, queryModel, loggerFactory)
        {
            this.CommandModel = commandModel;
            //Logger.LogDebug("");
            this.WireViewEvents();
        }

        private void WireViewEvents()
        {
            this.View.Remove += this.ViewRemove;

            this.View.Add += this.ViewAdd;

            this.View.UpdateEntity += this.ViewUpdate;

            this.View.Copy += this.ViewCopy;

            this.View.ModifyField += this.ViewModifyField;
        }

        private async void ViewRemove(object sender, T e)
        {
            await Task.Run(() =>
            {
                var result = this.CommandModel.Remove(e.GetRemoveReq());

                if (result.Result)
                {
                    this.View.BindingSource.Remove(e);
                }
            }).ConfigureAwait(false);
        }

        private async void ViewAdd(object sender, T e)
        {
            var dgv = (TView)sender;

            if (dgv.DataGridView.CurrentRow == null)
            {
                return;
            }

            if (dgv.DataGridView.CurrentRow.Index >= dgv.DataGridView.RowCount - 1)
            {
                return;
            }

            var row = dgv.DataGridView.CurrentRow;

            await Task.Run(() =>
            {
                try
                {
                    //if (e == null)
                    //{
                    //    MessageBox.Show("e is null");
                    //}

                    //if (CommandModel == null)
                    //{
                    //    MessageBox.Show("CommandModel is null!");
                    //}

                    //try
                    //{
                    //    var resultt = e.GetAddReq();

                    //    if (resultt == null)
                    //    {
                    //        MessageBox.Show("GetAddReq null");
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    MessageBox.Show(ex.Message + " " + ex.Source);
                    //}
                    var addResult = this.CommandModel.Add(e.GetAddReq());

                    if (addResult.Id.Equals(Guid.Empty))
                    {
                        //result.Item2.DataGridView.CurrentRow.ErrorText = "Pippo";
                        //result.Item2.ShowErrorMessage("Error");
                        //ToDo genera evento 
                        //result.Item2.Error
                    }
                    else
                    {
                        var item = (T)dgv.DataGridView.Rows[row.Index].DataBoundItem;
                        item.Id = addResult.Id;

                        dgv.DataGridView.Rows[row.Index].Cells["Function"].Value = "Remove";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("" + ex.Source + " - " + ex.Message + " - " + ex.StackTrace);
                    this.Logger.LogError("ViewAdd: " + ex.Message + " - " + ex.StackTrace);
                }
            }).ConfigureAwait(false);
        }

        private async void ViewUpdate(object sender, T e)
        {
            TView dgv = (TView)sender;

            if (dgv.DataGridView.CurrentRow == null)
            {
                return;
            }

            if (dgv.DataGridView.CurrentRow.Index >= dgv.DataGridView.RowCount - 1)
            {
                return;
            }

            DataGridViewRow row = dgv.DataGridView.CurrentRow;

            await Task.Run(() =>
            {
                var addResult = this.CommandModel.Update(e.GetUpdateReq());
                ;

                if (addResult.Id.Equals(Guid.Empty))
                {
                    //result.Item2.DataGridView.CurrentRow.ErrorText = "Pippo";
                    //result.Item2.ShowErrorMessage("Error");
                    //ToDo genera evento 
                    //result.Item2.Error
                }
                else
                {
                    var item = (T)dgv.DataGridView.Rows[row.Index].DataBoundItem;
                    item.Id = addResult.Id;

                    dgv.DataGridView.Rows[row.Index].Cells["Function"].Value = "Remove";
                }
            }).ConfigureAwait(false);
        }

        private async void ViewCopy(object sender, T e)
        {
            TView dgv = (TView)sender;

            if (dgv.DataGridView.CurrentRow == null)
            {
                return;
            }

            if (dgv.DataGridView.CurrentRow.Index >= dgv.DataGridView.RowCount - 1)
            {
                return;
            }

            DataGridViewRow row = dgv.DataGridView.CurrentRow;

            await Task.Run(() =>
            {
                var copyResult = this.CommandModel.Copy(e.GetCopyReq());

                if (copyResult.Id.Equals(Guid.Empty))
                {

                }
                else
                {
                    var item = (T)dgv.DataGridView.Rows[row.Index].DataBoundItem;
                    item.Id = copyResult.Id;

                    dgv.DataGridView.Rows[row.Index].Cells["Function"].Value = "Remove";
                }
            }).ConfigureAwait(false);
        }

        private async void ViewModifyField(object sender, DataGridViewCellEventArgs e)
        {
            TView dgv = (TView)sender;
            if (dgv.DataGridView.Rows[e.RowIndex].Index >= dgv.DataGridView.RowCount - 1)
            {
                return;
            }

            DataGridViewRow row = dgv.DataGridView.Rows[e.RowIndex];
            DataGridViewCell cell = dgv.DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];

            var rowValue = (T)row.DataBoundItem;

            if (rowValue.Id.Equals(Guid.Empty))
            {
                return;
            }

            string columnName = dgv.DataGridView.Columns[cell.ColumnIndex].Name;

            string value = String.Empty;

            if (row.Cells[cell.ColumnIndex].ValueType == typeof(byte[]))
            {
                if (row.Cells[cell.ColumnIndex].Value != null)
                {
                    byte[] array = (byte[])row.Cells[cell.ColumnIndex].Value;
                    value = BitConverter.ToString(array);
                }
            }
            else
            {
                value = row.Cells[cell.ColumnIndex].Value.ToString();
            }

            Guid id = ((T)row.DataBoundItem).Id;

            await Task.Run(() =>
            {
                var modifyResult = this.CommandModel.ModifyField(new TModifyFieldReq
                {
                    Id = id, FieldName = columnName, Value = value
                });

                dgv.DataGridView.Rows[row.Index].Cells[columnName].Style.BackColor =
                    modifyResult.Result ? Color.Green : Color.DarkRed;
            }).ConfigureAwait(false);
        }
    }
}
