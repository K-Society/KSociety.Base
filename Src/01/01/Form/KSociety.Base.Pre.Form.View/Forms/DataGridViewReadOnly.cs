using KSociety.Base.InfraSub.Shared.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSociety.Base.Pre.Form.View.Forms;

public partial class DataGridViewReadOnly<T, TList> where T : IObject where TList : InfraSub.Shared.Interface.IList<T>
{
    public DataGridViewReadOnly()
    {
        InitializeComponent();
        Initialize();
    }

    public DataGridViewReadOnly(IContainer container)
    {
        container.Add(this);

        InitializeComponent();
        Initialize();
    }

    private void Initialize()
    {
        AutoGenerateColumns = false;
        AllowUserToAddRows = false;
        AllowUserToDeleteRows = false;
        AllowDrop = false;

        foreach (var propertyInfo in typeof(TList).GetProperties())
        {
            if (!propertyInfo.Name.Equals("List"))
            {
                BindingSourcesComboBox.Add(propertyInfo.Name, null);
            }
        }
        //AutoGenerateColumns = true;
        //AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;



        //DataGridViewColumn col = new StdDataGridViewFunctionColumn();
        //Columns.Add(col);

        CreateColumns();
    }
}