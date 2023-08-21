namespace KSociety.Base.Pre.Form.View.Abstractions
{
    using System;
    using System.Windows.Forms;

    public interface IDataGridViewBase
    {
        DataGridView DataGridView { get; }
        BindingSource BindingSource { get; }

        void ShowErrorMessage(string errorMessage);

        //void ShowProgress(int progressPercentage);

        event EventHandler LoadData;

        //event EventHandler ErrorMessage;

        //event EventHandler Error;

        //ProgressBar ProgressBarLoadDataProperty { get; }

        //event EventHandler DefaultValuesNeeded;

        event EventHandler DataSourceChanged;
    }
}