using System;
using System.IO;
using System.Text;

namespace FastReport.Web
{
    internal class WebLog
    {
        private object locker = new object();
    
        private StringBuilder log = new StringBuilder();
        private bool showStackTrace = false;
        private string logFileName;

        public string Text
        {
            get { return log.ToString(); }
        }   
        
        public string LogFile
        {
            get { return logFileName;  }
            set { logFileName = value; }
        }

        public void Add(string line)
        {
            log.Append(line).Append("<br />");
        }

        public void Clear()
        {
            log = new StringBuilder();
        }

        public void AddError(Exception e)
        {
            lock (locker)
            {
                Add(String.Format("<span style=\"color:red\"><b>ERROR:</b><br /> {0}</span>", e.Message.Replace("\n", "<br />")));
                if (showStackTrace)
                    Add(e.StackTrace);
            }
        }

        public void Flush()
        {
            log.AppendLine().AppendLine();
            if (!String.IsNullOrEmpty(logFileName))
            {
                try
                {
                    lock (locker)
                    {
                        if (File.Exists(logFileName))
                        {
                            using (FileStream file = new FileStream(logFileName, FileMode.Append))
                            using (StreamWriter writer = new StreamWriter(file, Encoding.UTF8))
                            {
                                writer.Write(log);
                            }
                        }
                        else
                        {
                            using (FileStream file = new FileStream(logFileName, FileMode.Create))
                            using (StreamWriter writer = new StreamWriter(file, Encoding.UTF8))
                            {
                                writer.Write(log);
                            }
                        }
                    }
                }
                catch
                {
                    //
                }
            }
        }

        public WebLog(bool trace)
        {
            showStackTrace = trace;
        }
    }
}
