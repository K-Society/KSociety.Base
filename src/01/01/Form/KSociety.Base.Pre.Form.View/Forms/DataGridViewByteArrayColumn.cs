// Copyright � K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

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
