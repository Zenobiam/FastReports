namespace FastReport.Fonts
{
    /// <summary>
    /// Define type of font file
    /// </summary>
    public enum FontType
    {
        /// <summary>
        /// Classic TrueType font
        /// </summary>
        TrueTypeFont = 0x00000000,
        /// <summary>
        /// Collection of TrueType fonts
        /// </summary>
        TrueTypeCollection = 0x66637474,
        /// <summary>
        /// OpenType font format
        /// </summary>
        OpenTypeFont = 0x4f54544f,
        ///
        UnknownFontType = 0x7fffffff
    }

}