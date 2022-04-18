using FastReport.Forms;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace FastReport.Data
{
    partial class TableDataSource : IHasEditor
    {
        #region Public Methods

        /// <inheritdoc/>
        public bool InvokeEditor()
        {
            using (QueryWizardForm form = new QueryWizardForm(this))
            {
                return form.ShowDialog() == DialogResult.OK;
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void TryToLoadData()
        {
            // we are in the VS design mode, so dataset is empty. To fill it with real data, try to
            // find the table adapter, instantiate it and invoke its Fill method.
            if (Report != null && (Report.IsVSDesignMode || Report.AutoFillDataSet) &&
              Table != null && Table.Rows.Count == 0)
            {
                string tableAdapterName = Table.GetType().Name;
                if (tableAdapterName.EndsWith("DataTable"))
                    tableAdapterName = tableAdapterName.Substring(0, tableAdapterName.Length - 9);
                tableAdapterName += "TableAdapter";

                Assembly dataAssembly = Table.GetType().Assembly;
                foreach (Type type in dataAssembly.GetTypes())
                {
                    if (type.Name == tableAdapterName)
                    {
                        object tableAdapter = Activator.CreateInstance(type);
                        try
                        {
                            MethodInfo fillMethod = tableAdapter.GetType().GetMethod("Fill");
                            if (fillMethod != null)
                                fillMethod.Invoke(tableAdapter, new object[] { Table });
                        }
                        catch
                        {
                        }
                        finally
                        {
                            if (tableAdapter is IDisposable)
                                (tableAdapter as IDisposable).Dispose();
                        }
                        break;
                    }
                }
            }
        }

        #endregion Private Methods
    }
}