using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

namespace FastReport.Data
{
  /// <summary>
  /// Represents the SqlCeDataConnection connection string builder.
  /// </summary>
  /// <remarks>
  /// Use this class to parse connection string returned by the <b>SqlCeDataConnection</b> class.
  /// </remarks>
  public class SqlCeConnectionStringBuilder : DbConnectionStringBuilder
  {
    /// <summary>
    /// Gets or sets the data source name.
    /// </summary>
    public string DataSource
    {
      get
      {
        object dataSource;
        if (TryGetValue("DataSource", out dataSource))
          return (string)dataSource;
        return "";
      }
      set
      {
        base["DataSource"] = value;
      }
    }

    /// <summary>
    /// Gets or sets the database password.
    /// </summary>
    public string Password
    {
      get
      {
        object password;
        if (TryGetValue("Password", out password))
          return (string)password;
        return "";
      }
      set
      {
        base["Password"] = value;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlCeConnectionStringBuilder"/> class with default settings.
    /// </summary>
    public SqlCeConnectionStringBuilder()
    {
      ConnectionString = "";
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlCeConnectionStringBuilder"/> class with 
    /// specified connection string.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    public SqlCeConnectionStringBuilder(string connectionString) : base()
    {
      ConnectionString = connectionString;
    }
  }
}
