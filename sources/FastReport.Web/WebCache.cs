using System;
using System.Configuration;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.Caching;

namespace FastReport.Web
{      
    /// <summary>
    /// 
    /// </summary>
    public class WebReportCache 
    {
        internal static string StoragePrefix = FastReport.Utils.Config.Version + ".";
        internal static string touchFilename = StoragePrefix + "touch";
        internal static string maskStorage = StoragePrefix + "*";

        private bool webFarmMode = false;
        private string fileStoragePath;
        private int fileStorageTimeout;
        private int storageCleanup;
        private int cacheDelay = 15;
        private CacheItemPriority priority = CacheItemPriority.Default;

        private readonly object locker = new object();

        /// <summary>
        /// 
        /// </summary>
        public bool WebFarmMode
        {
            get { return webFarmMode; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Delay
        {
            get { return cacheDelay; }
            set { cacheDelay = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public CacheItemPriority Priority
        {
            get { return priority; }
            set { priority = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Obj"></param>
        public void PutObject(string Name, Object Obj)
        {
            if (Obj != null)
            {
                lock (locker)
                {
                    if (WebFarmMode)
                    {
                        if (Obj is WebReport)
                        {
                            WebReport webReport = Obj as WebReport;
                            using(Stream propStream = webReport.GetPropData())
                                PutFileStorage(Name, propStream);

                            using(Stream repStream = webReport.GetReportData())
                                PutFileStorage(Name + "report", repStream);


                            using(Stream prepStream = webReport.GetPreparedData())
                                PutFileStorage(Name + "prepared", prepStream);

                        }
                        else
                            PutFileStorage(Name, Obj);

                        CleanFileStorage();
                    }
                    else
                        if (!String.IsNullOrEmpty(Name) && Obj != null)
#if DOTNET_4
                        HttpRuntime.Cache.Insert(
                                Name, 
                                Obj, 
                                null,
                                Cache.NoAbsoluteExpiration, 
                                new TimeSpan(0, cacheDelay, 0),
                                UpdateCallback);
#else
                    HttpRuntime.Cache.Add(
                                Name, 
                                Obj, 
                                null,
                                Cache.NoAbsoluteExpiration, 
                                new TimeSpan(0, cacheDelay, 0),
                                priority, 
                                RemovedCallback);
#endif
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public object GetObject(string Name)
        {
            return GetObject(Name, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public object GetObject(string Name, Object obj)
        {            
            lock (locker)
            {
                if (WebFarmMode)
                {
                    if ((obj != null) && (obj is WebReport))
                    {
                        WebReport webReport = obj as WebReport;
                        
                        using(Stream propStream = GetFileStorage(Name) as Stream)
                            webReport.SetPropData(propStream);

                        using (Stream reportStream = GetFileStorage(Name + "report") as Stream)
                            webReport.SetReportData(reportStream);

                        using (Stream preparedStream = GetFileStorage(Name + "prepared") as Stream)
                            webReport.SetPreparedData(preparedStream);
                    }
                    else
                        obj = GetFileStorage(Name);
                }
                else
                    if (!String.IsNullOrEmpty(Name))
                    {
                        Object cached_obj = HttpRuntime.Cache[Name];
                        if (cached_obj != null)
                            obj = cached_obj;
                    }
            }
            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        public void Remove(string Name)
        {
            lock (locker)
            {
                if (!String.IsNullOrEmpty(Name))
                {
                    if (WebFarmMode)
                        DeleteFileStorage(Name);
                    else
                        HttpRuntime.Cache.Remove(Name);
                }
            }
        }

        private void DeleteFileStorage(string Name)
        {
            string fileName = String.Concat(fileStoragePath, StoragePrefix, Name);
            if (File.Exists(fileName))
            {
                try
                {
                    File.Delete(fileName);
                }
                catch
                {
                    // nothing
                }
            }
        }

        private string GetStoragePath(HttpContext context)
        {
            string path = ConfigurationManager.AppSettings["FastReportStoragePath"];
            if (!String.IsNullOrEmpty(path))
            {
                path += Path.DirectorySeparatorChar;
                if (!(path.StartsWith(@"\\") || path.StartsWith(@"//")) && (context != null))
                    path = context.Request.MapPath(path);
            }
            return path;
        }

        private int GetStorageTimeout()
        {
            string valueS = ConfigurationManager.AppSettings["FastReportStorageTimeout"];
            int value = 15;
            if (!String.IsNullOrEmpty(valueS))
                value = Convert.ToInt16(valueS);
            return value;
        }

        private int GetStorageCleanup()
        {
            string valueS = ConfigurationManager.AppSettings["FastReportStorageCleanup"];
            int value = 1;
            if (!String.IsNullOrEmpty(valueS))
                value = Convert.ToInt16(valueS);
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CleanFileStorage()
        {
            if (webFarmMode)
            {
                string touch = Path.Combine(fileStoragePath, touchFilename);

                if (!String.IsNullOrEmpty(fileStoragePath) && Directory.Exists(fileStoragePath))
                {
                    if (!File.Exists(touch))
                    {
                        using (FileStream file = File.Create(touch)) { };
                    }
                    else
                    {
                        DateTime created = File.GetLastWriteTime(touch);
                        if (DateTime.Now > created.AddMinutes(storageCleanup))
                        {
                            File.SetLastWriteTime(touch, DateTime.Now);

                            string[] dir = Directory.GetFiles(fileStoragePath, maskStorage);
                            foreach (string file in dir)
                            {
                                try
                                {
                                    if (DateTime.Now > File.GetLastWriteTime(file).AddMinutes(fileStorageTimeout))
                                        File.Delete(file);
                                }
                                catch
                                {
                                    //nothing
                                }
                            }
                        }
                    }
                }
                return Directory.GetFiles(fileStoragePath, maskStorage).Length;
            }
            else 
                return 0;
        }

        private object GetFileStorage(string name)
        {
            string fileName = String.Concat(fileStoragePath, StoragePrefix, name);
            object obj = null;
            if (File.Exists(fileName))
            {
                try
                {
                    File.SetLastWriteTime(fileName, DateTime.Now);
                }
                catch { };
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    obj = bf.Deserialize(fs);
                }
            }            
            return obj;
        }

        private void PutFileStorage(string name, object value)
        {
            string fileName = String.Concat(fileStoragePath, StoragePrefix, name);
            
            if (File.Exists(fileName))
            {
                if (value is byte[])
                {
                    try
                    {
                        File.SetLastWriteTime(fileName, DateTime.Now);
                    }
                    catch
                    {
                        // nothing to do
                    }
                }
                else
                    try
                    {
                        File.Delete(fileName);
                    }
                    catch
                    {
                        // nothing to do
                    }
            }

            if (!File.Exists(fileName))
            {
                using (FileStream fs = new FileStream(fileName, FileMode.CreateNew))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, value);
                }
            }

        }
#if DOTNET_4
        private void UpdateCallback(string key, 
            CacheItemUpdateReason reason,
            out Object expensiveObject,
            out CacheDependency dependency,
            out DateTime absoluteExpiration,
            out TimeSpan slidingExpiration)
        {
            Object v = HttpRuntime.Cache[key];
            dependency = null;

            if (v!= null && v is WebReport)
            {
                WebReport webReport = v as WebReport;

                for (int i = 0; i < webReport.Tabs.Count; i++)
                {
                    if (webReport.Tabs[i].Report.PreparedPages != null)
                        webReport.Tabs[i].Report.PreparedPages.Clear();
                    webReport.Tabs[i].Properties.ReportDone = false;
                }
                expensiveObject = v;
                absoluteExpiration = Cache.NoAbsoluteExpiration;
                slidingExpiration = new TimeSpan(0, cacheDelay, 0);
            }
            else
            {
                expensiveObject = null;
                absoluteExpiration = DateTime.Now;
                slidingExpiration = new TimeSpan(0, 0, 0);
            }
        }
#else
        private void RemovedCallback(String k, Object v, CacheItemRemovedReason r)
        {
            if (v is WebReport)
            {
                WebReport webReport = v as WebReport;
                try
                {
                    if (webReport.Report.PreparedPages != null)
                        webReport.Report.PreparedPages.Clear();

                    webReport.ReportDone = false;

                    //PutObject(k, webReport);
                    

                    //webReport.Report.Dispose();
                    //webReport.Report = null;
                }
                catch
                {
                    // nothing
                }
            }
        }
#endif
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public WebReportCache(HttpContext context)
        {
            fileStoragePath = GetStoragePath(context);
            webFarmMode = !String.IsNullOrEmpty(fileStoragePath);
            fileStorageTimeout = GetStorageTimeout();
            storageCleanup = GetStorageCleanup();
        }
    }
}