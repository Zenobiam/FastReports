using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Utils;

namespace FastReport.Preview
{
  internal partial class SourcePages
  {
        private Base RichObjectTranslation(Base source, Base parent)
        {
            // RichObject transformed at source pages if ConvertRichText is true
            if (source is RichObject)
            {
                RichObject rich = source as RichObject;
                if (rich.ConvertRichText)
                {
                    Base translated = TranslateRichObjectToBand(rich);
                    CloneObjects(translated, parent);
                    translated.Parent = parent;
                    return translated;
                }
                else return null;
            }
            else return null;
        }

        private Base TranslateRichObjectToBand(RichObject rich)
        {
            ChildBand band = new ChildBand();
            band.BaseName = "rt";
            band.Name = rich.Name + "_1";
            band.Border = rich.Border;
            band.Fill = rich.Fill;
            band.Height = rich.Height;
            band.SetReport(rich.Report);
            band.CanShrink = rich.CanShrink;
            band.CanGrow = rich.CanGrow;
            band.CanBreak = rich.CanBreak;
            band.Width = rich.Width;
            band.Top = rich.Top;

            float height;
            List<ComponentBase> clone_list = rich.Convert2ReportObjects(out height);
            foreach (ComponentBase obj in clone_list)
            {
                obj.Left += rich.Left;
                obj.Parent = band;
                obj.CreateUniqueName();
            }

            return band;
        }

    }
}
