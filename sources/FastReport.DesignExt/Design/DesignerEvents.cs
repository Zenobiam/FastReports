using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using FastReport.Data;

namespace FastReport.Design
{
  /// <summary>
  /// Provides a data for the designer ReportLoaded event.
  /// </summary>
  public class ReportLoadedEventArgs
  {
    /// <summary>
    /// The current report.
    /// </summary>
    public Report Report { get; }

    internal ReportLoadedEventArgs(Report report)
    {
            this.Report = report;
    }
  }

  /// <summary>
  /// Represents the method that will handle the designer ReportLoaded event.
  /// </summary>
  /// <param name="sender">The source of the event.</param>
  /// <param name="e">The event data.</param>
  public delegate void ReportLoadedEventHandler(object sender, ReportLoadedEventArgs e);


  /// <summary>
  /// Provides a data for the designer ObjectInserted event.
  /// </summary>
  public class ObjectInsertedEventArgs
  {
        /// <summary>
        /// Gets the inserted object.
        /// </summary>
        public Base Object { get; }

        /// <summary>
        /// Gets the source where the object is inserted from.
        /// </summary>
        public InsertFrom InsertSource { get; }

        internal ObjectInsertedEventArgs(Base obj, InsertFrom source)
        {
            Object = obj;
            InsertSource = source;
        }
  }

  /// <summary>
  /// Represents the method that will handle the designer ObjectInserted event.
  /// </summary>
  /// <param name="sender">The source of the event.</param>
  /// <param name="e">The event data.</param>
  public delegate void ObjectInsertedEventHandler(object sender, ObjectInsertedEventArgs e);


  /// <summary>
  /// Provides a data for the designer's custom dialog events.
  /// </summary>
  public class OpenSaveDialogEventArgs
  {

        /// <summary>
        /// Gets or sets a file name.
        /// </summary>
        /// <remarks>
        /// This property contains the location of a report. If you work with files (like the 
        /// standard "Open" and "Save" dialogs do), treat this property as a file name. 
        /// </remarks>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that the dialog was cancelled.
        /// </summary>
        /// <remarks>
        /// This property is used to tell the designer that the user was cancelled the dialog.
        /// </remarks>
        public bool Cancel { get; set; }

        /// <summary>
        /// Gets or sets the custom data that is shared across events.
        /// </summary>
        /// <remarks>
        /// You may set the Data in the OpenDialog event and use it later in the OpenReport event.
        /// </remarks>
        public object Data { get; set; }

        /// <summary>
        /// Gets a report designer.
        /// </summary>
        public Designer Designer { get; }

        internal bool IsPlugin { get; set; }

        internal OpenSaveDialogEventArgs(Designer designer)
        {
            this.Designer = designer;
            FileName = "";
        }
  }

  /// <summary>
  /// Represents the method that will handle the designer's custom dialogs event.
  /// </summary>
  /// <param name="sender">The source of the event.</param>
  /// <param name="e">The event data.</param>
  public delegate void OpenSaveDialogEventHandler(object sender, OpenSaveDialogEventArgs e);


  /// <summary>
  /// Provides a data for the designer's custom dialog events.
  /// </summary>
  public class OpenSaveReportEventArgs
  {

        /// <summary>
        /// Gets a report.
        /// </summary>
        /// <remarks>
        /// Use this report in the load/save operations.
        /// </remarks>
        public Report Report { get; }

        /// <summary>
        /// Gets a file name.
        /// </summary>
        /// <remarks>
        /// This property contains the location of a report that was selected by the user in the 
        /// open/save dialogs. If you work with files (like the standard "Open" and "Save" dialogs do), 
        /// treat this property as a file name. 
        /// </remarks>
        public string FileName { get; } = "";

        /// <summary>
        /// Gets the custom data that was set in the OpenDialog event.
        /// </summary>
        public object Data { get; }

        internal bool IsPlugin { get; }

        internal OpenSaveReportEventArgs(Report report, string fileName, object data, bool isPlugin)
        {
            this.Report = report;
            this.FileName = fileName;
            this.Data = data;
            this.IsPlugin = isPlugin;
        }
  }

  /// <summary>
  /// Represents the method that will handle the designer's custom dialogs event.
  /// </summary>
  /// <param name="sender">The source of the event.</param>
  /// <param name="e">The event data.</param>
  public delegate void OpenSaveReportEventHandler(object sender, OpenSaveReportEventArgs e);


  /// <summary>
  /// Provides data for the FilterConnectionTables event.
  /// </summary>
  public class FilterConnectionTablesEventArgs
  {

        /// <summary>
        /// Gets the Connection object.
        /// </summary>
        public DataConnectionBase Connection { get; }

        /// <summary>
        /// Gets the table name.
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// Gets or sets a value that indicates whether this table should be skipped.
        /// </summary>
        public bool Skip { get; set; }

        internal FilterConnectionTablesEventArgs(DataConnectionBase connection, string tableName)
        {
            this.Connection = connection;
            this.TableName = tableName;
        }
  }

  /// <summary>
  /// Represents the method that will handle the FilterConnectionTables event.
  /// </summary>
  /// <param name="sender">The source of the event.</param>
  /// <param name="e">The event data.</param>
  public delegate void FilterConnectionTablesEventHandler(object sender, FilterConnectionTablesEventArgs e);


  /// <summary>
  /// Provides data for the CustomQueryBuilder event.
  /// </summary>
  public class CustomQueryBuilderEventArgs
  {

        /// <summary>
        /// Gets the Connection object.
        /// </summary>
        public DataConnectionBase Connection { get; }

        /// <summary>
        /// Gets or sets the query text.
        /// </summary>
        public string SQL { get; set; }

        /// <summary>
        /// Gets or sets the query parameters.
        /// </summary>
        public CommandParameterCollection Parameters { get; set; }

        internal CustomQueryBuilderEventArgs(DataConnectionBase connection, string sql)
        {
            this.Connection = connection;
            SQL = sql;
        }

        internal CustomQueryBuilderEventArgs(DataConnectionBase connection, string sql, CommandParameterCollection parameters) : this(connection, sql)
        {
                this.Parameters = parameters;
        }
  }

  /// <summary>
  /// Represents the method that will handle the CustomQueryBuilder event.
  /// </summary>
  /// <param name="sender">The source of the event.</param>
  /// <param name="e">The event data.</param>
  public delegate void CustomQueryBuilderEventHandler(object sender, CustomQueryBuilderEventArgs e);

}
