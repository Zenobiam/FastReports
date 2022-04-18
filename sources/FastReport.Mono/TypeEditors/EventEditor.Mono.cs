using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms.Design;

namespace FastReport.TypeEditors
{
  internal class EventEditor : UITypeEditor
  {
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
    {
      return UITypeEditorEditStyle.Modal;
    }

    public override object EditValue(ITypeDescriptorContext context, 
      IServiceProvider provider, object Value)
    {
      // this method is called when we click on drop-down arrow
      if (context != null && context.Instance != null)
      {
        Base component = null;
        if (context.Instance is Base)
          component = context.Instance as Base;
        else if (context.Instance is object[])
          component = ((object[])context.Instance)[0] as Base;
        Report report = component.Report;

        report.Designer.ActiveReportTab.SwitchToCode();

        string scriptEventName = context.PropertyDescriptor.Name;
        // cut off "Event" ("xxxEvent")
        string eventName = scriptEventName.Replace("Event", "");
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
            report.CodeHelper.Editor.Focus();
            return newEventName;
          }
        }
      }  
      return Value;
    }
      
  }
}