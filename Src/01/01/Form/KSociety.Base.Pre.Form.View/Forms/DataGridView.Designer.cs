
using KSociety.Base.InfraSub.Shared.Interface;
using KSociety.Base.Pre.Form.View.Abstractions;

namespace KSociety.Base.Pre.Form.View.Forms
{
    partial class DataGridView<T, TList> : DataGridViewBase<T, TList>, IDataGridView<T>, IListView<T, TList> where T : IObject where TList : IList<T>
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        #endregion
    }
}
