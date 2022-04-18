

using System.Data.Common;

namespace FastReport.Data
{
    public class SharePointConnectionStringBuilder : DbConnectionStringBuilder
    {
        #region Public Properties

        public string Url
        {
            get
            {
                object result;
                if (TryGetValue("Url", out result))
                    return result.ToString();
                return "";
            }
            set { this["Url"] = value; }
        }

        public string Password
        {
            get
            {
                object result;
                if (TryGetValue("Password", out result))
                    return result.ToString();
                return "";
            }
            set { this["Password"] = value; }
        }

        public string UserName
        {
            get
            {
                object result;
                if (TryGetValue("UserName", out result))
                    return result.ToString();
                return "";
            }
            set { this["UserName"] = value; }
        }

        public bool Office365
        {
            get
            {
                object result;
                if (TryGetValue("Office365", out result))
                    return bool.Parse(result.ToString());
                return false;
            }
            set { this["Office365"] = value.ToString(); }
        }
        #endregion

        public SharePointConnectionStringBuilder(string connectionString)
        {
            this.ConnectionString = connectionString;
        }
    }
}
