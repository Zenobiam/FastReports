using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using FastReport.Utils;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Controls
{
    internal class FlagsControl : CheckedListBox
    {
        private Type enumType;
        private bool allowMultipleFlags;

        public Enum Flags
        {
            get
            {
                string s = "";
                foreach (object o in CheckedItems)
                {
                    s += (string)o + ", ";
                }
                if (s != "")
                {
                    s = s.Remove(s.Length - 2);
                    return (Enum)Enum.Parse(enumType, s);
                }
                return (Enum)Enum.ToObject(enumType, 0);
            }
            set
            {
                enumType = value.GetType();
                string[] names = Enum.GetNames(enumType);
                Array values = Enum.GetValues(enumType);
                int enumValue = (int)Enum.ToObject(enumType, value);

                float maxWidth = 0;
                for (int i = 0; i < names.Length; i++)
                {
                    int val = (int)values.GetValue(i);
                    if (val != 0 && (val & 3) != 3)
                    {
                        Items.Add(names[i]);
                        SetItemChecked(Items.Count - 1, (enumValue & val) != 0);
                        float itemWidth = DrawUtils.MeasureString(names[i]).Width;
                        if (itemWidth > maxWidth)
                            maxWidth = itemWidth;
                    }
                }

                Width = (int)maxWidth + 20;
                Height = (Items.Count + 1) * (DrawUtils.DefaultItemHeight + 1);
            }
        }

        internal bool AllowMultipleFlags
        {
            get { return allowMultipleFlags; }
            set { allowMultipleFlags = value; }
        }

        protected override void OnItemCheck(ItemCheckEventArgs ice)
        {
            base.OnItemCheck(ice);
            if(!allowMultipleFlags && ice.NewValue == CheckState.Checked)
            {
                foreach (int i in CheckedIndices)
                    SetItemChecked(i, false);
            }
        }

        public FlagsControl()
        {
            BorderStyle = BorderStyle.None;
            Font = DpiHelper.ConvertUnits(DrawUtils.DefaultFont, true);
            CheckOnClick = true;
            allowMultipleFlags = true;
        }
    }
}
