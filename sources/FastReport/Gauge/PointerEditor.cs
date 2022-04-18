using System;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms;
using FastReport.Gauge;
using FastReport.Gauge.Linear;
using FastReport.Gauge.Simple;
using FastReport.Gauge.Radial;

namespace FastReport.TypeEditors
{
    /// <summary>
    /// Provides a user interface for editing an expression.
    /// </summary>
    public class PointerEditor : UITypeEditor
    {
        /// <summary>
        /// 
        /// </summary>
        protected GaugeObject gauge;
        /// <summary>
        /// 
        /// </summary>
        protected GaugeEditorForm gaugeEditor;

        internal PointerEditor() : base()
        {
            gauge = new GaugeObject();
            gaugeEditor = new GaugeEditorForm(gauge);
        }

        private void SetPointer(GaugePointer pointer)
        {
            if (pointer is LinearPointer)
            {
                gauge = pointer.Parent as LinearGauge;
                gaugeEditor = new LinearGaugeEditorForm((LinearGauge)gauge);
            }
            else if (pointer is SimplePointer)
            {
                gauge = pointer.Parent as SimpleGauge;
                gaugeEditor = new SimpleGaugeEditorForm((SimpleGauge)gauge);
            }
            else if (pointer is RadialPointer)
            {
                gauge = pointer.Parent as RadialGauge;
                gaugeEditor = new RadialGaugeEditorForm((RadialGauge)gauge);
            }
            gaugeEditor.pageControl1.ActivePage = (Panel)gaugeEditor.pageControl1.Pages.Find("pgPointer", false)[0];
        }

        /// <inheritdoc/>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        /// <inheritdoc/>
        public override object EditValue(ITypeDescriptorContext context,
          IServiceProvider provider, object Value)
        {
            GaugePointer pointer = (GaugePointer)Value;
            Report report = context != null && context.Instance is Base ? (context.Instance as Base).Report : null;
            if (report != null)
            {
                SetPointer(pointer);
                gauge.InvokeEditor(gaugeEditor);
                return gauge.Pointer;
            }
            return Value;
        }
    }
}
