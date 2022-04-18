using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.PropertyGridInternal;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.Drawing.Design;
using FastReport.Utils;
using FastReport.TypeEditors;
using FastReport.TypeConverters;
using System.Reflection;
using FastReport.Editor.Syntax.Parsers;
using System.Threading;
using System.Globalization;

namespace FastReport.Controls
{
    internal enum PropertyGridContent
    {
        Properties,
        Events
    }

    internal class FRPropertyGrid : PropertyGrid
    {
        private UIStyle style;

        public UIStyle Style
        {
            get { return style; }
            set
            {
                style = value;
#if MONO
          ToolStripRenderer = UIStyleUtils.GetToolStripRenderer(style);
#endif
            }
        }

        public FRPropertyGrid()
        {
            UseCompatibleTextRendering = false;
            PropertySort = PropertySort.Alphabetical;
            PropertyTabs.RemoveTabType(typeof(PropertiesTab));
        }
    }

    internal class FRPropertiesGrid : FRPropertyGrid
    {
        protected override Type DefaultTabType
        {
            get { return typeof(FRPropertiesTab); }
        }

        public FRPropertiesGrid()
        {
            PropertyTabs.AddTabType(typeof(FRPropertiesTab));
        }
    }

    internal class FREventsGrid : FRPropertyGrid
    {
        protected override Type DefaultTabType
        {
            get { return typeof(FREventsTab); }
        }

        public FREventsGrid()
        {
            PropertyTabs.AddTabType(typeof(FREventsTab));
        }
    }

    internal class FRPropertyDescriptor : PropertyDescriptor
    {
        private PropertyDescriptor descriptor;

        public override string Category
        {
            get
            {
                return GetCategory();
            }
        }

        private string GetCategory()
        {
            CultureInfo currentlangUI = Thread.CurrentThread.CurrentUICulture;
            Thread.CurrentThread.CurrentUICulture = Config.EngCultureInfo;
            string category = descriptor.Category;
            Thread.CurrentThread.CurrentUICulture = currentlangUI;
            if (Res.StringExists("Properties,Categories," + category))
                return Res.Get("Properties,Categories," + category);
            return category;
        }

        public override string DisplayName
        {
            get
            {
                return descriptor.DisplayName;
            }
        }

        public override string Description
        {
            get
            {
                try
                {
                    return ReflectionRepository.DescriptionHelper.GetDescription(ComponentType.GetProperty(Name));
                }
                catch
                {
                }
                return descriptor.Description;
            }
        }

        public override Type ComponentType
        {
            get { return descriptor.ComponentType; }
        }

        public override bool IsReadOnly
        {
            get { return descriptor.IsReadOnly; }
        }

        public override Type PropertyType
        {
            get { return descriptor.PropertyType; }
        }

        public override bool CanResetValue(object component)
        {
            return descriptor.CanResetValue(component);
        }

        public override object GetValue(object component)
        {
            return descriptor.GetValue(component);
        }

        public override void ResetValue(object component)
        {
            descriptor.ResetValue(component);
        }

        public override void SetValue(object component, object value)
        {
            descriptor.SetValue(component, value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return descriptor.ShouldSerializeValue(component);
        }

        public FRPropertyDescriptor(PropertyDescriptor descriptor)
            : base(descriptor)
        {
            this.descriptor = descriptor;
        }
    }

    internal class FREventDescriptor : FRPropertyDescriptor
    {
        public override string DisplayName
        {
            get { return base.DisplayName.Replace("Event", ""); }
        }

        protected override Attribute[] AttributeArray
        {
            get
            {
                return new Attribute[] { 
            new EditorAttribute(typeof(EventEditor), typeof(UITypeEditor)),
            new TypeConverterAttribute(typeof(EventConverter))};
            }
            set { base.AttributeArray = value; }
        }

        public FREventDescriptor(PropertyDescriptor descriptor)
            : base(descriptor)
        {
        }
    }

    internal class FRPropertiesTab : PropertyTab
    {
        #region Public Methods
        public override bool CanExtend(object extendee)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(
          ITypeDescriptorContext context, object component, Attribute[] attributes)
        {
            return GetProperties(component, attributes);
        }

        public override PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(component);

            PropertyDescriptorCollection properties = new PropertyDescriptorCollection(null);
            foreach (PropertyDescriptor prop in props)
            {
                BrowsableAttribute browsable = prop.Attributes[typeof(BrowsableAttribute)] as BrowsableAttribute;
                // skip nonbrowsable properties
                if (browsable != null && browsable.Browsable == false)
                    continue;
                // skip properties other than "Restriction" if DontModify flag is set
                if (component is Base && (component as Base).HasRestriction(Restrictions.DontModify) && prop.Name != "Restrictions")
                    continue;
                // skip all properties if HideAllProperties flag is set
                if (component is Base && (component as Base).HasRestriction(Restrictions.HideAllProperties))
                    continue;
                // check if property is not an event
                if (!prop.Name.EndsWith("Event"))
                    properties.Add(new FRPropertyDescriptor(prop));
            }
            return properties;
        }

        public override Bitmap Bitmap
        {
            get { return Res.GetImage(78); }
        }

        public override string TabName
        {
            get { return Res.Get("Designer,ToolWindow,Properties,PropertiesTab"); }
        }
        #endregion
    }


    internal class FREventsTab : PropertyTab
    {
        #region Public Methods
        public override bool CanExtend(object extendee)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(
          ITypeDescriptorContext context, object component, Attribute[] attributes)
        {
            return GetProperties(component, attributes);
        }

        public override PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(component);

            PropertyDescriptorCollection properties = new PropertyDescriptorCollection(null);
            foreach (PropertyDescriptor prop in props)
            {
                BrowsableAttribute attr = prop.Attributes[typeof(BrowsableAttribute)] as BrowsableAttribute;
                // skip nonbrowsable properties
                if (attr != null && attr.Browsable == false) continue;
                // check if property is an event
                if (prop.Name.EndsWith("Event"))
                    properties.Add(new FREventDescriptor(prop));
            }
            return properties;
        }

        public override Bitmap Bitmap
        {
            get { return Res.GetImage(79); }
        }

        public override string TabName
        {
            get { return Res.Get("Designer,ToolWindow,Properties,EventsTab"); }
        }
        #endregion
    }
}
