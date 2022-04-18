
namespace FastReport.Fonts.LinqExts
{
    internal static class ExtFor2005
    {
        #region Public Methods

        public static bool Contains(GlyphSubstitutionClass.LookupTypes[] arr, GlyphSubstitutionClass.LookupTypes value)
        {
            for (int i = 0; i < arr.Length; i++)
                if (arr[i] == value)
                    return true;
            return false;
        }

        #endregion Public Methods
    }
}