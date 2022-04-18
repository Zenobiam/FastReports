using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

#pragma warning disable CS3001, CS3002, CS3003, CS1591
namespace FastReport.Fonts
{
    public class GlyphChar
    {
        private ushort glyph;
        private float width;

        //one of
        private GlyphType glyphType;
        private char character;
        private string str;

        public ushort Glyph
        {
            get
            {
                return glyph;
            }
            set
            {
                glyph = value;
            }
        }

        public float Width
        {
            get
            {
                return width;
            }
        }

        public GlyphType GlyphType
        {
            get
            {
                return glyphType;
            }
            set
            {
                glyphType = value;
            }
        }

        public char Character
        {
            get
            {
                return character;
            }
            set
            {
                character = value;
            }
        }

        public string String
        {
            get
            {
                return str;
            }
        }


        public GlyphChar(ushort glyph, float width, char character)
        {
            this.glyph = glyph;
            this.width = width;
            this.character = character;
            this.glyphType = GlyphType.Character;
        }

        public GlyphChar(ushort glyph, float width, string str)
        {
            this.glyph = glyph;
            this.width = width;
            this.str = str;
            this.glyphType = GlyphType.String;
        }

        public GlyphChar(ushort glyph, float width)
        {
            this.glyph = glyph;
            this.width = width;
            this.glyphType = GlyphType.None;
        }
    }

    public enum GlyphType
    {
        None,
        Character,
        String,
        Index
    }


    /// <summary>
    /// TrueTypeFont object
    /// </summary>
    public partial class TrueTypeFont : OpenTypeFontBase, IDisposable
    {
        #region Structures
        public enum ChecksumFaultAction
        {
            IgnoreChecksum,
            ThrowException,
            Warn
        }

        public enum FontPackOptions
        {
            /// <summary>
            /// Expected default behaviour as version 2020.3
            /// </summary>
            Defatult_Options = 0,
            /// <summary>
            /// We use dictionary and pack "Index To Location" table (reorder glyph indexes)
            /// </summary>
            Pack_Indexes = 1

            /* These definitions are reserved
               uniscribe = 2,      // Use uniscribe instead direct structures 
               decompose = 4,      // If true then ignore CMAP table
               collate = 8;        // 
               use_kerning = 16;    // Use kerning table for g;yph widths calculatiions
             */
        }

        #endregion

        #region "Data"

        IntPtr selector_ptr;   // Pointer to subfont
        private HorizontalHeaderClass horizontal_header;
        private OS2WindowsMetricsClass windows_metrix;
        private IndexToLocationClass index_to_location;
        private CmapTableClass cmap_table;
        private GlyphSubstitutionClass gsub_table;
        private GlyphTableClass glyph_table;
        private KerningTableClass kerning_table;
        private HorizontalMetrixClass horizontal_metrix_table;
        private uint calculated_file_size;
        private bool parsed;
        private string lang;
        private FontType font_type;

        private bool need_bold_emulation = false;
        private bool need_italic_emulation = false;
        private bool text_tables_preapred = false;
        private bool skip_GSUB_parsing = false;
        private bool skip_CMAP_on_export = true;
        private bool skip_GSUB_on_export = true;
        #endregion

        #region "Public propertirs"

        public GlyphTableClass Glyphs { get { return glyph_table; } }
        public IndexToLocationClass I2L { get { return index_to_location; } }
        public OS2WindowsMetricsClass OS2WindowsMetrics { get { return windows_metrix; } }
        public KerningTableClass Kerning { get { return kerning_table; } }
        public HorizontalMetrixClass HMetrics { get { return horizontal_metrix_table; } }

        public ChecksumFaultAction checksum_action;
        public bool Bold
        {
            get
            {
                return need_bold_emulation |
                  (
                    (0 != (Header.MacStyle & 1)) |
                   (windows_metrix != null) ?
                    windows_metrix.IsBold : false // OSX sometime ignores MacStyle, so check Windows Metrics
                  );
            }
            set
            {
                need_bold_emulation = value;
            }
        }
        public bool Italic
        {
            get
            {
                return need_italic_emulation |
                  (
                    (Postscript != null) ?
                    Postscript.IsItalic :
                    0 != (Header.MacStyle & 2)
                  );
            }
            set
            {
                need_italic_emulation = value;
                //////if (postscript != null)
                //////  postscript.IsItalic = value;
                //////font_header.MacStyle = (ushort)(value ? (font_header.MacStyle | 2) : (font_header.MacStyle & 0xfffd));
            }
        }
        public uint FontVersion { get { return (uint)SwapInt32(Marshal.ReadInt32(selector_ptr)); } }
        public float baseLine { get { return Postscript.post_script.underlinePosition / (float)Header.unitsPerEm; } }
        public float Ascender { get { return horizontal_header.Ascender / (float)Header.unitsPerEm; } }
        public float LeftOffSet { get { return horizontal_header.MinLeftSideBearing / (float)Header.unitsPerEm; } }
        public bool IsClassicFormat { get { return this.listOfTables.ContainsKey(TablesID.Glyph); } }
        public bool IsCollection { get { return selector_ptr != beginfile_ptr; } }
        private string script;
        /// <summary>
        /// Get or set current script
        /// </summary>
        public string Script
        {
            get
            {
                return script;
            }
            set
            {
                script = value;
            }
        }
        /// <summary>
        /// Get or set current Language
        /// </summary>
        public string Language { get { return lang ?? string.Empty; } set { lang = value; } }
        /// <summary>
        /// Get available scripts
        /// </summary>
        public IEnumerable<string> Scripts { get { return gsub_table.Scripts; } }


        public FontType Type { get { return font_type; } set { font_type = value; } }
        public IntPtr RawData { get { return selector_ptr; } }
        public uint FileSize { get { return calculated_file_size; } set { calculated_file_size = value; } }

        public string FastName { get { return Name + (Bold ? "-B" : string.Empty) + (Italic ? "-I" : string.Empty); } }

        public int LicensingRights
        {
            get
            {
                return this.windows_metrix == null ? 0 : (int)windows_metrix.LicensingRights;
            }
        }

        /// <summary>
        /// Get available languages for script
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public IEnumerable<string> GetLanguages(string script) { return gsub_table.Languages(script); }
        /// <summary>
        /// Get available features for language and script
        /// </summary>
        /// <param name="script"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public IEnumerable<string> GetFeatures(string script, string language)
        {
            return gsub_table.GetFeatures(script, language);
        }
        #endregion

        #region Private functions
        ///////////////////////////////////////////////////////////////////////////////////////
        // Change byte-order of table fields
        ///////////////////////////////////////////////////////////////////////////////////////
        private void ChangeEndian()
        {
            dir.sfntversion = SwapUInt32(dir.sfntversion);
            dir.numTables = SwapUInt16(dir.numTables);
            dir.searchRange = SwapUInt16(dir.searchRange);
            dir.entrySelector = SwapUInt16(dir.entrySelector);
            dir.rangeShift = SwapUInt16(dir.rangeShift);
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        // Calculates checksum of font table
        ///////////////////////////////////////////////////////////////////////////////////////
        private uint CalcTableChecksum(bool packed, IntPtr font, TrueTypeTable entry)
        {
            uint Sum = 0;
            uint length = (entry.Length + 3) / 4;
            IntPtr TablePtr = Increment(font, packed ? (int)entry.PackedOfffset : (int)entry.Offset);

            int[] Temp = new int[length];
            Marshal.Copy(TablePtr, Temp, 0, (int)length);

            for (uint i = 0; i < length; i++) unchecked
                {
                    Sum += SwapUInt32((uint)Temp[i]);
                }

            Temp = null;

            return Sum;
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        // Checksum stuff
        ///////////////////////////////////////////////////////////////////////////////////////
        private void CheckTablesChecksum()
        {
            if (this.checksum_action == ChecksumFaultAction.IgnoreChecksum) return;
            foreach (TrueTypeTable entry in listOfTables.Values)
            {
                try
                {
                    uint cs = CalcTableChecksum(false, this.beginfile_ptr, entry);
                    if (cs != entry.checkSum) throw new Exception("Table ID \"" + entry.TAG + "\" checksum error.\r\nContinue?");
                }
                catch (Exception ex)
                {
                    if (this.checksum_action == ChecksumFaultAction.ThrowException) throw ex;
                    //                    DialogResult dr = MessageBox.Show(ex.Message, "Font table checksum error", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
                    //                    if (dr == DialogResult.No) throw ex;
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        // Checksum calculator
        ///////////////////////////////////////////////////////////////////////////////////////
        private void CalculateFontChecksum(
            IntPtr start_offset,
            uint font_length,
            FontHeaderClass new_header)
        {
            uint Sum = 0;
            int length = (int)font_length / 4;
            int[] Temp = new int[length];
            Marshal.Copy(start_offset, Temp, 0, length);
            for (uint i = 0; i < length; i++) unchecked
                {
                    Sum += SwapUInt32((uint)Temp[i]);
                }
            Temp = null;
            Sum = unchecked(0xb1b0afba - Sum);

            new_header.SaveFontHeader(start_offset, Sum);
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        // Order of TrueType tables is important
        ///////////////////////////////////////////////////////////////////////////////////////
        private ArrayList GetTablesOrder(out int font_size)
        {
            Hashtable TablesOrder = new Hashtable();
            font_size = 0;
            foreach (TrueTypeTable entry in listOfTables.Values)
            {
                switch ((TablesID)entry.tag)
                {
                    case TablesID.CMAP:
                        if (skip_CMAP_on_export)
                            continue;
                        break;
                    case TablesID.GlyphSubstitution:
                        if (skip_GSUB_on_export)
                            continue;
                        break;
                }
                TablesOrder.Add(entry.Offset, entry.tag);
                font_size += (int)((entry.Length + 3) & ~(uint)3);
            }
            ArrayList tables_positions = new ArrayList(TablesOrder.Keys);
            tables_positions.Sort();
            ArrayList indexed_tags = new ArrayList();
            foreach (uint offset in tables_positions)
            {
                uint tag = (uint)TablesOrder[offset];
                indexed_tags.Add((TablesID)tag);
            }
            tables_positions = null;
            TablesOrder = null;

            return indexed_tags;
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        // These tables are mandatory for TTF file
        ///////////////////////////////////////////////////////////////////////////////////////
        private bool LoadCoreTables()
        {
            parsed = false;
            bool bitmap = LoadBaseTables();

            if (listOfTables.ContainsKey(TablesID.HorizontalHeader))
            {
                horizontal_header = (HorizontalHeaderClass)listOfTables[TablesID.HorizontalHeader];
                horizontal_header.Load(this.beginfile_ptr);
            }
            else if (!bitmap)
                throw new Exception("HorizontalHeader not found."); ;

            if (listOfTables.ContainsKey(TablesID.OS2Table))
            {
                windows_metrix = (OS2WindowsMetricsClass)listOfTables[TablesID.OS2Table];
                windows_metrix.Load(this.beginfile_ptr);
            }
            else
            {
                // It looks like we found Apple font.
            }

            if (!listOfTables.ContainsKey(TablesID.CMAP)) throw new Exception("CMAP not found."); ;
            cmap_table = (CmapTableClass)listOfTables[TablesID.CMAP];
            cmap_table.LoadCmapTable(this.beginfile_ptr);


            parsed = !bitmap && this.Glyphs != null;

            return parsed;
        }

        /// <summary>
        /// Prepare tables for drawing, subsetting and so on
        /// </summary>
        /// <returns>always true</returns>
        public bool PrepareIndexes()
        {
            lock (this.listOfTables)
            {
                if (!text_tables_preapred)
                {
                    if (listOfTables.ContainsKey(TablesID.IndexToLocation))
                    {
                        index_to_location = (IndexToLocationClass)listOfTables[TablesID.IndexToLocation];
                        index_to_location.LoadIndexToLocation(this.beginfile_ptr, Header);
                    }
                    else
                    {
                        // Apple ITFDevanagari.ttc does not have this table. Why? How?
                    }
                    if (listOfTables.ContainsKey(TablesID.Glyph))
                    {
                        glyph_table = (GlyphTableClass)listOfTables[TablesID.Glyph];
                        glyph_table.Load(this.beginfile_ptr);
                    }
                    else
                    {
                        // Apple ITFDevanagari.ttc does not have this table. Why? How?
                    }

                    if (listOfTables.ContainsKey(TablesID.KerningTable))
                    {
                        kerning_table = (KerningTableClass)listOfTables[TablesID.KerningTable];
                        kerning_table.Load(this.beginfile_ptr);
                    }

                    if (!skip_GSUB_parsing && listOfTables.ContainsKey(TablesID.GlyphSubstitution))
                    {
                        gsub_table = (GlyphSubstitutionClass)listOfTables[TablesID.GlyphSubstitution];
                        gsub_table.Load(this.beginfile_ptr);
                    }

                    if (listOfTables.ContainsKey(TablesID.PreProgram))
                    {
                        PreProgramClass preprogram = (PreProgramClass)listOfTables[TablesID.PreProgram];
                        preprogram.Load(this.beginfile_ptr);
                    }

                    if (listOfTables.ContainsKey(TablesID.HorizontalMetrix))
                    {
                        horizontal_metrix_table = (HorizontalMetrixClass)listOfTables[TablesID.HorizontalMetrix];
                        horizontal_metrix_table.numberOfMetrics = horizontal_header.NumberOfHMetrics;
                        horizontal_metrix_table.Load(this.beginfile_ptr);
                    }
                }
                text_tables_preapred = true;
            }
            return text_tables_preapred;
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        // Parse important tables
        ///////////////////////////////////////////////////////////////////////////////////////
        private void LoadDescriptors(bool skip_list, ArrayList skip_array)
        {
            dir = (TableDirectory)Marshal.PtrToStructure(this.selector_ptr, typeof(TableDirectory));
            ChangeEndian();

            IntPtr tbls = Increment(selector_ptr, Marshal.SizeOf(dir));
            uint max_offset = 0;

            for (int i = 0; i < dir.numTables; i++)
            {
                TrueTypeTable table = new TrueTypeTable(tbls);

                if ((skip_list ^ skip_array.Contains((TablesID)table.tag)) == false)
                {
                    TrueTypeTable parsed_table;
                    switch ((TablesID)table.tag)
                    {
                        case TablesID.FontHeader: parsed_table = new FontHeaderClass(table); break;
                        case TablesID.MaximumProfile: parsed_table = new MaximumProfileClass(table); break;
                        case TablesID.Name: parsed_table = new NameTableClass(table); break;
                        case TablesID.IndexToLocation: parsed_table = new IndexToLocationClass(table); break;
                        case TablesID.CMAP: parsed_table = new CmapTableClass(table); break;
                        case TablesID.Glyph: parsed_table = new GlyphTableClass(table); break;
                        case TablesID.GlyphSubstitution: parsed_table = new GlyphSubstitutionClass(table); break;
                        case TablesID.PreProgram: parsed_table = new PreProgramClass(table); break;
                        case TablesID.HorizontalHeader: parsed_table = new HorizontalHeaderClass(table); break;
                        case TablesID.Postscript: parsed_table = new PostScriptClass(table); break;
                        case TablesID.OS2Table: parsed_table = new OS2WindowsMetricsClass(table); break;
                        case TablesID.KerningTable: parsed_table = new KerningTableClass(table); break;
                        case TablesID.HorizontalMetrix: parsed_table = new HorizontalMetrixClass(table); break;
                        case TablesID.BitmapFontHeader: parsed_table = new FontHeaderClass(table); break;

                        default: parsed_table = table; break;
                    }
                    try
                    {
                        listOfTables.Add((TablesID)table.tag, parsed_table);
                        if (max_offset < table.Offset)
                        {
                            max_offset = table.Offset;
                            calculated_file_size = table.Offset + table.Length;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                else
                {
                }
                tbls = Increment(tbls, table.descriptor_size);
            }
            dir.numTables = (ushort)listOfTables.Count;
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        // Contstruct TTF header
        ///////////////////////////////////////////////////////////////////////////////////////
        private void SaveDescriptors(IntPtr position, Hashtable ListOfPackedTables)
        {
            Header.checkSumAdjustment = 0;
            dir.numTables = (ushort)ListOfPackedTables.Count;
            ChangeEndian();
            Marshal.StructureToPtr(dir, position, false);
            ChangeEndian();

            IntPtr tbls = Increment(position, Marshal.SizeOf(dir));

            ArrayList descriptor_list = new ArrayList(ListOfPackedTables.Keys);
            for (int i = 0; i < descriptor_list.Count; i++)
            {
                descriptor_list[i] = SwapUInt32((uint)(int)descriptor_list[i]);
            }
            descriptor_list.Sort();
            for (int i = 0; i < descriptor_list.Count; i++)
            {
                descriptor_list[i] = (TablesID)SwapUInt32((uint)descriptor_list[i]);
            }

            foreach (TablesID tag in descriptor_list)
            {
                TrueTypeTable entry = (TrueTypeTable)ListOfPackedTables[tag];
                tbls = entry.StoreDescriptor(tbls);
            }
            descriptor_list = null;
        }

        private List<ushort> GetCmapGsubIndexes(string text)
        {
            ushort[] used_glyphs = new ushort[text.Length];
            int index = 0;
            foreach (ushort ch in text)
                used_glyphs[index++] = this.cmap_table.GetGlyphIndex(ch);

            return this.gsub_table != null ?
                gsub_table.ApplyGlyph(Script, Language, used_glyphs) :
                new List<ushort>(used_glyphs);
        }

        private List<ushort> GetCmapGsubIndexes(IList<ushort> input_glyphs)
        {
            int i = 0;
            ushort[] result = new ushort[input_glyphs.Count];
            foreach (ushort glyph in input_glyphs)
                result[i] = this.cmap_table.GetGlyphIndex(glyph);

            return this.gsub_table != null ?
                gsub_table.ApplyGlyph(Script, Language, result) :
                new List<ushort>(result);
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        // Glyph indexes is an important table which translates characters to their positions 
        // in glyph table. Table not modified at this time
        ///////////////////////////////////////////////////////////////////////////////////////
        private void BuildGlyphIndexList(
            ushort[] used_glyphs,      // Sorted list of glyphs which will stored in packed font
            bool uniscribe,             // If true then ignore CMAP table
            bool decompose,             // If true then decompose of composite glyphs
            bool collate,               // Ignores composiition width (?)
            bool use_kerning,           // Use kernig table on width calculation (for correct position next glyph)
            out ArrayList Indices,        // Translated glyph codes to their indexes
            out ArrayList GlyphWidths)    // Widths of glyphs
        {
            ushort length;
            uint location;
            Indices = new ArrayList();
            GlyphWidths = new ArrayList();

            List<ushort> glyphindeces = new List<ushort>();
            if (!uniscribe)
            {
                for (int i = 0; i < used_glyphs.Length; i++)
                {
                    used_glyphs[i] = this.cmap_table.GetGlyphIndex(used_glyphs[i]);
                }

                if (this.gsub_table != null)
                    glyphindeces = this.gsub_table.ApplyGlyph(Script, Language, used_glyphs);
                else
                    glyphindeces = new List<ushort>(used_glyphs);
            }
            else
            {
                glyphindeces = new List<ushort>(used_glyphs);
            }

            for (int i = 0; i < glyphindeces.Count; i++)
            {
                ushort idx = glyphindeces[i];

                //                if (collate) ListOfUsedGlyphs[key] = idx;

                length = this.index_to_location.GetGlyph(idx, Header, out location);
                if (length == 0)
                {
                    length = this.index_to_location.GetGlyph(0, Header, out location);
                }
                if (length != 0)
                {
                    float GlyphWidth = 0;

                    foreach (ushort composed_idx in this.glyph_table.CheckGlyph((int)location, (int)length))
                    {
                        if (!collate || !Indices.Contains(composed_idx))
                        {
                            if (decompose)
                            {
                                Indices.Add(composed_idx);
                            }
                        }
                    }
                    if (!collate || !Indices.Contains(idx))
                    {
                        Indices.Add(idx);
                        HorizontalMetrixClass.longHorMetric hm = horizontal_metrix_table[idx];
                        GlyphWidth = hm.advanceWidth;

                        // This is a correct version of glyphs width calculation. Do not change it, please
                        if (use_kerning)
                        {
                            if (i < used_glyphs.Length - 1 && this.kerning_table != null)
                            {
                                ushort next_idx = (ushort)used_glyphs[i + 1];
                                short fix_kerning = kerning_table.TryKerning(idx, next_idx);
                                if (fix_kerning != 0)
                                    GlyphWidth += (float)fix_kerning;
                            }
                        }
                        GlyphWidths.Add(GlyphWidth);
                    }
                }
                else if ((!collate || !Indices.Contains(idx)))
                {
                    Indices.Add(idx);
                    GlyphWidths.Add((float)(windows_metrix.AvgCharWidth));// * 1000 / font_header.unitsPerEm));
                }
            }
            used_glyphs = null;
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        // Repack Horizontal Header and hmtx table
        ///////////////////////////////////////////////////////////////////////////////////////
        private void RepackHorizontalMetix(ref Dictionary<ushort, GlyphChar> dict)
        {
            this.HMetrics.RepackWithDictionary(ref dict, this.MaxProfile.GlyphsCount);
            this.horizontal_header.NumberOfHMetrics = HMetrics.numberOfMetrics;
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        // Repack Glyph and pack IndexToLocation tables
        // This is a form with dictionary. Used on pack 'loca' table
        ///////////////////////////////////////////////////////////////////////////////////////
        private void PackGlyph_I2L(
            IntPtr position,
            ref Dictionary<ushort, GlyphChar> dict,
            out byte[] SavedGlyphs,
            out short[] ShortI2L,
            out int[] LongI2L)
        {
            FontHeaderClass.IndexToLoc type = Header.indexToLocFormat;
            ShortI2L = null;
            LongI2L = null;

            // Calculate packed size of Glyph table
            uint glyph_table_size = 0;
            foreach (ushort idx in dict.Keys)
            {
                uint temp_location;
                glyph_table_size += this.index_to_location.GetGlyph(idx, Header, out temp_location);
            }

            ushort length = 0;
            uint location = 0;
            int sqz_index = 0;
            uint out_index = 0;

            TrueTypeTable table_entry = (TrueTypeTable)listOfTables[TablesID.Glyph];
            IntPtr glyph_table_src_ptr = Increment(this.beginfile_ptr, (int)(table_entry.Offset));
            SavedGlyphs = new byte[glyph_table_size];
            ShortI2L = new short[dict.Count + 1];

            foreach (ushort key in dict.Keys)
            {
                GlyphChar glyph_char = dict[key];

                length = this.index_to_location.GetGlyph(key, Header, out location);

                ShortI2L[sqz_index] = SwapInt16((short)(out_index / 2));

                if (length != 0)
                {
                    IntPtr glyph_ptr = Increment(glyph_table_src_ptr, (int)location);
                    Marshal.Copy(glyph_ptr, SavedGlyphs, (int)(out_index), length);
                    out_index += length;
                }
                ++sqz_index;
            }
            ShortI2L[sqz_index] = SwapInt16((short)(out_index / 2));
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        // Repack Glyph and modify IndexToLocation tables
        ///////////////////////////////////////////////////////////////////////////////////////
        private void PackGlyphTable(
            IntPtr position,
            IList<ushort> keep_list,
            bool uniscribe,
            out byte[] SavedGlyphs,
            out short[] ShortI2L,
            out int[] LongI2L)
        {
            // Allocate new IndexToLocation table
            FontHeaderClass.IndexToLoc type = Header.indexToLocFormat;
            ShortI2L = type == FontHeaderClass.IndexToLoc.ShortType ? new short[this.index_to_location.Short.Length] : null;
            LongI2L = type == FontHeaderClass.IndexToLoc.LongType ? new int[this.index_to_location.Long.Length] : null;

            List<ushort> temp_glyphs = new List<ushort>(keep_list);
            temp_glyphs.Add((ushort)0); // Default glyph must be present
            temp_glyphs.Sort();
            ushort[] used_glyphs = temp_glyphs.ToArray();

            ArrayList composite_indexes, glyph_widths;
            BuildGlyphIndexList(used_glyphs, uniscribe, true, true, false, out composite_indexes, out glyph_widths);
            composite_indexes.Sort();

            uint glyph_table_size = 0;
            ushort length = 0;
            uint location = 0;

            // Calculate packed size of Glyph table
            foreach (ushort idx in composite_indexes)
            {
                glyph_table_size += this.index_to_location.GetGlyph(idx, Header, out location);
            }

            TrueTypeTable table_entry = (TrueTypeTable)listOfTables[TablesID.Glyph];
            IntPtr glyph_table_src_ptr = Increment(this.beginfile_ptr, (int)(table_entry.Offset));
            SavedGlyphs = new byte[glyph_table_size];

            uint out_index = 0;
            int sqz_index = 0;

            foreach (ushort i2l_idx in composite_indexes)
            {
                length = this.index_to_location.GetGlyph(i2l_idx, Header, out location);
                switch (type)
                {
                    case FontHeaderClass.IndexToLoc.ShortType:
                        for (; sqz_index <= i2l_idx; sqz_index++)
                            ShortI2L[sqz_index] = (short)(out_index / 2);
                        break;

                    case FontHeaderClass.IndexToLoc.LongType:
                        for (; sqz_index <= i2l_idx; sqz_index++)
                            LongI2L[sqz_index] = (int)out_index;
                        break;
                }

                if (length != 0)
                {
                    IntPtr glyph_ptr = Increment(glyph_table_src_ptr, (int)location);
                    Marshal.Copy(glyph_ptr, SavedGlyphs, (int)(out_index), length);
                    out_index += length;
                }
            }

            // Modify IndexToLocation table

            table_entry = (TrueTypeTable)listOfTables[TablesID.IndexToLocation];
            IntPtr i2l_ptr = Increment(this.beginfile_ptr, (int)(table_entry.Offset));
            switch (type)
            {
                case FontHeaderClass.IndexToLoc.ShortType:
                    for (; sqz_index < ShortI2L.Length; sqz_index++)
                        ShortI2L[sqz_index] = (short)(out_index / 2);
                    for (int i = 0; i < ShortI2L.Length; i++)
                    {
                        byte[] buf = BitConverter.GetBytes(ShortI2L[i]);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(buf);
                        ShortI2L[i] = BitConverter.ToInt16(buf, 0);
                    }
                    break;

                case FontHeaderClass.IndexToLoc.LongType:
                    for (; sqz_index < LongI2L.Length; sqz_index++)
                        LongI2L[sqz_index] = (int)out_index;
                    for (int i = 0; i < LongI2L.Length; i++)
                    {
                        byte[] buf = BitConverter.GetBytes(LongI2L[i]);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(buf);
                        LongI2L[i] = BitConverter.ToInt32(buf, 0);
                    }
                    break;
            }
        }


        #endregion

        #region "Public methods"

        /// <summary>
        /// Return raw image of the font
        /// </summary>
        /// <returns>Array of font bytes</returns>
        public byte[] GetRawFontData()
        {
            byte[] raw = new byte[this.calculated_file_size];
            Marshal.Copy(this.selector_ptr, raw, 0, (int)this.calculated_file_size);
            return raw;
        }

        public byte[] GetFontData()
        {
            PrepareIndexes();

            int font_size = 0;
            Hashtable ListOfPackedTables = new Hashtable();
            this.skip_CMAP_on_export = false;
            this.skip_GSUB_on_export = false;
            ArrayList indexed_tags = GetTablesOrder(out font_size);
            uint current_offset = (uint)(Marshal.SizeOf(dir) + this.dir.numTables * 16);
            font_size += (int)current_offset;
            IntPtr packed_font_ptr = Marshal.AllocHGlobal(font_size);

            foreach (TablesID tag in indexed_tags)
            {
                TrueTypeTable entry = new TrueTypeTable((TrueTypeTable)listOfTables[tag]);

                current_offset = entry.Save(this.beginfile_ptr, packed_font_ptr, current_offset);
                if ((current_offset % 4) != 0)
                    throw new Exception("Align error");
#if TRACE_ENABLED
                if (entry.debug_len != ((entry.Length + 3) & ~3))
                {
                    System.Diagnostics.Trace.Write("Length not matched");
                }
#endif
                ListOfPackedTables.Add(tag, entry);
            }
            SaveDescriptors(packed_font_ptr, ListOfPackedTables);

            FontHeaderClass new_header = new FontHeaderClass((FontHeaderClass)listOfTables[TablesID.FontHeader]);
            CalculateFontChecksum(packed_font_ptr, current_offset, new_header);

            byte[] buff = new byte[current_offset];
            Marshal.Copy(packed_font_ptr, buff, 0, (int)current_offset);
            Marshal.FreeHGlobal(packed_font_ptr);
            return buff;
        }

        /// <summary>
        /// Cut some information from TTF file to reduce its size
        /// </summary>
        /// <param name="dict">Alphabet subset dictionary/param>
        /// <param name="options">Describes how to pack font</param>
        /// <returns></returns>
        public byte[] PackFont(ref Dictionary<ushort, GlyphChar> dict, FontPackOptions options)
        {
            TrueTypeFont font = this;
            if (options == FontPackOptions.Pack_Indexes)
            {
                font = new TrueTypeFont(beginfile_ptr, selector_ptr);
                font.PrepareCommonTables();
                font.PrepareIndexes();
            }
            return font.PackFontInternal(ref dict, options);
        }

        private byte[] PackFontInternal(ref Dictionary<ushort, GlyphChar> dict, FontPackOptions options)
        {
            uint current_offset;
            byte[] Glyphs;
            short[] ShortI2L;
            int[] LongI2L;

            int calculated_original_size;
            ArrayList indexed_tags = GetTablesOrder(out calculated_original_size);

            if (options == FontPackOptions.Pack_Indexes)
            {
                PackGlyph_I2L(this.beginfile_ptr, ref dict, out Glyphs, out ShortI2L, out LongI2L);
                RepackHorizontalMetix(ref dict);
                MaxProfile.GlyphsCount = dict.Count;
            }
            else
            {
                bool uniscribe = true;
                ushort[] glyph_indexes = new ushort[dict.Keys.Count];
                dict.Keys.CopyTo(glyph_indexes, 0);
                PackGlyphTable(this.beginfile_ptr, glyph_indexes, uniscribe, out Glyphs, out ShortI2L, out LongI2L);
            }

            ///////////////////////////////////////////////////////////////////////////////////////
            // sizeof(TrueTypeFont.TableDirectory) + this.dir.numTables * sizeof(TrueTypeTable.TableEntry));
            ///////////////////////////////////////////////////////////////////////////////////////
            current_offset = (uint)(Marshal.SizeOf(dir) + indexed_tags.Count * 16);

            IntPtr packed_font_ptr = Marshal.AllocHGlobal((int)calculated_file_size);

            Hashtable ListOfPackedTables = new Hashtable();

            ///////////////////////////////////////////////////////////////////////////////////////
            // Store all tables
            ///////////////////////////////////////////////////////////////////////////////////////
            FontHeaderClass new_header = new FontHeaderClass((FontHeaderClass)listOfTables[TablesID.FontHeader]);
            foreach (TablesID tag in indexed_tags)
            {
                uint len;
                TrueTypeTable entry = null;
                IntPtr ptr = Increment(packed_font_ptr, (int)current_offset);
                switch (tag)
                {
                    case TablesID.FontHeader:
                        entry = new_header;
                        break;

                    case TablesID.Glyph:
                        entry = new TrueTypeTable((TrueTypeTable)listOfTables[tag]);
                        len = (uint)((Glyphs.Length + 3) / 4) * 4;
                        Marshal.Copy(Glyphs, 0, ptr, Glyphs.Length);
                        if (len > Glyphs.Length)
                        {
                            // Trick to keep consistent unused tail bytes for each call
                            ptr = Increment(ptr, Glyphs.Length);
                            Marshal.Copy(Glyphs, 0, ptr, (int)len - Glyphs.Length);
                        }
                        entry.Offset = current_offset;
                        entry.SetLenght(len);
                        entry.checkSum = CalcTableChecksum(true, packed_font_ptr, entry);
                        current_offset += len;
                        ListOfPackedTables.Add(tag, entry);
                        continue;

                    case TablesID.IndexToLocation:
                        entry = new IndexToLocationClass((TrueTypeTable)listOfTables[tag]);

                        if (LongI2L != null)
                        {
                            Marshal.Copy(LongI2L, 0, ptr, LongI2L.Length);
                            len = (uint)(LongI2L.Length * 4);
                            new_header.indexToLocFormat = FontHeaderClass.IndexToLoc.LongType;
                        }
                        else
                        {
                            Marshal.Copy(ShortI2L, 0, ptr, ShortI2L.Length);
                            len = (uint)(ShortI2L.Length * 2);
                            len = (uint)((len + 3) / 4) * 4;
                            new_header.indexToLocFormat = FontHeaderClass.IndexToLoc.ShortType;
                        }
                        entry.Offset = current_offset;
                        entry.SetLenght(len);
                        entry.checkSum = CalcTableChecksum(true, packed_font_ptr, entry);
                        current_offset += len;
                        ListOfPackedTables.Add(tag, entry);
                        continue;

                    case TablesID.MaximumProfile:
                        MaximumProfileClass profile = (MaximumProfileClass)listOfTables[tag];
                        current_offset = profile.Save(packed_font_ptr, current_offset);
                        if ((current_offset % 4) != 0)
                            throw new Exception("Profile align error");
                        ListOfPackedTables.Add(tag, profile);
                        continue;

                    case TablesID.HorizontalHeader:
                        HorizontalHeaderClass hor_head = (HorizontalHeaderClass)listOfTables[tag];
                        current_offset = hor_head.Save(packed_font_ptr, current_offset);
                        if ((current_offset % 4) != 0)
                            throw new Exception("Profile align error");
                        ListOfPackedTables.Add(tag, hor_head);
                        continue;

                    case TablesID.HorizontalMetrix:
                        if (Postscript != null &&  !Postscript.IsFixedPitch)
                        {
                            HorizontalMetrixClass hmetrix = (HorizontalMetrixClass)listOfTables[tag];
                            current_offset = hmetrix.Save(packed_font_ptr, current_offset);
                            if ((current_offset % 4) != 0)
                                throw new Exception("hmetrix align error");
                            ListOfPackedTables.Add(tag, hmetrix);
                            continue;
                        }
                        entry = new TrueTypeTable((TrueTypeTable)listOfTables[tag]);
                        break;

                    default:
                        entry = new TrueTypeTable((TrueTypeTable)listOfTables[tag]);
                        break;
                }
                if (entry != null)
                {
                    current_offset = entry.Save(this.beginfile_ptr, packed_font_ptr, current_offset);
                    if ((current_offset % 4) != 0)
                        throw new Exception("Align error");
                    ListOfPackedTables.Add(tag, entry);
                }
            }

            SaveDescriptors(packed_font_ptr, ListOfPackedTables);
            CalculateFontChecksum(packed_font_ptr, current_offset, new_header);
            LongI2L = null;
            ShortI2L = null;
            Glyphs = null;

            ListOfPackedTables = null;
            indexed_tags = null;
            byte[] buff = new byte[current_offset];
            Marshal.Copy(packed_font_ptr, buff, 0, (int)current_offset);

            Marshal.FreeHGlobal(packed_font_ptr);

            return buff;
        }

        /// <summary>
        /// Parse font supplement tables which store properties of the font
        /// </summary>
        public void PrepareCommonTables()
        {
            bool optimistic = true;  // Which tables copy to packed font: Optimistic - use keep list, Pessimistic - use skip list

            // --- These tables we will discard from font ---
            TablesID[] skip_list = new TablesID[]
            {
                TablesID.HorizontakDeviceMetrix,
                TablesID.DigitalSignature,
                TablesID.GlyphPosition,
                TablesID.EmbedBitmapLocation,
                TablesID.EmbededBitmapData
            };

            // --- These tables we will keep in font ---
            TablesID[] keep_list = new TablesID[]
            {
            TablesID.FontHeader,
            TablesID.BitmapFontHeader,
            TablesID.CMAP,
            TablesID.Glyph,
            TablesID.IndexToLocation,
            TablesID.MaximumProfile,
            TablesID.HorizontalHeader,
            TablesID.HorizontalMetrix,
            TablesID.OS2Table,
            TablesID.Name,
            TablesID.Postscript,
            TablesID.GlyphSubstitution,
            TablesID.KerningTable,
            /* --- No problem found if skip these two tables */
//            TablesID.HorizontakDeviceMetrix,
//            TablesID.VerticalMetrix,
            /* --- Related to font programm. Do we need keep these tables? */
            TablesID.PreProgram,
            TablesID.FontProgram,
            TablesID.ControlValueTable,
                //            TablesID.DigitalSignature,
                //            TablesID.GridFittingAndScanConversion,
                //            TablesID.GlyphDefinition,
                //            TablesID.GlyphPosition,
                //            TablesID.LinearThreshold,
                //            TablesID.Justification,
                //            TablesID.PCL5Table,
                //            TablesID.VertivalMetrixHeader,
                //            TablesID.VerticalDeviceMetrix,
                /* Bitmap tables are optional */
                //            TablesID.EmbedBitmapLocation,
                //            TablesID.EmbededBitmapData,
                //            TablesID.Metadata,  // Must keep for OpenTypes fonts
            };

            ArrayList tables;
            if (optimistic)
                tables = new ArrayList(keep_list);
            else
                tables = new ArrayList(skip_list);

            try
            {
                lock (this.listOfTables)
                {
                    LoadDescriptors(optimistic, tables);
                    LoadCoreTables();
                    CheckTablesChecksum();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Unable parse font: " + e.Message);
            }
            tables = null;
        }

#if false
        /// <summary>
        /// Get unique font identifier
        /// </summary>
        /// <returns></returns>
        public string GetHashKey()
        {
            string result = null;
            if (name_table == null)
            {
                dir = (TableDirectory)Marshal.PtrToStructure(this.selector_ptr, typeof(TableDirectory));
                ChangeEndian();

                IntPtr tbls = Increment(selector_ptr, Marshal.SizeOf(dir));

                for (int i = 0; i < dir.numTables; i++)
                {
                    TrueTypeTable table = new TrueTypeTable(tbls);

                    if (((TablesID)table.tag) == TablesID.Name)
                    {
                        this.name_table = new NameTableClass(table);
                        this.name_table.Load(beginfile_ptr/*selector_ptr*/);
                    }

                    if (((TablesID)table.tag) == TablesID.FontHeader)
                    {
                        this.font_header = new FontHeaderClass(table);
                        this.Header.Load(beginfile_ptr/*selector_ptr*/);
                    }

                    tbls = Increment(tbls, table.descriptor_size);
                }
                dir.numTables = (ushort)listOfTables.Count;

                if (this.name_table != null)
                {
                    //result = parsed_table[NameTableClass.NameID.FamilyName];
                    //if (0 != (header_table.MacStyle & 1)) result += "_Bold";
                    //if (0 != (header_table.MacStyle & 2)) result += "_Italic";
                    result = this.name_table[NameTableClass.NameID.UniqueID].ToString();
                }
            }
            return result;
        }
#endif
        /// <summary>
        /// Create glyph outline assigned to specific position
        /// </summary>
        /// <param name="ch">unicode character which will be drawn</param>
        /// <param name="size">size of the character</param>
        /// <param name="position">position of outline</param>
        /// <returns>outline of character</returns>
        public FastGraphicsPath GetGlyph(char ch, int size, PointF position)
        {
            GlyphTableClass.GlyphHeader gheader;
            uint location;
            ushort i2l_idx = this.cmap_table.GetGlyphIndex((ushort)ch);
            ushort length = this.index_to_location.GetGlyph(i2l_idx, Header, out location);

            float rsize = (float)this.Header.unitsPerEm / size;

            return this.glyph_table.GetGlyph((int)location, length, rsize, position, this.index_to_location, Header, out gheader);
        }

        /// <summary>
        /// Get glyph's outline
        /// </summary>
        /// <param name="ch">unicode charter</param>
        /// <param name="size">outline image size</param>
        /// <returns></returns>
        public FastGraphicsPath GetGlyph(ushort ch, float size)
        {

            uint location;
            if (index_to_location == null)
                PrepareIndexes();

            ushort length = this.index_to_location.GetGlyph(ch, this.Header, out location);
            float rsize = (float)this.Header.unitsPerEm / size * 0.75f;

            GlyphTableClass.GlyphHeader gheader;
            FastGraphicsPath aPath = this.glyph_table.GetGlyph((int)location, length, rsize, Point.Empty, this.index_to_location, Header, out gheader);
            return aPath;
        }

#endregion

        /// <summary>
        /// Constructor of TrueTypeFont object
        /// </summary>
        /// <param name="begin">The start of RAW image of font or font collection</param>
        /// <param name="font">Actual address of font within RAW image</param>
        public TrueTypeFont(IntPtr begin, IntPtr font)
        {
            beginfile_ptr = begin;
            selector_ptr = font;
            checksum_action = ChecksumFaultAction.IgnoreChecksum; //.ThrowException;
            listOfTables = new Hashtable();
        }

#region Font Structures
        /// <summary>
        /// Description of SCRIPT_STATE structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SCRIPT_STATE
        {
            /// <summary>
            /// data
            /// </summary>
            public short data;
            /// <summary>
            /// uBidiLevel
            /// </summary>
            public int uBidiLevel
            {
                get { return data & 0x001F; }
            }
            /// <summary>
            /// SetRtl
            /// </summary>
            public void SetRtl()
            {
                data = 0x801;
            }
        }

        /// <summary>
        /// Description of SCRIPT_ANALYSIS structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SCRIPT_ANALYSIS
        {
            /// <summary>
            /// data
            /// </summary>
            public short data;
            /// <summary>
            /// state
            /// </summary>
            public SCRIPT_STATE state;
        }

        /// <summary>
        /// Description of SCRIPT_CONTROL structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SCRIPT_CONTROL
        {
            /// <summary>
            /// data
            /// </summary>
            public int data;
        }

        /// <summary>
        /// Description of SCRIPT_DIGITSUBSTITUTE structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SCRIPT_DIGITSUBSTITUTE
        {
            /// <summary>
            /// NationalDigitLanguage
            /// </summary>
            public short NationalDigitLanguage;
            /// <summary>
            /// TraditionalDigitLanguage
            /// </summary>
            public short TraditionalDigitLanguage;
            /// <summary>
            /// DigitSubstitute
            /// </summary>
            public byte DigitSubstitute;
            /// <summary>
            /// dwReserved
            /// </summary>
            public int dwReserved;
        }

        /// <summary>
        /// Description of SCRIPT_ITEM structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SCRIPT_ITEM
        {
            /// <summary>
            /// iCharPos
            /// </summary>
            public int iCharPos;
            /// <summary>
            /// analysis
            /// </summary>
            public SCRIPT_ANALYSIS analysis;
        }

        /// <summary>
        /// Description of SCRIPT_VISATTR structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SCRIPT_VISATTR
        {
            /// <summary>
            /// data
            /// </summary>
            public short data;
        }

        /// <summary>
        /// Description of GOFFSET structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct GOFFSET
        {
            /// <summary>
            /// du
            /// </summary>
            public int du;
            /// <summary>
            /// dv
            /// </summary>
            public int dv;
        }

        /// <summary>
        /// Description of ABC structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ABC
        {
            /// <summary>
            /// abcA
            /// </summary>
            public int abcA;
            /// <summary>
            /// abcB
            /// </summary>
            public int abcB;
            /// <summary>
            /// abcC
            /// </summary>
            public int abcC;
        }

        /// <summary>
        /// Description of FontRect structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct FontRect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        /// <summary>
        /// Description of FontPoint structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct FontPoint
        {
            public int x;
            public int y;
        }

        /// <summary>
        /// Description of OutlineTextMetric structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct OutlineTextMetric
        {
            public uint otmSize;
            public OS2WindowsMetricsClass.FontTextMetric otmTextMetrics;
            public byte otmFiller;
            public OS2WindowsMetricsClass.FontPanose otmPanoseNumber;
            public uint otmfsSelection;
            public uint otmfsType;
            public int otmsCharSlopeRise;
            public int otmsCharSlopeRun;
            public int otmItalicAngle;
            public uint otmEMSquare;
            public int otmAscent;
            public int otmDescent;
            public uint otmLineGap;
            public uint otmsCapEmHeight;
            public uint otmsXHeight;
            public FontRect otmrcFontBox;
            public int otmMacAscent;
            public int otmMacDescent;
            public uint otmMacLineGap;
            public uint otmusMinimumPPEM;
            public FontPoint otmptSubscriptSize;
            public FontPoint otmptSubscriptOffset;
            public FontPoint otmptSuperscriptSize;
            public FontPoint otmptSuperscriptOffset;
            public uint otmsStrikeoutSize;
            public int otmsStrikeoutPosition;
            public int otmsUnderscoreSize;
            public int otmsUnderscorePosition;
            public string otmpFamilyName;
            public string otmpFaceName;
            public string otmpStyleName;
            public string otmpFullName;
        }
#endregion

        /// <summary>
        /// Emulation of Uniscribe GetOutlineTextMetrics
        /// </summary>
        /// <param name="TextMetric">Reference to metric structure</param>
        public void GetOutlineTextMetrics(ref OutlineTextMetric TextMetric)
        {
#region With hope that these options translated 
            TextMetric.otmTextMetrics.tmHeight = windows_metrix.Ascent + windows_metrix.Descent;
            TextMetric.otmTextMetrics.tmAscent = windows_metrix.Ascent;
            TextMetric.otmTextMetrics.tmDescent = windows_metrix.Descent;
            TextMetric.otmTextMetrics.tmAveCharWidth = this.windows_metrix.AvgCharWidth;
            TextMetric.otmTextMetrics.tmMaxCharWidth = this.horizontal_header.MaxWidth;
            TextMetric.otmTextMetrics.tmWeight = this.windows_metrix.Weight;
            TextMetric.otmTextMetrics.tmFirstChar = (char)this.windows_metrix.FirstCharIndex;
            TextMetric.otmTextMetrics.tmLastChar = (char)this.windows_metrix.LastCharIndex;
            TextMetric.otmTextMetrics.tmDefaultChar = (char)this.windows_metrix.DefaultChar;
            TextMetric.otmTextMetrics.tmBreakChar = (char)this.windows_metrix.BreakChar;
            TextMetric.otmTextMetrics.tmItalic = (byte)((Header.MacStyle & 0x2) != 0 ? 35 : 0);
            TextMetric.otmPanoseNumber = this.windows_metrix.Panose;
            TextMetric.otmItalicAngle = (this.Postscript.ItalicAngle >> 16) * 10; // Simplified version
            TextMetric.otmEMSquare = this.Header.unitsPerEm;
            TextMetric.otmAscent = this.horizontal_header.Ascender;
            TextMetric.otmDescent = this.horizontal_header.Descender;
            TextMetric.otmLineGap = (this.horizontal_header.LineGap > 0) ? (uint)this.horizontal_header.LineGap : 0;
            TextMetric.otmsCapEmHeight = (uint)(this.Header.yMax - this.Header.yMin) / TextMetric.otmEMSquare; // 500
            TextMetric.otmrcFontBox = new FontRect();
            TextMetric.otmrcFontBox.top = this.Header.yMax;
            TextMetric.otmrcFontBox.left = this.Header.xMin;
            TextMetric.otmrcFontBox.right = this.Header.xMax;
            TextMetric.otmrcFontBox.bottom = this.Header.yMin;
            IList o = Names[NameTableClass.NameID.FamilyName] as IList;
            TextMetric.otmpFamilyName = o[0] as string;
            o = Names[NameTableClass.NameID.UniqueID] as IList;
            TextMetric.otmpFaceName = o[0] as string;
            o = Names[NameTableClass.NameID.SubFamilyName] as IList;
            TextMetric.otmpStyleName = o[0] as string;
            o = Names[NameTableClass.NameID.FullName] as IList;
            TextMetric.otmpFullName = o[0] as string;
#endregion

#region These options set to some almost random values
            TextMetric.otmSize = 212; // 
            TextMetric.otmFiller = 210;
            TextMetric.otmfsSelection = 64;
            TextMetric.otmfsType = 8;
            TextMetric.otmsCharSlopeRise = 1;
            TextMetric.otmsCharSlopeRun = 0;
            TextMetric.otmsXHeight = 250;
            TextMetric.otmMacAscent = 905;
            TextMetric.otmMacDescent = -212;
            TextMetric.otmMacLineGap = 33;
            TextMetric.otmusMinimumPPEM = 9;
            TextMetric.otmptSubscriptSize = new FontPoint();
            TextMetric.otmptSubscriptSize.x = 700;
            TextMetric.otmptSubscriptSize.y = 650;
            TextMetric.otmptSubscriptOffset = new FontPoint();
            TextMetric.otmptSubscriptOffset.x = 0;
            TextMetric.otmptSubscriptOffset.y = 138;
            TextMetric.otmptSuperscriptSize = new FontPoint();
            TextMetric.otmptSuperscriptSize.x = 700;
            TextMetric.otmptSuperscriptSize.y = 650;
            TextMetric.otmptSuperscriptOffset = new FontPoint();
            TextMetric.otmptSuperscriptOffset.x = 0;
            TextMetric.otmptSuperscriptOffset.y = 477;
            TextMetric.otmsStrikeoutSize = 50;
            TextMetric.otmsStrikeoutPosition = 259;
            TextMetric.otmsUnderscoreSize = 73;
            TextMetric.otmsUnderscorePosition = -106;
#endregion

#region These options still uninitialized
            //          public int tmInternalLeading;
            //          public int tmExternalLeading;
            //          TextMetric.otmTextMetrics.tmOverhang = this.windows_metrix.;
            //          public int tmDigitizedAspectX;
            //          public int tmDigitizedAspectY;
            //          public byte tmUnderlined;
            //          public byte tmStruckOut;
            //          public byte tmPitchAndFamily;
            //          public byte tmCharSet;
#endregion
        }

        /// <summary>
        /// Translate text to positions of glyphs in glyph tables and glyphs width
        /// </summary>
        /// <param name="text"></param>
        /// <param name="font_size">size in px</param>
        /// <param name="glyphs"></param>
        /// <param name="widths"></param>
        /// <param name="rtl"></param>
        /// <returns></returns>
        public int GetGlyphIndices(string text, float font_size, out ushort[] glyphs, out float[] widths, bool rtl)
        {
            PrepareIndexes();

            int i;
            List<ushort> used_glyphs_list = GetCmapGsubIndexes(text);
            glyphs = used_glyphs_list.ToArray();

            if (this.index_to_location == null || this.glyph_table == null)
            {
                widths = new float[glyphs.Length];
                HorizontalMetrixClass.longHorMetric hm;
                for (i = 0; i < widths.Length; ++i)
                {
                    int glyph = glyphs[i];

                    if (horizontal_metrix_table != null && horizontal_metrix_table.Length > glyph)
                    {
                        hm = horizontal_metrix_table[glyph];
                        float GlyphWidth = hm.advanceWidth;

                        // (use_kerning)
                        if (i < glyphs.Length - 1 && this.kerning_table != null)
                        {
                            ushort next_idx = glyphs[i + 1];
                            short kerning = this.kerning_table.TryKerning((ushort)glyph, next_idx);
                            if (kerning != 0)
                                GlyphWidth += (float)kerning; // * 1000 / font_header.unitsPerEm;
                        }
                        widths[i] = GlyphWidth;
                    }
                    else if (OS2WindowsMetrics != null)
                    {
                        widths[i] = OS2WindowsMetrics.AvgCharWidth;
                    }
                    else
                    {
                        // Last attempt (last)
                        widths[i] = this.horizontal_header.MaxWidth;
                    }
                }
            }
            else
            {
                ArrayList results;
                ArrayList text_widths;
                BuildGlyphIndexList(glyphs, true, false, false, true, out results, out text_widths);
                widths = (float[])text_widths.ToArray(typeof(float));
                float rsize = (float)this.Header.unitsPerEm / font_size;
                for (i = 0; i < widths.Length; i++)
                    widths[i] = widths[i] / rsize;
            }

            if (rtl)
            {
                // Custom Reverse
                ushort[] rvs_glyphs = new ushort[glyphs.Length];
                float[] rvs_widths = new float[widths.Length];
                List<RtlDigits> digits = new List<RtlDigits>();

                for (i = 0; i < glyphs.Length; i++)
                {
                    ushort glyph = glyphs[i];
                    float width = widths[i];
                    if (glyph < 29 && glyph > 18)   // IsDigit
                        digits.Add(new RtlDigits(glyph, width, i));
                    else
                    {
                        rvs_glyphs[glyphs.Length - i - 1] = glyph;
                        rvs_widths[widths.Length - i - 1] = width;
                    }
                }

                int start_shift = 0;
                for (i = 0; i < digits.Count; i++)
                {
                    int shift = 1;
                    while (i + shift < digits.Count && digits[i + shift].index == digits[i].index + shift)
                        shift++;

                    rvs_glyphs[glyphs.Length - digits[i].index - shift + start_shift] = digits[i].glyph;
                    rvs_widths[widths.Length - digits[i].index - shift + start_shift] = digits[i].width;

                    if (shift != 1)
                        start_shift++;
                    else
                        start_shift = 0;
                }

                glyphs = rvs_glyphs;
                widths = rvs_widths;
            }
            return glyphs.Length;
        }

        private class RtlDigits
        {
            internal readonly ushort glyph;
            internal readonly float width;
            internal readonly int index;

            internal RtlDigits(ushort glyph, float width, int index)
            {
                this.glyph = glyph;
                this.index = index;
                this.width = width;
            }
        }

#region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: free managed objects (if exist)
                }
                disposedValue = true;
            }
        }

        /// <summary>
        /// Destructor of TrueTypeFont object
        /// </summary>
        ~TrueTypeFont()
        {
            Dispose(false);
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            // TODO: раскомментировать следующую строку, если метод завершения переопределен выше.
            GC.SuppressFinalize(this);
        }
#endregion
    }
}
#pragma warning restore