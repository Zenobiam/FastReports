using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using FastReport.Utils;
using FastReport.Data;
using FastReport.Design.ImportPlugins;

namespace FastReport.Design
{
    internal class DesignerClipboard
    {
        private readonly Designer designer;

        public bool CanPaste
        {
            get
            {
                try
                {
                    return Clipboard.ContainsData("FastReport") || Clipboard.ContainsText() || Clipboard.ContainsImage();
                }
                catch
                {
                    return false;
                }
            }
        }

        public void Cut()
        {
            Copy();
            designer.cmdDelete.Invoke();
        }

        public void Copy()
        {
            // Things that we should consider:
            // - when copy an object that is selected, we must save its absolute position instead of relative Top, Left.
            //   This is necessary when copying several objects that have different parents.
            // - when copy a parent object and its child, and both are selected, we must exclude a child from the 
            //   selected objects. This is necessary to avoid duplicate copy of the child object - it will be
            //   copied by its parent.

            if (designer.SelectedObjects == null)
                return;
            using (ClipboardParent parent = new ClipboardParent())
            {
                Hashtable bounds = new Hashtable();
                parent.PageType = designer.SelectedObjects[0].Page.GetType();
                foreach (Base c in designer.Objects)
                {
                    if (c.IsSelected)
                    {
                        if (c.HasFlag(Flags.CanCopy))
                        {
                            if (!parent.Contains(c))
                            {
                                if (c is ComponentBase)
                                {
                                    ComponentBase c1 = c as ComponentBase;
                                    bounds[c1] = c1.Bounds;
                                    c1.SetDesigning(false);
                                    c1.Left = c1.AbsLeft;
                                    c1.Top = c1.AbsTop;
                                }
                                parent.Objects.Add(c);
                            }
                        }
                    }
                }
                using (MemoryStream stream = new MemoryStream())
                using (FRWriter writer = new FRWriter())
                {
                    writer.SerializeTo = SerializeTo.Clipboard;
                    writer.Write(parent);
                    writer.Save(stream);
                    Clipboard.SetData("FastReport", stream);
                }

                // restore components' state
                foreach (Base c in parent.Objects)
                {
                    if (c is ComponentBase)
                    {
                        ComponentBase c1 = c as ComponentBase;
                        if (bounds[c1] != null)
                            c1.Bounds = (RectangleF)bounds[c1];
                        c1.SetDesigning(true);
                    }
                }
            }
        }

        public void Paste()
        {
            if (Clipboard.ContainsData("FastReport"))
            {
                using (ClipboardParent parent = new ClipboardParent())
                using (MemoryStream stream = Clipboard.GetData("FastReport") as MemoryStream)
                using (FRReader reader = new FRReader(null))

                {
                    reader.DeserializeFrom = SerializeTo.Clipboard;
                    reader.Load(stream);
                    reader.Read(parent);

                    PageBase page = designer.ActiveReportTab.ActivePage;
                    if (page.GetType() == parent.PageType)
                    {
                        designer.ActiveReportTab.ActivePageDesigner.Paste(parent.Objects, InsertFrom.Clipboard);
                    }
                }
            }
            if (Clipboard.ContainsText())
            {
                ObjectCollection list = new ObjectCollection();
                TextObject obj = new TextObject();
                obj.Text = Clipboard.GetText();
                list.Add(obj);
                designer.ActiveReportTab.ActivePageDesigner.Paste(list, InsertFrom.NewObject);
                return;
            }

            if (Clipboard.ContainsImage())
            {
                ObjectCollection list = new ObjectCollection();
                PictureObject pic = new PictureObject();
                Image x = Clipboard.GetImage();
                pic.Image = x;
                pic.Width = x.Width;
                pic.Height = x.Height;
                list.Add(pic);
                designer.ActiveReportTab.ActivePageDesigner.Paste(list, InsertFrom.NewObject);
                return;
            }
        }

        public DesignerClipboard(Designer designer)
        {
            this.designer = designer;
        }

        static DesignerClipboard()
        {
            RegisteredObjects.Add(typeof(ClipboardParent), "", 0);
        }


        private class ClipboardParent : Base
        {
            private ObjectCollection objects;
            private Type pageType;

            public ObjectCollection Objects
            {
                get { return objects; }
            }

            public new ObjectCollection AllObjects
            {
                get
                {
                    ObjectCollection result = new ObjectCollection();
                    foreach (Base c in Objects)
                        EnumObjects(c, result);
                    return result;
                }
            }

            public Type PageType
            {
                get { return pageType; }
                set { pageType = value; }
            }

            private void EnumObjects(Base c, ObjectCollection list)
            {
                if (c != this)
                    list.Add(c);
                foreach (Base obj in c.ChildObjects)
                    EnumObjects(obj, list);
            }


            public bool Contains(Base obj)
            {
                return AllObjects.Contains(obj);
            }

            public override void Assign(Base source)
            {
                BaseAssign(source);
            }

            public override void Serialize(FRWriter writer)
            {
                writer.ItemName = Name;
                writer.WriteStr("PageType", pageType.Name);
                foreach (Base c in objects)
                {
                    writer.Write(c);
                }
            }

            public override void Deserialize(FRReader reader)
            {
                pageType = RegisteredObjects.FindType(reader.ReadStr("PageType"));
                while (reader.NextItem())
                {
                    Base c = reader.Read() as Base;
                    if (c != null)
                        objects.Add(c);
                }
            }

            public ClipboardParent()
            {
                objects = new ObjectCollection();
                Name = "ClipboardParent";
            }
        }
    }
}
