// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Pre.Form.View.Forms
{
    using System.ComponentModel;
    using System.Windows.Forms;

    public partial class DataGridViewFunctionColumn : DataGridViewButtonColumn
    {
        public DataGridViewFunctionColumn()
        {
            this.InitializeComponent();
            this.Initialize();
        }

        public DataGridViewFunctionColumn(IContainer container)
        {
            container.Add(this);

            this.InitializeComponent();
            this.Initialize();
        }

        private void Initialize()
        {
            this.DataPropertyName = "Function";
            this.Name = "Function";
            //Text = "Remove";
            this.HeaderText = @"Function";
            this.UseColumnTextForButtonValue = false;

            this.Width = 60;

            this.CellTemplate = new DataGridViewFunctionCell();
        }
    }
}
