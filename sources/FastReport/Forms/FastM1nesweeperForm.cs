using FastReport.Utils;
using System;

namespace FastReport.Forms
{
    internal partial class FastM1nesweeperForm : BaseDialogForm
    {
        #region Private Fields

        private bool lockUpdate;
        private DifPreset[] presets;

        #endregion Private Fields

        #region Public Properties

        public int Bombs
        {
            get
            {
                return (int)tbBombs.Value;
            }
        }

        public int Columns
        {
            get
            {
                return (int)tbColumns.Value;
            }
        }

        public int Rows
        {
            get
            {
                return (int)tbRows.Value;
            }
        }

        #endregion Public Properties

        #region Public Constructors

        public FastM1nesweeperForm()
        {
            InitializeComponent();
            Localize();
            Scale();
        }

        #endregion Public Constructors

        #region Public Methods

        public override void Localize()
        {
            base.Localize();
            MyRes res = new MyRes("Forms,FastM1nesweeper");
            Text = res.Get("");

            lWelcome.Text = res.Get("Welcome");
            gbSettings.Text = res.Get("Settings");
            lDifficulty.Text = res.Get("Difficulty");

            cbDifficulty.Items.Add(res.Get("Custom"));
            cbDifficulty.Items.Add(res.Get("Easy"));
            cbDifficulty.Items.Add(res.Get("Medium"));
            cbDifficulty.Items.Add(res.Get("Hard"));
            cbDifficulty.Items.Add(res.Get("Master"));
            for (int i = 1; i <= 13; i++)
            {
                cbDifficulty.Items.Add("T" + i.ToString());
            }

            presets = new DifPreset[5 + 13];

            presets[0] = new DifPreset(0, 0, 0);
            presets[1] = new DifPreset(10, 8, 8);
            presets[2] = new DifPreset(20, 12, 12);
            presets[3] = new DifPreset(40, 16, 16);
            presets[4] = new DifPreset(80, 20, 20);
            for (int i = 1; i <= 13; i++)
            {
                presets[4 + i] = new DifPreset(80 + i * 10, 25, 25);
            }

            lBombs.Text = res.Get("Bombs");
            lColumns.Text = res.Get("Columns");
            lRows.Text = res.Get("Rows");
            cbDifficulty.SelectedIndex = 2;
        }

        #endregion Public Methods

        #region Private Methods

        private void cbDifficulty_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lockUpdate)
                return;
            try
            {
                lockUpdate = true;

                if (cbDifficulty.SelectedIndex > 0)
                {
                    int i = cbDifficulty.SelectedIndex;
                    tbBombs.Value = presets[i].bombs;
                    tbColumns.Value = presets[i].columns;
                    tbRows.Value = presets[i].rows;
                }
            }
            finally
            {
                lockUpdate = false;
            }
        }

        private void Settings_ValueChanged(object sender, EventArgs e)
        {
            if (lockUpdate)
                return;
            try
            {
                lockUpdate = true;

                for (int i = 0; i < presets.Length; i++)
                {
                    if (presets[i].bombs == tbBombs.Value && presets[i].columns == tbColumns.Value && presets[i].rows == tbRows.Value)
                    {
                        cbDifficulty.SelectedIndex = i;
                        return;
                    }
                }

                cbDifficulty.SelectedIndex = 0;
            }
            finally
            {
                lockUpdate = false;
            }
        }

        #endregion Private Methods

        #region Private Structs

        private struct DifPreset
        {
            #region Public Fields

            public int bombs;
            public int columns;
            public int rows;

            #endregion Public Fields

            #region Public Constructors

            public DifPreset(int bombs, int rows, int columns)
            {
                this.bombs = bombs;
                this.rows = rows;
                this.columns = columns;
            }

            #endregion Public Constructors
        }

        #endregion Private Structs
    }
}