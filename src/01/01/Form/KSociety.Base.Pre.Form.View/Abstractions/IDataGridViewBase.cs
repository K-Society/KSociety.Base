// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

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