using System;
using KSociety.Base.InfraSub.Shared.Interface;
using System.Windows.Forms;

namespace KSociety.Base.Pre.Form.View.Abstractions
{
    public interface IDataGridView<T> : IDataGridViewBase where T : IObject
    {
        event EventHandler<T> Remove;

        event EventHandler<T> Add;

        event EventHandler<T> UpdateEntity;

        event EventHandler<T> Copy;

        //event EventHandler<T> ModifyField;

        event EventHandler<DataGridViewCellEventArgs> ModifyField;
    }
}