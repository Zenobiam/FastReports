using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FastReport.Web
{
    /// <summary>
    /// 
    /// </summary>
    internal enum WebTemplateMode
    {
        /// <summary>
        /// 
        /// </summary>
        CSS,
        /// <summary>
        /// 
        /// </summary>
        HTML
    }

    /// <summary>
    /// 
    /// </summary>
    internal class WebTemplate
    {
        private string sourceTemplate;
        private string preparedTemplate;
        private Dictionary<string, string> variables = new Dictionary<string, string>();
        private WebTemplateMode mode;
        private string var_prefix;
        private string var_suffix;
        private bool clean = false;

        private const string css_delim = " ;}\t";
        private const int max_key_length = 50;


        /// <summary>
        /// 
        /// </summary>
        public bool Clean
        {
            get
            {
                return clean;
            }
            set
            {
                clean = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Prepared
        {
            get
            {
                return preparedTemplate;
            }
            set
            {
                preparedTemplate = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Source
        {
            get
            {
                return sourceTemplate;
            }
            set
            {
                sourceTemplate = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> Variables
        {
            get
            {
                return variables;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public WebTemplateMode Mode
        {
            get
            {
                return mode;
            }
            set
            {
                SetTemplateMode(value);
            }
        }


        private void SetTemplateMode(WebTemplateMode value)
        {
            mode = value;
            if (mode == WebTemplateMode.CSS)
            {
                var_prefix = "/*";
                var_suffix = "*/";
            }
            else
            {
                var_prefix = "<!--";
                var_suffix = "-->";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetVariable(string key, string value)
        {
            if (variables.ContainsKey(key))
                variables[key] = value;
            else
                variables.Add(key, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetVariable(string key)
        {
            if (variables.ContainsKey(key))
                return variables[key];
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public void LoadFromStream(Stream stream)
        {
            byte[] buff = new byte[stream.Length - stream.Position];
            stream.Read(buff, 0, buff.Length);
            sourceTemplate = Encoding.UTF8.GetString(buff);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public void SavePreparedToStream(Stream stream)
        {
            byte[] buff = Encoding.UTF8.GetBytes(preparedTemplate);
            stream.Write(buff, 0, buff.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public string Prepare(string template)
        {
            sourceTemplate = template;
            return Prepare();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Prepare()
        {
            StringBuilder result = new StringBuilder(sourceTemplate.Length);
            int position = 0;
            int startPrefixPos;            
            do 
            {
                startPrefixPos = sourceTemplate.IndexOf(var_prefix, position);
                if (startPrefixPos != -1)
                {                
                    int startKeyPos = startPrefixPos + var_prefix.Length;
                    int max_length = max_key_length;
                    if (startKeyPos + max_key_length > sourceTemplate.Length)
                    {
                        max_length = sourceTemplate.Length - startKeyPos;
                    }
                    int endKeyPos = sourceTemplate.IndexOf(var_suffix, startKeyPos, max_length);
                    if (endKeyPos != -1)
                    {
                        string keyName = sourceTemplate.Substring(startKeyPos, endKeyPos - startKeyPos);
                        if (variables.ContainsKey(keyName))
                        {
                            if (mode == WebTemplateMode.HTML)
                            {
                                result.Append(sourceTemplate.Substring(position, startPrefixPos - position));
                                result.Append(variables[keyName]);
                                position = endKeyPos + var_suffix.Length;
                            }
                            else
                            {
                                result.Append(sourceTemplate.Substring(position, startPrefixPos - position));
                                result.Append(variables[keyName]);
                                position = sourceTemplate.IndexOfAny(css_delim.ToCharArray(), endKeyPos + var_suffix.Length, max_key_length);
                            }
                        }
                        else
                        {
                            result.Append(sourceTemplate.Substring(position, startPrefixPos - position));
                            position = endKeyPos + var_suffix.Length;
                        }
                    }
                    else
                    {
                        result.Append(sourceTemplate.Substring(position, startKeyPos - position));
                        position = startKeyPos;
                    }                    
                }
                else
                {
                    result.Append(sourceTemplate.Substring(position));
                }
            }
            while (startPrefixPos != -1);
            if (Clean)
                result.Replace("\r\n", "");                
            preparedTemplate = result.ToString();
            return preparedTemplate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <param name="templateMode"></param>
        /// <param name="humanreadable"></param>
        public WebTemplate(string template, WebTemplateMode templateMode, bool humanreadable)
        {
            sourceTemplate = template;
            Mode = templateMode;
            clean = !humanreadable;
        }

        /// <summary>
        /// 
        /// </summary>
        public WebTemplate()
        {
            Mode = WebTemplateMode.HTML;
        }
    }
}
