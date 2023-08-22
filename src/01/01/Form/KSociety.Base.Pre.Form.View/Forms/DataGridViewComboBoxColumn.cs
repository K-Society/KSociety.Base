// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Pre.Form.View.Forms
{
    using System.ComponentModel;

    public partial class DataGridViewComboBoxColumn : System.Windows.Forms.DataGridViewComboBoxColumn
    {
        public DataGridViewComboBoxColumn()
        {
            this.InitializeComponent();
        }

        public DataGridViewComboBoxColumn(IContainer container)
        {
            container.Add(this);

            this.InitializeComponent();
        }
    }
}
