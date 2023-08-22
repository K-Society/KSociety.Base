// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Pre.Form.View.Abstractions
{
    using System;
    using InfraSub.Shared.Interface;
    using System.Windows.Forms;

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
