using KSociety.Base.InfraSub.Shared.Interface;
using KSociety.Base.Pre.Model.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KSociety.Base.Pre.Form.View.Forms;

public partial class DataGridViewBase<T, TList> where T : IObject where TList : InfraSub.Shared.Interface.IList<T>
{
    protected readonly Dictionary<string, BindingSource> BindingSourcesComboBox = new Dictionary<string, BindingSource>();
    public DataGridView DataGridView { get; private set; }
    public BindingSource BindingSource { get; private set; }

    public TList ListView
    {
        set
        {
            try
            {
                if (value == null)
                {
                    //ToDo Add Log
                    return;
                }
                BindingSource =
                    new BindingSource(
                        new SortableBindingList<T>(value.List)
                            { SynchronizationContext = SynchronizationContext.Current }, null);

                DataSource = BindingSource;
                BindingSource.AllowNew = true;
                foreach (var propertyInfo in typeof(TList).GetProperties())
                {
                    if (propertyInfo.Name.Equals("List")) continue;
                    dynamic objectValue = propertyInfo.GetValue(value, null);
                    if (!BindingSourcesComboBox.ContainsKey(propertyInfo.Name)) continue;
                    BindingSourcesComboBox[propertyInfo.Name] = new BindingSource(objectValue.List, null);

                    (Columns[propertyInfo.Name] as DataGridViewComboBoxColumn).DataSource =
                        BindingSourcesComboBox[propertyInfo.Name];

                }

                BindingSource.BindingComplete += BindingSourceBindingComplete;
            }
            catch (Exception ex)
            {
                string[] lines = { ex.Message, ex.StackTrace };
                System.IO.File.WriteAllLines(@"C:\JOB\ListView.txt", lines);
            }
        }
    }

    public DataGridViewBase()
    {
        InitializeComponent();
    }

    public DataGridViewBase(IContainer container)
    {
        container.Add(this);

        InitializeComponent();
    }

    public event EventHandler Error; // { get; set; }

    protected void CreateColumns()
    {
        foreach (var propertyInfo in typeof(T).GetProperties())
        {
            if (!IsBrowsable(propertyInfo)) continue;

            DataGridViewColumn dataGridViewColumn;

            if (BindingSourcesComboBox.ContainsKey(propertyInfo.Name))
            {
                dataGridViewColumn = new DataGridViewComboBoxColumn
                {
                    ValueMember = "Key",
                    DisplayMember = "Value"

                    //DisplayIndex = 0
                };
            }
            else
            {
                if (propertyInfo.PropertyType == typeof(bool))
                {
                    dataGridViewColumn = new DataGridViewCheckBoxColumn();
                    //dataGridViewColumn.ValueType = propertyInfo.PropertyType;
                }
                else if (propertyInfo.PropertyType == typeof(byte[]))
                {
                    dataGridViewColumn = new DataGridViewByteArrayColumn();
                    //dataGridViewColumn.ValueType = typeof(string); //propertyInfo.PropertyType;
                }
                else
                {
                    dataGridViewColumn = new DataGridViewTextBoxColumn();
                    //dataGridViewColumn.ValueType = propertyInfo.PropertyType;
                }


                dataGridViewColumn.ValueType = propertyInfo.PropertyType;
            }

            dataGridViewColumn.Name = propertyInfo.Name;
            dataGridViewColumn.DataPropertyName = propertyInfo.Name;

            Columns.Add(dataGridViewColumn);
        }
    }

    //https://docs.microsoft.com/it-it/dotnet/framework/winforms/multiple-controls-bound-to-data-source-synchronized
    private static void BindingSourceBindingComplete(object sender, BindingCompleteEventArgs e)
    {
        // Check if the data source has been updated, and that no error has occured.
        if (e.BindingCompleteContext ==
            BindingCompleteContext.DataSourceUpdate && e.Exception == null)

            // If not, end the current edit.
            e.Binding.BindingManagerBase.EndCurrentEdit();
    }

    private static bool IsBrowsable(PropertyInfo propertyInfo)
    {
        var attributes = propertyInfo.GetCustomAttributes(false);

        if (attributes.Any())
        {
            foreach (var attribute in attributes)
            {
                if (attribute is BrowsableAttribute browsableAttribute)
                {
                    return browsableAttribute.Browsable;
                }
            }
            return true;
        }
        return true;
    }

    //No async!
    private void StdDataGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
    {
        // get style to use for row header cells
        DataGridViewCellStyle style = RowHeadersDefaultCellStyle;
        // get pen (needed to get text brush)
        Pen pen = new Pen(style.ForeColor);
        // get row rectangle and adjust to width of row header
        RectangleF rec = e.RowBounds;
        rec.Width = RowHeadersWidth;
        // create formating object to center row number
        StringFormat format = StringFormat.GenericTypographic;
        format.Alignment = StringAlignment.Center;
        format.LineAlignment = StringAlignment.Center;
        // draw the row number in row header
        e.Graphics.DrawString((e.RowIndex + 1).ToString(), style.Font, pen.Brush, rec, format);

        //e.RowPostPaintApp
    }

    private async void StdDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
    {
        await Task.Factory.StartNew(() =>
        {

            //MessageBox.Show(e.Context + @" - " + e.Exception.StackTrace);
            MessageBox.Show(e.Context + @" - " + e.Exception.Message + @" - " + e.Exception.StackTrace);
        }).ConfigureAwait(false);
    }

    public event EventHandler LoadData;
    public event EventHandler ErrorMessage;

    public void ShowErrorMessage(string errorMessage)
    {
        MessageBox.Show(errorMessage);
    }

    //public void ShowProgress(int progressPercentage)
    //{
    //    //Make it thread safe.
    //    //progressBarGridView.Show();
    //    if (progressBarGridView.InvokeRequired)
    //        progressBarGridView.Invoke(new MethodInvoker(delegate { progressBarGridView.Value = progressPercentage; }));
    //    else
    //        progressBarGridView.Value = progressPercentage;
    //}

    public void LoadDataGrid()
    {
        LoadData?.Invoke(new object(), new EventArgs());
    }

    private void StdDataGridViewBase_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
        //try
        //{
        DataGridView senderGrid = (DataGridView)sender;

        if (senderGrid.Columns[e.ColumnIndex] is DataGridViewByteArrayColumn /*|| e.RowIndex >= 0*/)
        {
            if (e.Value != null)
            {
                byte[] array = (byte[])e.Value;
                e.Value = BitConverter.ToString(array);
                e.FormattingApplied = true;
            }
            else
                e.FormattingApplied = false;
        }
        //}
        //catch (Exception ex)
        //{

        //}
    }

    private void StdDataGridViewBase_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
    {
        DataGridView senderGrid = (DataGridView)sender;

        if (senderGrid.Columns[e.ColumnIndex] is DataGridViewByteArrayColumn)
        {
            if (e.Value != null)
            {
                string[] splitResult = e.Value.ToString()?.Split('-');

                byte[] byteArray = new byte[splitResult.Length];

                for (int i = 0; i < splitResult.Length; i++)
                {
                    byteArray[i] = Convert.ToByte(splitResult[i], 16);
                }

                e.Value = byteArray;

                e.ParsingApplied = true;
            }
            else
                e.ParsingApplied = false;
        }
    }
}