// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Pre.Web.Shared
{
    using Blazorise.DataGrid;
    using KSociety.Base.InfraSub.Shared.Interface;
    using Srv.Agent.List.GridView;
    using Microsoft.AspNetCore.Components;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    public partial class DataGridBase<T, TList, TGridView>
        where T : IObject
        where TList : InfraSub.Shared.Interface.IList<T>
        where TGridView : IAgentQueryModel<T, TList>
    {
        [Parameter] public int CurrentPageNumber { get; set; } = 1;

        [Parameter] public List<T> DataItems { get; set; }

        [Parameter] public List<ColumnDefinition> Columns { get; set; }

        //[Parameter]
        //public PagingConfig Paging { get; set; }

        [Parameter] public RenderFragment CustomPager { get; set; }

        //[Parameter]
        //public TList QueryListGridView { get; set; }

        [Parameter] public TGridView QueryListGridView { get; set; }

        public Dictionary<string, string> SelectColumns { get; set; } = new Dictionary<string, string>();

        protected override void OnInitialized()
        {
            //_queryListGridView = await QueryListGridView.LoadAllRecordsAsync();
            this.CreateSelectColumns();
            this.CreateColumns();
            this.DataItems = this.QueryListGridView.LoadAllRecords().List;

        }

        //public void GoToPreviousPage()
        //{
        //    CurrentPageNumber = Paging.PreviousPageNumber(CurrentPageNumber);
        //}

        //public void GoToNextPage()
        //{
        //    CurrentPageNumber = Paging.NextPageNumber(CurrentPageNumber, QueryListGridView.List.Count);
        //}

        //private void OnModifyItem(ModifyItemEventArgs args)
        //{
        //    ;
        //    //Value = (string)args.Value;
        //    //await ValueChanged.InvokeAsync(Value);
        //}

        public async Task OnSaveClicked()
        {
            ;
        }

        protected async Task OnRowRemovedAsync(T item)
        {
            //TAgentCommand.Remove()
            ;
        }

        protected void CreateSelectColumns()
        {
            foreach (var propertyInfo in typeof(TList).GetProperties())
            {
                if (propertyInfo.Name.Equals("List"))
                {
                    continue;
                }

                this.SelectColumns.Add(propertyInfo.Name, propertyInfo.Name);
            }
        }

        protected void CreateColumns()
        {
            this.Columns ??= new List<ColumnDefinition>();
            try
            {
                foreach (var propertyInfo in typeof(T).GetProperties())
                {
                    if (!IsBrowsable(propertyInfo))
                    {
                        continue;
                    }

                    //DataGridViewColumn dataGridViewColumn;
                    ColumnDefinition dataGridViewColumn = new ColumnDefinition();

                    //if (BindingSourcesComboBox.ContainsKey(propertyInfo.Name))
                    //{
                    //    dataGridViewColumn = new StdDataGridViewComboBoxColumn
                    //    {
                    //        ValueMember = "Key",
                    //        DisplayMember = "Value"

                    //        //DisplayIndex = 0
                    //    };
                    //}
                    //else
                    //{

                    if (this.SelectColumns.ContainsKey(propertyInfo.Name))
                    {
                        dataGridViewColumn.ColumnType = DataGridColumnType.Text;
                        dataGridViewColumn.ColumnType = DataGridColumnType.Select;
                        //SelectColumns[propertyInfo.Name] = typeof(TList).GetProperty(propertyInfo.Name).GetValue(null);
                    }
                    else
                    {

                        if (propertyInfo.PropertyType == typeof(bool))
                        {
                            //dataGridViewColumn = new DataGridViewCheckBoxColumn();
                            ////dataGridViewColumn.ValueType = propertyInfo.PropertyType;
                            dataGridViewColumn.DataType = DataType.Boolean;
                            dataGridViewColumn.ColumnType = DataGridColumnType.Check;
                        }
                        else if (propertyInfo.PropertyType == typeof(byte[]))
                        {
                            //dataGridViewColumn = new StdDataGridViewByteArrayColumn();
                            ////dataGridViewColumn.ValueType = typeof(string); //propertyInfo.PropertyType;
                            dataGridViewColumn.DataType = DataType.String;
                            dataGridViewColumn.ColumnType = DataGridColumnType.Text;
                        }
                        else if (propertyInfo.PropertyType == typeof(int))
                        {
                            //dataGridViewColumn = new DataGridViewTextBoxColumn();
                            ////dataGridViewColumn.ValueType = propertyInfo.PropertyType;
                            dataGridViewColumn.DataType = DataType.Number; //propertyInfo.PropertyType.ToString();
                            dataGridViewColumn.ColumnType = DataGridColumnType.Numeric;
                        }
                        else
                        {
                            dataGridViewColumn.DataType = DataType.String;
                            dataGridViewColumn.ColumnType = DataGridColumnType.Text;
                        }
                    }


                    //dataGridViewColumn.ValueType = propertyInfo.PropertyType;
                    //propertyInfo.PropertyType;
                    //}

                    //dataGridViewColumn.Name = propertyInfo.Name;
                    //dataGridViewColumn.DataPropertyName = propertyInfo.Name;

                    dataGridViewColumn.Caption = propertyInfo.Name;
                    dataGridViewColumn.DataField = propertyInfo.Name;
                    //dataGridViewColumn.DataPropertyName = propertyInfo.Name;


                    this.Columns.Add(dataGridViewColumn);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " - " + ex.StackTrace);
            }
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
    }
}
