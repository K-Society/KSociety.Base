using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KSociety.Base.Pre.Form.View.Forms
{
    public partial class DataGridViewByteArrayColumn : DataGridViewTextBoxColumn
    {
        public DataGridViewByteArrayColumn()
        {
            InitializeComponent();
            Initialize();
        }

        public DataGridViewByteArrayColumn(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            CellTemplate = new DataGridViewByteArrayCell();
        }
    }
}
