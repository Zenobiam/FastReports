using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace FastReport.TypeConverters
{
    internal class EventConverter : TypeConverter
    {
        #region Public Methods

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
                return (string)value;
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value == null)
                    return "";
                return (string)value;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            // this method is called when we doubleclick the event
            if (context != null && context.Instance != null)
            {
                Base component = null;
                if (context.Instance is Base)
                    component = context.Instance as Base;
                else if (context.Instance is object[])
                    component = ((object[])context.Instance)[0] as Base;
                Report report = component.Report;

                if (report.Designer.Restrictions.DontEditCode)
                    return base.GetStandardValues(context);

                report.Designer.ActiveReportTab.SwitchToCode();
                // cut off "Event" ("xxxEvent")
                string eventName = context.PropertyDescriptor.Name.Replace("Event", "");
                EventInfo eventInfo = component.GetType().GetEvent(eventName);

                string eventValue = context.PropertyDescriptor.GetValue(context.Instance) as string;
                if (eventValue != null && eventValue != "")
                {
                    // locate event handler
                    report.CodeHelper.LocateHandler(eventValue);
                }
                else
                {
                    // create new event handler
                    string newEventName = component.Name + "_" + eventName;
                    if (report.CodeHelper.AddHandler(eventInfo.EventHandlerType, newEventName))
                    {
                        context.PropertyDescriptor.SetValue(context.Instance, newEventName);
                        report.Designer.SetModified(null, "Change");
                        report.CodeHelper.Editor.Focus();
                    }
                }
            }
            return base.GetStandardValues(context);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
#if MONO
            return false;
#else
            return true;
#endif			
        }

        #endregion Public Methods
    }
}