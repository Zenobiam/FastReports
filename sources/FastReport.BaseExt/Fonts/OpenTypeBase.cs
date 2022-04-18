using System;
using System.Collections;
using System.Runtime.InteropServices;

#pragma warning disable CS3001, CS3002, CS3003, CS1591

namespace FastReport.Fonts
{
#if false
    /// <summary>
    /// Interface for TrueTypeFont object
    /// </summary>
    public interface ITrueTypeFont
    {
        bool IsClassicFormat { get; }
        string Name { get; }
        FontType Type { get; }
        bool Bold { get; set; }
        bool Italic { get; set; }
        Int32 LicensingRights { get; }
        bool IsCollection { get; }
        string FastName { get; }
        ICollection GetNames(NameTableClass.NameID id);
        ICollection TablesList { get; }
        bool Prepare();
        FastGraphicsPath DrawString(string text, PointF position, float size);
        byte[] GetRawFontData();
        byte[] PackFont(IList<ushort> keep_list, bool uniscribe);
    }
#endif
    /// <summary>
    /// Font header
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct TableDirectory
    {
        [FieldOffset(0)]
        public uint sfntversion;     // 4 bytes
        [FieldOffset(4)]
        public ushort numTables;       // 2 bytes
        [FieldOffset(6)]
        public ushort searchRange;    // 2 bytes
        [FieldOffset(8)]
        public ushort entrySelector;  // 1 byte
        [FieldOffset(10)]
        public ushort rangeShift;
    }

    public enum TablesID
    {
        FontHeader = 0x64616568,
        CMAP = 0x70616D63,
        Glyph = 0x66796c67,
        IndexToLocation = 0x61636f6c,
        MaximumProfile = 0x7078616d,
        HorizontalHeader = 0x61656868,
        HorizontalMetrix = 0x78746d68,
        OS2Table = 0x322f534f,
        Name = 0x656d616e,
        Postscript = 0x74736f70,

        GlyphSubstitution = 0x42555347,
        PreProgram = 0x70657270,
        HorizontakDeviceMetrix = 0x786d6468,
        ControlValueTable = 0x20747663,
        DigitalSignature = 0x47495344,
        GridFittingAndScanConversion = 0x70736167,
        GlyphDefinition = 0x46454447,
        FontProgram = 0x6d677066,
        GlyphPosition = 0x534f5047,
        LinearThreshold = 0x4853544c,
        /* Found in Arial */
        Justification = 0x4654534a,
        VerticalDeviceMetrix = 0x584d4456,
        PCL5Table = 0x544c4350,
        KerningTable = 0x6e72656b,
        /* Found in Gulim */
        VertivalMetrixHeader = 0x61656876,
        VerticalMetrix = 0x78746d76,
        EmbedBitmapLocation = 0x434c4245,
        EmbededBitmapData = 0x54444245,
        /* Modern spec */
        Metadata = 0x6174656d,
        CompactFontFormat = 0x20464643,
        /* Apple Extension */
        BitmapFontHeader = 0x64656862,
        BitmapLocation = 0x636f6c62,
        BitmapData = 0x74616462,
    }


    public class OpenTypeFontBase : TTF_Helpers
    {
        internal IntPtr beginfile_ptr;
        internal TableDirectory dir;                // List of tables of font
        internal Hashtable listOfTables;
        // These objects represent some internal tables within font
        private FontHeaderClass font_header;
        private NameTableClass name_table;
        private MaximumProfileClass profile;
        private PostScriptClass postscript;
        private string font_name;
        public ICollection TablesList { get { return listOfTables.Values; } }
        public FontHeaderClass Header { get { return font_header; } }
        public NameTableClass Names { get { return name_table; } }
        public MaximumProfileClass MaxProfile { get { return profile; } }
        public PostScriptClass Postscript { get { return postscript; } }
        public ICollection GetNames(NameTableClass.NameID id) { return name_table[id]; }
        public string Name { get { return font_name; } }

        ///////////////////////////////////////////////////////////////////////////////////////
        // Build font name by typographic rules
        ///////////////////////////////////////////////////////////////////////////////////////
        protected void PrepareFontName()
        {
            string result = null;

            if (Names.NamesDict.ContainsKey(NameTableClass.NameID.PreferredFamily) && Names.NamesDict.ContainsKey(NameTableClass.NameID.PreferredSubFamily))
            {
                foreach (object o in Names[NameTableClass.NameID.PreferredFamily])
                {
                    result = o as string;
                    break;
                }
                foreach (object o in Names[NameTableClass.NameID.PreferredSubFamily])
                {
                    string subfamily = o as string;
                    int index = subfamily.IndexOf(" ");
                    if (index > 0)
                        subfamily = subfamily.Substring(0, index);
                    result += " " + subfamily;
                    break;
                }
            }
            else
            {
                foreach (object o in Names[NameTableClass.NameID.FamilyName])
                {
                    result = o as string;
                    break;
                }
            }
            font_name = result;
        }

        internal bool LoadBaseTables()
        {
            bool bitmap = false;
            if (listOfTables.ContainsKey(TablesID.FontHeader))
            {
                font_header = (FontHeaderClass) listOfTables[TablesID.FontHeader];
                font_header.Load(this.beginfile_ptr);
            }
            else if (listOfTables.ContainsKey(TablesID.BitmapFontHeader))
            {
                font_header = (FontHeaderClass) listOfTables[TablesID.BitmapFontHeader];
                Header.Load(this.beginfile_ptr);
                bitmap = true;
            }
            else throw new Exception("FontHeader not found."); ;

            if (!listOfTables.ContainsKey(TablesID.Name)) throw new Exception("Name not found."); ;
            name_table = (NameTableClass)listOfTables[TablesID.Name];
            name_table.Load(this.beginfile_ptr);
            PrepareFontName();

            if (listOfTables.ContainsKey(TablesID.MaximumProfile))
            {
                profile = (MaximumProfileClass)listOfTables[TablesID.MaximumProfile];
                profile.Load(this.beginfile_ptr);
            }
            else throw new Exception("MaximuProfile not found.");

            if (listOfTables.ContainsKey(TablesID.Postscript))
            {
                postscript = (PostScriptClass)listOfTables[TablesID.Postscript];
                postscript.Load(this.beginfile_ptr);
            }

            return bitmap;
        }

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

    }

}
