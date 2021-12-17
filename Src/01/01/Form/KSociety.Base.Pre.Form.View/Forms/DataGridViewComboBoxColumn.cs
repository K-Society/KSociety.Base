using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSociety.Base.Pre.Form.View.Forms;

public partial class DataGridViewComboBoxColumn : System.Windows.Forms.DataGridViewComboBoxColumn
{
    public DataGridViewComboBoxColumn()
    {
        InitializeComponent();
    }

    public DataGridViewComboBoxColumn(IContainer container)
    {
        container.Add(this);

        InitializeComponent();
    }
}