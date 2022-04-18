using System;
using System.Data;
using System.Windows.Forms;
using FastReport.Utils;
using FastReport.Data;
using FastReport.Editor;
using FastReport.Editor.Common;
using ActiveQueryBuilder.Core;
using System.Data.Common;
using FastReport.Editor.Syntax.Parsers;

namespace FastReport.Forms
{
    public partial class ActiveQBForm : Form
    {
        private DataConnectionBase FConnection;
        private SyntaxEdit FEditor;

        public ActiveQueryBuilder.View.WinForms.QueryBuilder QueryBuilder
        {
            get
            {
                return queryBuilder1;
            }
        }


        public DataConnectionBase Connection
        {
            get { return FConnection; }
            set
            {
                FConnection = value;
                switch (FConnection.GetType().Name)
                {
                    case "DB2DataConnection":
                        queryBuilder1.MetadataProvider = new DB2MetadataProvider();
                        queryBuilder1.SyntaxProvider = new DB2SyntaxProvider();
                        break;

                    case "MsAccessDataConnection":
                        queryBuilder1.MetadataProvider = new OLEDBMetadataProvider();
                        queryBuilder1.SyntaxProvider = new MSAccessSyntaxProvider();
                        break;

                    case "OleDbDataConnection":
                        queryBuilder1.MetadataProvider = new OLEDBMetadataProvider();
                        queryBuilder1.SyntaxProvider = new AutoSyntaxProvider();
                        break;

                    case "MsSqlDataConnection":
                        queryBuilder1.MetadataProvider = new MSSQLMetadataProvider();
                        queryBuilder1.SyntaxProvider = new MSSQLSyntaxProvider();
                        break;

                    case "OdbcDataConnection":
                        queryBuilder1.MetadataProvider = new ODBCMetadataProvider();
                        queryBuilder1.SyntaxProvider = new AutoSyntaxProvider();
                        break;

                    case "OracleDataConnection":
                        queryBuilder1.MetadataProvider = new OracleMetadataProvider();
                        queryBuilder1.SyntaxProvider = new OracleSyntaxProvider();
                        break;

                    case "OracleNativeDataConnection":
                        queryBuilder1.MetadataProvider = new OracleNativeMetadataProvider();
                        queryBuilder1.SyntaxProvider = new OracleSyntaxProvider();
                        break;

                    case "FirebirdDataConnection":
                        queryBuilder1.MetadataProvider = new FirebirdMetadataProvider();
                        queryBuilder1.SyntaxProvider = new FirebirdSyntaxProvider();
                        break;

                    case "MySqlDataConnection":
                        queryBuilder1.MetadataProvider = new MySQLMetadataProvider();
                        queryBuilder1.SyntaxProvider = new MySQLSyntaxProvider();
                        break;

                    case "InformixDataConnection":
                        queryBuilder1.MetadataProvider = new InformixMetadataProvider();
                        queryBuilder1.SyntaxProvider = new InformixSyntaxProvider();
                        break;

                    case "PostgresDataConnection":
                        queryBuilder1.MetadataProvider = new PostgreSQLMetadataProvider();
                        queryBuilder1.SyntaxProvider = new PostgreSQLSyntaxProvider();
                        break;

                    case "SqlCeDataConnection":
                        queryBuilder1.MetadataProvider = new MSSQLCEMetadataProvider();
                        queryBuilder1.SyntaxProvider = new MSSQLCESyntaxProvider();
                        break;

                    case "SQLiteDataConnection":
                        queryBuilder1.MetadataProvider = new SQLiteMetadataProvider();
                        queryBuilder1.SyntaxProvider = new SQLiteSyntaxProvider();
                        break;

                    case "SybaseDataConnection":
                        queryBuilder1.MetadataProvider = new SybaseMetadataProvider();
                        queryBuilder1.SyntaxProvider = new SybaseSyntaxProvider();
                        break;

                    case "VistaDBDataConnection":
                        queryBuilder1.MetadataProvider = new VistaDB5MetadataProvider();
                        queryBuilder1.SyntaxProvider = new VistaDBSyntaxProvider();
                        break;
                }

                if (queryBuilder1.MetadataProvider == null)
                    throw new Exception(FConnection.GetType().Name + " connection is not supported by Query Builder");

                queryBuilder1.MetadataProvider.Connection = FConnection.GetConnection();
                queryBuilder1.RefreshDatasourcesMetadata();
                queryBuilder1.InitializeDatabaseSchemaTree();
            }
        }

        public string SQL
        {
            get { return queryBuilder1.SQL; }
            set { queryBuilder1.SQL = value; }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void ActiveQBForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Config.SaveFormState(this);
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex == 1)
            {
                using (DbConnection con = FConnection.GetConnection())
                {
                    con.Open();
                    DataTable table = new DataTable();
                    string sql = SQL;
                    if (sql.Trim() != "")
                    {
                        try
                        {
                            using (DbDataAdapter adapter = FConnection.GetAdapter(SQL, con, new CommandParameterCollection(null)))
                            {
                                adapter.Fill(table);
                            }
                        }
                        catch (Exception ex)
                        {
                            using (ExceptionForm form = new ExceptionForm(ex))
                            {
                                form.ShowDialog();
                            }
                        }
                    }
                    dataGridView1.DataSource = table;
                }
            }
        }

        private void queryBuilder1_SQLUpdated(object sender, EventArgs e)
        {
            FEditor.Source.Text = SQL;
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            queryBuilder1.Focus();
        }

        public ActiveQBForm()
        {
            InitializeComponent();
            Config.RestoreFormState(this);
            toolStrip1.Renderer = Config.DesignerSettings.ToolStripRenderer;
            queryBuilder1.Language = Res.LocaleName;
            btnOK.Image = Res.GetImage(210);
            btnCancel.Image = Res.GetImage(212);

            FEditor = new SyntaxEdit();
            FEditor.Parent = splitContainer1.Panel2;
            FEditor.Dock = DockStyle.Fill;
            FEditor.BorderStyle = EditBorderStyle.FixedSingle;
            FEditor.Scroll.ScrollBars = RichTextBoxScrollBars.Both;
            FEditor.Scroll.Options = ScrollingOptions.SmoothScroll;
            FEditor.Gutter.Visible = false;

            FEditor.Source = new TextSource();
            FEditor.Lexer = new SqlParser();
            FEditor.Source.Lexer = FEditor.Lexer;
            FEditor.Source.Readonly = true;
        }

        private void queryView1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

