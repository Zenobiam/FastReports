using FastReport.Barcode;
using FastReport.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
#if DEBUG
using System.Diagnostics;
#endif
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

#pragma warning disable CS3001, CS3002, CS3003, CS1591

namespace FastReport.Fonts
{
    /// <summary>
    /// Request font file
    /// </summary>
    /// <param name="font">System.Drawing.Font object</param>
    /// <param name="stream">System/IO.Stream object</param>
    public delegate void RequestFontHandler(System.Drawing.Font font, out Stream stream);
    /// <summary>
    /// Get full path to a font file by it's identifier
    /// </summary>
    /// <param name="font_id">font name with attributes</param>
    /// <param name="pathname">path to a font file</param>
    public delegate void GetFontPathHandler(string font_id, out string pathname);
    /// <summary>
    /// assign full path to a font file to it's identifier
    /// </summary>
    /// <param name="font_id">font name with attributes</param>
    /// <param name="pathname">path to a font file</param>
    public delegate void SetFontPathHandler(string font_id, string pathname);

    /////////////////////////////////////////////////////////////////////////////////////////////////
    // True Type Font Collection
    /////////////////////////////////////////////////////////////////////////////////////////////////
    public class TrueTypeCollection : TTF_Helpers
    {
        internal class TrueTypeImage
        /// <summary>
        /// Request font stream 
        /// </summary>
        {
            private int ref_count = 0;
            private IntPtr beginfile_ptr;

            TrueTypeImage(IntPtr beginfile_ptr)
            {
                this.beginfile_ptr = beginfile_ptr;
                ref_count = 1;
            }

            ~TrueTypeImage()
            {
                --ref_count;
                if (ref_count == 0)
                    Marshal.FreeHGlobal(beginfile_ptr);
            }
            void AddReference() { ++ref_count; }
        }

        private static Dictionary<string, string> font_hash = new Dictionary<string, string>();
        private static Dictionary<string, TrueTypeFont> fonts_collection = new Dictionary<string, TrueTypeFont>();
        private static Dictionary<string, string> name_collisions = new Dictionary<string, string>();
        private static object lockObj = new object();
        /// <summary>
        /// Request font file location
        /// </summary>
        public static event RequestFontHandler OnRequestFontData = null;
        /// <summary>
        /// Request font file location
        /// </summary>
        public static event GetFontPathHandler OnGetFontPath = null;
        /// <summary>
        /// Register font file
        /// </summary>
        public static event SetFontPathHandler OnSetFontPath = null;

#pragma warning disable CS1591  // Missing XML comment for publicly visible type or member
        /// <summary>
        /// Will be removed soon
        /// </summary>
        public enum FileCheckOptions { Check, Parse, Load }

        /// <summary>
        /// Different caching strategies
        /// </summary>
        public enum CacheOptions { DoNotUpdate, UpdatePath, UpdateCollection, UpdateAll }

        #region "Public properties"
        public Dictionary<string, string> FontHash { get { return font_hash; } }
        public Dictionary<string, TrueTypeFont> Collection { get { return fonts_collection; } }
        #endregion

        #region "Collection structures"
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct TTCollectionHeader
        {
            [FieldOffset(0)]
            public uint TTCTag;     //  	TrueType Collection ID string: 'ttcf'
            [FieldOffset(4)]
            public uint Version;    // 	Version of the TTC Header (1.0), 0x00010000
            [FieldOffset(8)]
            public uint numFonts;   // 	Number of fonts in TTC
        }
        #endregion

        private static FontType CheckFontType(byte[] font)
        {
            FontType font_type = FontType.UnknownFontType;
            if (font[0] == 0 && font[1] == 1 && font[2] == 0 && font[3] == 0)
            {
                font_type = FontType.TrueTypeFont;
            }
            else if (font[0] == 't' && font[1] == 't' && font[2] == 'c' && font[3] == 'f')
            {
                font_type = FontType.TrueTypeCollection;
            }
            else if (font[0] == 't' && font[1] == 'r' && font[2] == 'u' && font[3] == 'e')
            {
                font_type = FontType.TrueTypeFont;
            }
            else if (font[0] == 'O' && font[1] == 'T' && font[2] == 'T' && font[3] == 'O')
            {
                font_type = FontType.OpenTypeFont;
            }
            return font_type;
        }

        /// <summary>
        /// Progress callback type definition
        /// </summary>
        /// <param name="status">Progress information</param>
        public delegate void StatusHandler(string status);

        /// <summary>
        /// Progress callback event
        /// </summary>
        public static event StatusHandler OnStatusChanged = null;

        internal static IList<TrueTypeFont> LoadfontStream(Stream f, FileCheckOptions options, string path)
        {
            IList<TrueTypeFont> ttf = null;
            FontType font_type = FontType.UnknownFontType;
            byte[] font;

            if (f.Length < 100)
                return ttf;

            font = new byte[(int)f.Length];
            f.Read(font, 0, (int)f.Length);
            f.Close();

            font_type = CheckFontType(font);

            if (font_type != FontType.UnknownFontType)
            {
                IntPtr font_data = Marshal.AllocHGlobal(font.Length);
                Marshal.Copy(font, 0, font_data, font.Length);

                ttf = AddFontData(font_type, font_data);

                if (options != FileCheckOptions.Load)
                    Marshal.FreeHGlobal(font_data);
            }

            font = null;
            return ttf;
        }

        /// <summary>
        /// Open and parse TrueType file
        /// </summary>
        /// <param name="cache_options"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IList<TrueTypeFont> CheckFile(CacheOptions cache_options, string path)
        {
            IList<TrueTypeFont> result = new List<TrueTypeFont>();
            FontType font_type;

            if (!File.Exists(path))
            {
                // http://support.fast-report.com/tickets/273969
                return result;
            }

            if (OnStatusChanged != null)
                OnStatusChanged(path);

            FileStream f = File.OpenRead(path);
            if (f.Length > 100)
            {
                byte[] font_bytes = new byte[(int)f.Length];
                f.Read(font_bytes, 0, (int)f.Length);

                font_type = CheckFontType(font_bytes);

                if (font_type != FontType.UnknownFontType)
                {
                    IntPtr font_data = Marshal.AllocHGlobal(font_bytes.Length);
                    Marshal.Copy(font_bytes, 0, font_data, font_bytes.Length);

                    result = AddFontData(font_type, font_data);
                    if (cache_options != CacheOptions.UpdateCollection)
                        Marshal.FreeHGlobal(font_data);
                }
                font_bytes = null;


                if (cache_options != CacheOptions.DoNotUpdate)
                {
                    bool update_font_hash = (cache_options == CacheOptions.UpdatePath) | (cache_options == CacheOptions.UpdateAll);
                    bool update_collection = (cache_options == CacheOptions.UpdateCollection) | (cache_options == CacheOptions.UpdateAll);
                    foreach (TrueTypeFont ttf in result)
                    {
                        string fontname = ttf.FastName;
                        if (update_font_hash && !font_hash.ContainsKey(fontname))
                        {
                            font_hash.Add(fontname, path);
                            if (OnSetFontPath != null)
                                OnSetFontPath(fontname, path);
                        }
#if DEBUG_TTF
            else
              Console.WriteLine("Font already exist in font_hash:\n" + ttf.FastName);
#endif
                        if (update_collection && !fonts_collection.ContainsKey(fontname))
                            fonts_collection.Add(fontname, ttf);
#if DEBUG_TTF
            else
              Console.WriteLine("Font already exist in fonts_collection:\n" + ttf.FastName);
#endif
                    }
                }
            }
            f.Close();
            return result;
        }

        /// <summary>
        /// Find all fonts files in directory and it's subdirectories
        /// </summary>
        /// <param name="path">Directory where find fond files</param>
        /// <param name="font_list">Defines file which collect found fonts</param>
        public static void RecursiveFind(string path, string font_list)
        {
            detected_font_files = new List<string>();
            BuildListOfFontsInDirectory(path);
            bool fonts_were_modified = CompareHashes(true, true);
            if (fonts_were_modified)
                StoreFontList(font_list);
            detected_font_files = null;
        }


        /// <summary>
        /// Get TrueType font from static collection
        /// </summary>
        /// <param name="key">String which identifies font - family name + "-B" for bold and "-I" for italic</param>
        /// <returns></returns>
        public static TrueTypeFont GetTrueTypeFont(string key)
        {
            TrueTypeFont f = null;
            string file_name = String.Empty;

            lock (fonts_collection)
            {
                if (OnGetFontPath != null)
                {
                    if (font_hash.ContainsKey(key) == true)
                        file_name = (string)font_hash[key];
                    else
                    {
                        OnGetFontPath(key, out file_name);
                        font_hash[key] = file_name;
                    }
                }
                else
                {
                    if (font_hash.Count == 0)
                        CheckFontList(Utils.Config.FontListFolder);

                    if (!fonts_collection.ContainsKey(key))
                    {
#if false 
                        // Correct look or fail on trying to emulate regular font from italic
                        key = SelectBestFontFromHash(key);
                        if(key == String.Empty)
                            throw new FileNotFoundException(key);
#else
                        // Agressive attribute substitution, less fail but possible ugly look
                        string try_key = key;
                        do
                        {
                            if (font_hash.ContainsKey(key))
                            {
                                file_name = font_hash[key];
                                break;
                            }
                            try_key = ReverseBoldAttribute(key);
                            if (font_hash.ContainsKey(try_key))
                            {
                                file_name = font_hash[try_key];
                                font_hash.Add(key, file_name);
                                break;
                            }
                            try_key = ReverseItalicAttribute(key);
                            if (font_hash.ContainsKey(try_key))
                            {
                                file_name = font_hash[try_key];
                                font_hash.Add(key, file_name);
                                break;
                            }
                            try_key = ReverseBoldAttribute(key);
                            if (font_hash.ContainsKey(try_key))
                            {
                                file_name = font_hash[try_key];
                                font_hash.Add(key, file_name);
                                break;
                            }
                            throw new FileNotFoundException(key);
                        }
                        while (false);
#endif
                        if (!fonts_collection.ContainsKey(key))
                        {
                            if(try_key != key && fonts_collection.ContainsKey(try_key))
                            {
                                f = fonts_collection[try_key];
                                fonts_collection[key] = f;
                            }
                            else
                            {
                                file_name = font_hash[key];
                                IList<TrueTypeFont> list_ttf = CheckFile(CacheOptions.UpdateCollection, file_name);

                                if (!fonts_collection.ContainsKey(key))
                                {
                                    if (list_ttf.Count == 1)
                                        fonts_collection[key] = list_ttf[0] as TrueTypeFont;
                                    else if (list_ttf.Count > 1)
                                    {
                                        // Need additional parsing for selecting best font from collection
                                        fonts_collection[key] = list_ttf[0] as TrueTypeFont;
                                    }
                                }
                            }
                        }
                    }

                    if (!fonts_collection.ContainsKey(key))
                        throw new FileLoadException(file_name);
                }

                if (fonts_collection.ContainsKey(key) == false)
                {
                    IList<TrueTypeFont> lf = CheckFile(CacheOptions.UpdateCollection, file_name);
                    foreach(TrueTypeFont ttf in lf)
                    {
                        fonts_collection[key] = ttf;
                        ttf.PrepareIndexes();
                    }
                }
                f = fonts_collection[key] as TrueTypeFont;
            }
            return f;
        }

        /// <summary>
        /// Get font by its name and bold/italic attributes
        /// </summary>
        /// <param name="FastName"></param>
        /// <returns>TrueType font object</returns>
        public TrueTypeFont this[string FastName]
        {
            get { return GetTrueTypeFont(FastName); }
        }
        public TrueTypeFont this[FastFont index]
        {
            get { return GetTrueTypeFont(index.FastName); }
        }

        public TrueTypeFont this[System.Drawing.Font index]
        {
            get {

                TrueTypeFont f = null;
                string key = index.OriginalFontName + (index.Bold ? "-B" : "") + (index.Italic ? "-I" : "");

                if (OnRequestFontData != null)
                {
                    if (fonts_collection.ContainsKey(key) == false)
                    {
                        Stream font_stream;
                        OnRequestFontData(index, out font_stream);
                        LoadfontStream(font_stream, FileCheckOptions.Load, "");
                        font_stream.Close();
                    }
                    f = fonts_collection[key];
                }
                else
                {
                    f = GetTrueTypeFont(key);
                }
                return f;
            }
        }

        /// <summary>
        /// Workaround function for avoid font duplicates
        /// </summary>
        /// <param name="load">if true then font append to collection, if false then font append to hash</param>
        /// <param name="f">Internal font structure</param>
        /// <param name="path">This parameter defines path to font file if file append to hash. Not used otherwise</param>
        public static void ParseFont(bool load, TrueTypeFont f, string path)
        {
            ICollection names = f.GetNames(NameTableClass.NameID.FamilyName);
            string key;
            foreach (string s in names)
            {
                key = s;
                if (key.EndsWith(" Italic"))
                {
                    key = key.Remove(key.Length - 7);
                }
                if (s.EndsWith(" Bold"))
                {
                    key = key.Remove(key.Length - 5);
                }
                if (key.EndsWith(" Regular"))
                {
                    key = key.Remove(key.Length - 8);
                }
                key += (f.Bold ? "-B" : "") + (f.Italic ? "-I" : "");
                if (load) lock (fonts_collection)
                    {
                        fonts_collection[key] = f as TrueTypeFont;
                    }
                else lock (font_hash)
                    {
                        if (!font_hash.ContainsKey(key))
                    {
                            font_hash[key] = path;
                        if (OnSetFontPath != null)
                            OnSetFontPath(key, path);
                    }
                        else
                        {
                            int dup = 2;
                            string dup_key;
                            do dup_key = dup++.ToString() + key; while (font_hash.ContainsKey(dup_key));
                            font_hash[dup_key] = path;
                        }
                    }
            }
        }

        /// <summary>
        /// Parse raw image of font data and create TrueTypeFont objects from this image
        /// </summary>
        /// <param name="CollectionMode">Defines font or collections</param>
        /// <param name="font_data">Pointer to memory of raw font data</param>
        /// <returns></returns>
        public static IList<TrueTypeFont> AddFontData(FontType CollectionMode, IntPtr font_data)
        {
            TrueTypeFont f = null;
            IList<TrueTypeFont> result = new List<TrueTypeFont>();

            try
            {
                if (CollectionMode == FontType.TrueTypeFont || CollectionMode == FontType.OpenTypeFont)
                {
                    f = new TrueTypeFont(font_data, font_data);
                    f.PrepareCommonTables();
                    result.Add(f);
                }
                else
                {
                    TTCollectionHeader ch = (TTCollectionHeader)Marshal.PtrToStructure(font_data, typeof(TTCollectionHeader));
                    ch.Version = SwapUInt32(ch.Version);
                    ch.numFonts = SwapUInt32(ch.numFonts);

                    IntPtr subfont_table = Increment(font_data, Marshal.SizeOf(ch));

                    for (int i = 0; i < ch.numFonts; i++) unchecked
                        {
                            UInt32 subfont_idx = SwapUInt32((UInt32)Marshal.ReadInt32(subfont_table, i * sizeof(UInt32)));
                            IntPtr subfont_ptr = Increment(font_data, (int)subfont_idx);

                            f = new TrueTypeFont(font_data, subfont_ptr);
                            f.PrepareCommonTables();
                            result.Add(f);
                        }
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine("Unable add font: " + e.Message);
#else
        throw new Exception("Unable add font", e);
#endif
            }
            return result;
        }

        static TrueTypeCollection()
        {
            // Nohing TO DO
        }

        static IList<string> detected_font_files = null;
        private static void BuildListOfFontsInDirectory(string path)
        {
            foreach (string dir in Directory.GetDirectories(path))
                BuildListOfFontsInDirectory(dir);
            foreach (string file in Directory.GetFiles(path))
                if (file.ToLower().EndsWith(".ttf") || file.ToLower().EndsWith(".ttc") || file.ToLower().EndsWith(".otf"))
                    if (detected_font_files.Contains(file) == false)
                        detected_font_files.Add(file);
        }

        private static bool CompareHashes(bool delete, bool append)
        {
            IList<TrueTypeFont> new_fonts = null;
            List<string> removed_fonts = new List<string>();

            if (delete)
                foreach (string filename in font_hash.Values)
                    if (detected_font_files.Contains(filename) == false)
                        foreach (string dict in font_hash.Values)
                            if (dict == filename)
                                removed_fonts.Add(dict);

            foreach (string fontname in removed_fonts)
                font_hash.Remove(fontname);

            ICollection<string> files = font_hash.Values;
            if (append)
                foreach (string filename in detected_font_files)
                    if (files.Contains(filename) == false)
                    {
                        new_fonts = CheckFile(CacheOptions.DoNotUpdate, filename);
                        foreach (TrueTypeFont ttf in new_fonts)
                        {
                            string fastname = ttf.FastName;
                            if (font_hash.ContainsKey(fastname) == false)
                                font_hash[fastname] = filename;

                            foreach (string familyname_lang in ttf.GetNames(NameTableClass.NameID.FamilyName))
                            {
                                string key = familyname_lang + (ttf.Bold ? "-B" : string.Empty) + (ttf.Italic ? "-I" : string.Empty);
                                if (font_hash.ContainsKey(key) == false)
                                    font_hash[key] = filename;
#if false // Enable this to track fontname collisions
                                else
                                {
                                    
                                    if (name_collisions.ContainsKey(key) == false)
                                        name_collisions[key] = filename;
                                }
#endif

                            }
                        }
                    }
            return (removed_fonts.Count > 0) || (new_fonts != null && new_fonts.Count > 0);
        }

        readonly static string[] searchable_font_dirs =
          {
      // Linux font placeses 
      "/usr/share/fonts",
      "/usr/local/share/fonts",
      "~/.fonts",
      "~/.local/share/fonts",

      // Apple OSX font places
      "~/Library/Fonts/",
      "/Library/Fonts/",
      "/Network/Library/Fonts/",
      "/System/Library/Fonts/",
      "/System Folder/Fonts/"
    };

        private static string ReverseBoldAttribute(string font_id)
        {
            string result = font_id;
            string italic = "-I";

            if (font_id.EndsWith(italic))
                result = result.Substring(0, result.Length - 2);
            else
                italic = String.Empty;

            if (result.EndsWith("-B"))
                result = result.Substring(0, result.Length - 2);
            else
                result += "-B";

            result += italic;
            return result;
        }

        private static string ReverseItalicAttribute(string font_id)
        {
            const string italic = "-I";

            if (font_id.EndsWith(italic))
                return font_id.Substring(0, font_id.Length - 2);
            else
                return font_id + italic;
        }

        private static string SelectBestFontFromHash(string font_id)
        {
            if (font_hash.ContainsKey(font_id))
                return font_id;

            string key;
            bool italic = font_id.EndsWith("-I");

            if (italic)
            {
                key = font_id.Substring(0, font_id.Length - 2);
                if (font_hash.ContainsKey(key))
                    return key;
                bool bold = key.Substring(0, font_id.Length - 2).EndsWith("-B");
                if (!bold)
                {
                    key += "-B";
                    if (font_hash.ContainsKey(key))
                        return key;
                }
            }
            else
            {
                if (font_id.EndsWith("-B"))
                {
                    key = font_id.Substring(0, font_id.Length - 2);
                    if (font_hash.ContainsKey(key))
                        return key;
                }
                else
                {
                    key = font_id + "-B";
                    if (font_hash.ContainsKey(key))
                        return key;
                }
            }
            return String.Empty;
        }

        private static IList<string> GetFontDirectories()
        {
            IList<string> dir_list = new List<string>();

            string fontPath;

            fontPath = Environment.GetEnvironmentVariable("FONTDIR", EnvironmentVariableTarget.Process);
            if (fontPath != null && Directory.Exists(fontPath))
            {
                dir_list.Add(fontPath);
                return dir_list;
            }

            // Check Windows 10 user fonts folder
            fontPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if(fontPath != null)
            {
                if (fontPath.EndsWith(@"\Roaming"))
                    fontPath = fontPath.Remove(fontPath.Length - 8, 8);
                fontPath += @"\Local\Microsoft\Windows\Fonts";
                if(Directory.Exists(fontPath))
                    dir_list.Add(fontPath); 
            }

            // Windows font folders
            fontPath = Environment.GetEnvironmentVariable("SystemRoot", EnvironmentVariableTarget.Process);
            if (fontPath != null && Directory.Exists(fontPath))
            {
                fontPath += @"\Fonts";
                if (Directory.Exists(fontPath))
                    dir_list.Add(fontPath);
            }

            foreach (string fontDir in searchable_font_dirs)
            {
                if(fontDir.StartsWith("~"))
                {
                    string  constructed_path = Environment.GetEnvironmentVariable("HOME", EnvironmentVariableTarget.Process);
                    constructed_path += fontDir.Substring(1);
                    if (Directory.Exists(constructed_path))
                        dir_list.Add(constructed_path);
                }
                else
                {
                    if (Directory.Exists(fontDir))
                        dir_list.Add(fontDir);
                }
            }

            return dir_list;
        }

        private static string GetDelimiters(int key_len)
        {
            return key_len < 8 ? "\t\t\t\t" : key_len < 16 ? "\t\t\t" : key_len < 24 ? "\t\t" : "\t";
        }

        private static void StoreFontList(string font_list)
        {
            StreamWriter fl = null;

            try
            {
                fl = File.CreateText(font_list);
            }
            catch (UnauthorizedAccessException)
            {
                string tempPath = Path.GetTempPath();
                fl = File.CreateText(Path.Combine(tempPath, Path.GetFileName(font_list)));
            }

            foreach (string key in font_hash.Keys)
            {
                fl.WriteLine("{0}{1}{2}", key, GetDelimiters(key.Length), font_hash[key]);
            }
            fl.Close();
        }

       

        private static void LoadFontList(string font_list)
        {
            string[] lines = File.ReadAllLines(font_list);
            foreach (string line in lines)
            {
                string[] record = line.Split('\t');
                if (record.Length < 2)
                {
                    continue;
                }
                int second_field;
                for (second_field = 1; second_field < record.Length; second_field++)
                {
                    if (record[second_field].Length != 0)
                    {
                        font_hash[record[0]] = record[second_field];
                        break;
                    }
                }
            }
        }

        

        /// <summary>
        /// Build list of fonts
        /// </summary>
        /// <param name="folder">Optional path to font.list folder.</param>
        private static void CheckFontList(string folder)
        {
            if (Monitor.TryEnter(lockObj))
            {
                try
                {
                    string FontListFolder = folder ?? Utils.Config.Folder ?? AppDomain.CurrentDomain.BaseDirectory;
                    string font_list_Version = "font" + Config.Version + ".list";
                    string font_list = Path.Combine(FontListFolder, font_list_Version);
                    string font_list_temp = Path.Combine(Path.GetTempPath(), font_list_Version);
                    if (!Directory.Exists(FontListFolder))
                    {
                        Directory.CreateDirectory(FontListFolder);
                    }

                    if (File.Exists(font_list))
                        LoadFontList(font_list);
                    else if (File.Exists(font_list_temp))
                        LoadFontList(font_list_temp);

                    detected_font_files = new List<string>();
                    IList<string> sts = GetFontDirectories();
                    foreach (string font_dir in sts)
                    {
                        if (font_dir == null)
                            break;
                        BuildListOfFontsInDirectory(font_dir);
                    }

                    bool fonts_were_modified = CompareHashes(true, true);
                    if (fonts_were_modified)
                        StoreFontList(font_list);
                    detected_font_files = null;
                }
                finally
                {
                    Monitor.Exit(lockObj);
                }
            }
            else
            {
                Monitor.Enter(lockObj);
                Monitor.Exit(lockObj);
            }
        }

        /// <summary>
        /// Enumerate available fonts
        /// </summary>
        public void EnumerateFonts()
        {
            detected_font_files = new List<string>();
            IList<string> sts = GetFontDirectories();
            foreach (string font_dir in sts)
            {
                if (font_dir == null)
                    break;
                BuildListOfFontsInDirectory(font_dir);
            }

            foreach(string filename in detected_font_files)
            {
                CheckFile(CacheOptions.UpdatePath, filename); // Found new font
            }
        }

    }

}
#pragma warning restore