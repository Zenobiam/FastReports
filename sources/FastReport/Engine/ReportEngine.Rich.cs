using System.Collections.Generic;

namespace FastReport.Engine
{
    partial class ReportEngine
    {
        private List<RichObject> listOfRichObjects = new List<RichObject>();

        // Some objects may be transformed here
        private void PreprocessTemplate()
        {
            Base parent;
            float height;
            foreach(RichObject rich in listOfRichObjects)
            {
                parent = rich.Parent;
                List<ComponentBase> clone_list = rich.Convert2ReportObjects(out height);
                int position = parent.ChildObjects.IndexOf(rich);
                if(position != -1)
                {
                    (parent as IParent).RemoveChild(rich);
                    BandBase bandBase = parent as BandBase;
                    bandBase.CanBreak = true;
                    float bottom = bandBase.Bottom;
                    foreach (ComponentBase c in clone_list)
                    {
                        c.Left = rich.Left;
                        c.Top += rich.Top;
                        (parent as IParent).AddChild(c);
                        bottom = c.Bottom;
                    }
                }
            }
            listOfRichObjects.Clear();
        }

        private void AnalyzeTemplate(Base source, Base parent)
        {
            if (source is RichObject)
            {
                RichObject rich = source as RichObject;
                if (rich.ConvertRichText == true)
                {
                    if(rich.Text != null && rich.Text.StartsWith(@"{\rtf1"))
                        listOfRichObjects.Add(rich);
                }
            }
            foreach (Base c in source.ChildObjects)
            {
                AnalyzeTemplate(c, source);
            }
        }

        private void InitializePages()
        {
            listOfRichObjects.Clear();

            for (int i = 0; i < Report.Pages.Count; i++)
            {
                ReportPage page = Report.Pages[i] as ReportPage;
                if (page != null)
                {
                    AnalyzeTemplate(page, null);
                    if (listOfRichObjects.Count > 0)
                        PreprocessTemplate();
                    PreparedPages.AddSourcePage(page);
                }
            }
        }

        // This code splits RichObject to report objects
        // TODO: eleminate similar functions across all sources. This function must be single place for translation
        private void TranslateObjects(BandBase parentBand)
        {
            int originalObjectsCount = parentBand.Objects.Count;
            float shift = 0;
            float height, bottom = 0;
            for (int i = 0; i < originalObjectsCount; i++)
            {
                ComponentBase obj = parentBand.Objects[i];
                obj.Top += shift;
                if (obj is RichObject)
                {
                    RichObject rich = obj as RichObject;
                    if (rich.Visible && rich.ConvertRichText)
                    {
                        List<ComponentBase> clone_list = rich.Convert2ReportObjects(out height);
                        {
                            rich.Visible = false;
                            foreach (ComponentBase c in clone_list)
                            {
                                c.Left = rich.Left;
                                c.Top += rich.Top;
                                parentBand.AddChild(c);
                                bottom = c.Bottom;
                            }
                        }
                    }
                    shift = bottom - rich.Height;
                }
            }
        }

    }
}