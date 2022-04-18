using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using FastReport.Data;
using FastReport.Utils;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif


namespace FastReport.Forms
{
    internal partial class DataViewForm : ScaledSupportingForm
    {
#region Fields
        
        private DataGridView grid;
        private DataSourceBase data;
        private int currentFirstRow;
        private int rowsOnPage;
        private int rowsCount;

#endregion Fields

#region Properties

        private int CurrentFirstRow
        {
            get { return currentFirstRow; }
            set
            {
                if (value >= 0 && value < RowsCount)
                {
                    currentFirstRow = value;
                }
            }
        }

        private int RowsOnPage
        {
            get { return rowsOnPage; }
        }

        private int RowsCount
        {
            get { return rowsCount; }
        }

#endregion Properties

#region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataViewForm"/> class.
        /// </summary>
        public DataViewForm(DataSourceBase data)
        {
            this.data = data;

            InitializeComponent();
            // create grid
            grid = new DataGridView();
            grid.Dock = DockStyle.Fill;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.BorderStyle = BorderStyle.None;
            grid.BackgroundColor = Color.White;
            grid.GridColor = Color.LightGray;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;
            grid.RowHeadersVisible = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            grid.DataError += new DataGridViewDataErrorEventHandler(grid_DataError);
            this.Controls.Add(grid);
            grid.BringToFront();
            Localize();
            Init();

            currentFirstRow = 0;
            if (data is TableDataSource)
            {
                rowsCount = (data as TableDataSource).Table.Rows.Count;
            }
            else
            {
                rowsCount = data.Rows.Count;
            }

            // calc number of rows on page
            CalcRowsOnPage();

            // fill grid with RowsOnPage rows starting from CurrentFirstRow
            FillGridWithNRows();
            UpdateButtons();

            Scale();
            ScaleBotButtons();

            this.SizeChanged += new System.EventHandler(this.DataViewForm_SizeChanged);
        }

#endregion Constructors

#region Private Methods

        private void Init()
        {
            Text = data.Alias;
            Icon = Res.GetIcon(222);
            Font = DrawUtils.DefaultFont;
            ShowInTaskbar = false;
            Location = new Point(200, 200);
            StartPosition = FormStartPosition.Manual;
            Config.RestoreFormState(this);
        }

        private void Done()
        {
            Config.SaveFormState(this);
        }

        private void Localize()
        {
            MyRes res = new MyRes("Preview");

            btnFirst.Text = "";
            btnPrior.Text = "";
            btnNext.Text = "";
            btnLast.Text = "";

            btnFirst.Image = Res.GetImage(185);
            btnPrior.Image = Res.GetImage(186);
            btnNext.Image = Res.GetImage(187);
            btnLast.Image = Res.GetImage(188);
        }

        private void CalcRowsOnPage()
        {
            rowsOnPage = ClientRectangle.Height / grid.RowTemplate.Height - 3;
            if (RowsOnPage >= RowsCount)
            {
                CurrentFirstRow = 0;
            }
        }

        private void ScaleBotButtons()
        {
            btnLast.Location = new Point(Width - DpiHelper.ConvertUnits(21) - btnLast.Width, stStatus.Top + (stStatus.Height / 2 - btnFirst.Height / 2));
            btnNext.Location = new Point(btnLast.Location.X - btnNext.Width - DpiHelper.ConvertUnits(7), btnLast.Location.Y);
            btnPrior.Location = new Point(btnNext.Location.X - btnPrior.Width - DpiHelper.ConvertUnits(7), btnLast.Location.Y);
            btnFirst.Location = new Point(btnPrior.Location.X - btnFirst.Width - DpiHelper.ConvertUnits(7), btnLast.Location.Y);
            lblRows.Location = new Point(lblRows.Location.X, stStatus.Top + (stStatus.Height / 2 - lblRows.Height / 2));
        }

        private string GetStatusString()
        {
            if (RowsOnPage > 0)
            {
                if (RowsOnPage == 1)
                {
                    return String.Format(Res.Get("Designer,ToolWindow,Dictionary,RowMofNRows"), CurrentFirstRow + 1, RowsCount);
                }
                else if (CurrentFirstRow == RowsCount - 1)
                {
                    return String.Format(Res.Get("Designer,ToolWindow,Dictionary,RowMofNRows"), CurrentFirstRow + 1, RowsCount);
                }
                else
                {
                    int lastRowOnPage = CurrentFirstRow + RowsOnPage;
                    if (lastRowOnPage > RowsCount)
                    {
                        lastRowOnPage = RowsCount;
                    }
                    return String.Format(Res.Get("Designer,ToolWindow,Dictionary,RowsKtoLofNRows"), CurrentFirstRow + 1, lastRowOnPage, RowsCount);
                }
            }
            return String.Format(Res.Get("Designer,ToolWindow,Dictionary,NRows"), RowsCount);
        }

        private void FillGridWithNRows()
        {
            if (data is TableDataSource)
            {
                DataTable fullTable = (data as TableDataSource).Table;
                DataTable table = new DataTable(fullTable.TableName);
                table = fullTable.Clone();
                for (int i = 0, j = CurrentFirstRow; i < RowsOnPage && j < RowsCount; i++, j++)
                {
                    table.ImportRow(fullTable.Rows[j]);
                }

                // this limitation in 655 columns is needed to avoid error with FillWeight sum more than 65535
                if (table.Columns.Count <= 655)
                {
                    grid.DataSource = table;
                }
                else
                {
                    grid.Columns.Clear();
                    grid.Rows.Clear();
                    for (int i = 0; i < 655; i++)
                    {
                        DataColumn col = table.Columns[i];
                        DataGridViewTextBoxColumn dc = new DataGridViewTextBoxColumn();
                        dc.Frozen = false;
                        dc.HeaderText = col.ColumnName;
                        grid.Columns.Add(dc);
                       
                    }
                    foreach (DataRow row in table.Rows)
                    {
                        grid.Rows.Add(row);
                    }
                }
            }
            else
            {
                for (int i = 0, j = CurrentFirstRow; i < RowsOnPage && j < RowsCount; i++, j++)
                {

                    if (data.Rows[j] is IList)
                    {

                        IList list = (data.Rows[j] as IList);

                        if (j == CurrentFirstRow)
                        {
                            for (int k = 0; k < list.Count; k++)
                            {
                                grid.Columns.Add("Column " + (k + 1).ToString(), "Column " + (k + 1).ToString());
                            }
                        }

                        grid.Rows.Add();
                        for (int k = 0; k < list.Count; k++)
                        {
                            grid.Rows[j].Cells[k].Value = list[k];
                        }
                    }
                    else
                    {
                        if (j == CurrentFirstRow)
                            grid.Columns.Add("Column 1", "Column 1");
                        grid.Rows.Add(data.Rows[j]);
                    }
                }
            }
        }

        private void UpdateButtons()
        {
            if (RowsCount <= RowsOnPage || RowsOnPage <= 0)
            {
                btnFirst.Enabled = false;
                btnPrior.Enabled = false;
                btnNext.Enabled = false;
                btnLast.Enabled = false;
            }
            else
            {
                if (CurrentFirstRow < RowsOnPage)
                {
                    btnFirst.Enabled = false;
                    btnPrior.Enabled = false;
                    btnNext.Enabled = true;
                    btnLast.Enabled = true;
                }
                else if (RowsOnPage > (RowsCount - CurrentFirstRow) || CurrentFirstRow == RowsCount - 1 || (CurrentFirstRow + RowsOnPage) == RowsCount)
                {
                    btnFirst.Enabled = true;
                    btnPrior.Enabled = true;
                    btnNext.Enabled = false;
                    btnLast.Enabled = false;
                }
                else
                {
                    btnFirst.Enabled = true;
                    btnPrior.Enabled = true;
                    btnNext.Enabled = true;
                    btnLast.Enabled = true;
                }
            }
            lblRows.Text = GetStatusString();
        }

#endregion Private Methods

#region Event Handlers

        private void btnFirst_Click(object sender, EventArgs e)
        {
            lblRows.Focus();
            CurrentFirstRow =  0;
            FillGridWithNRows();
            UpdateButtons();
        }

        private void btnPrior_Click(object sender, EventArgs e)
        {
            lblRows.Focus();
            CurrentFirstRow -= RowsOnPage;
            FillGridWithNRows();
            UpdateButtons();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            lblRows.Focus();
            CurrentFirstRow += RowsOnPage;
            FillGridWithNRows();
            UpdateButtons();
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            lblRows.Focus();
            if (RowsOnPage == 1)
            {
                CurrentFirstRow = RowsCount - 1;
            }
            else
            {
                CurrentFirstRow = RowsCount % RowsOnPage == 0 ? RowsCount - RowsOnPage : RowsCount - RowsCount % RowsOnPage;
            }
            FillGridWithNRows();
            UpdateButtons();
        }

        private void DataViewForm_SizeChanged(object sender, EventArgs e)
        {
            CalcRowsOnPage();
            FillGridWithNRows();
            UpdateButtons();
        }

        private void DataViewForm_Shown(object sender, EventArgs e)
        {
            int i = 0;
            while (i < grid.Columns.Count)
            {
                Column c = data.Columns.FindByName(grid.Columns[i].HeaderText);
                if (c != null)
                {
                    if (c.Enabled)
                        grid.Columns[i].HeaderText = c.Alias;
                    else
                    {
                        grid.Columns.RemoveAt(i);
                        i--;
                    }
                }
                i++;
            }
        }

        private void DataViewForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Done();
        }

        private void grid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

#endregion Event Handlers
    }
}
