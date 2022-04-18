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
    public class ScaleEditor : UITypeEditor
    {
        /// <summary>
        /// 
        /// </summary>
        protected GaugeObject gauge;
        /// <summary>
        /// 
        /// </summary>
        protected GaugeEditorForm gaugeEditor;

        internal ScaleEditor() : base()
        {
            gauge = new GaugeObject();
            gaugeEditor = new GaugeEditorForm(gauge);
        }

        private void SetScale(GaugeScale scale)
        {
            if (scale is LinearScale)
            {
                gauge = scale.Parent as LinearGauge;
                gaugeEditor = new LinearGaugeEditorForm((LinearGauge)gauge);
            }
            else if (scale is SimpleScale)
            {
                gauge = scale.Parent as SimpleGauge;
                gaugeEditor = new SimpleGaugeEditorForm((SimpleGauge)gauge);
            }
            else if (scale is RadialScale)
            {
                gauge = scale.Parent as RadialGauge;
                gaugeEditor = new RadialGaugeEditorForm((RadialGauge)gauge);
            }
            gaugeEditor.pageControl1.ActivePage = (Panel)gaugeEditor.pageControl1.Pages.Find("pgScale", false)[0];
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
            GaugeScale scale = (GaugeScale)Value;
            Report report = context != null && context.Instance is Base ? (context.Instance as Base).Report : null;
            if (report != null)
            {
                SetScale(scale);
                gauge.InvokeEditor(gaugeEditor);
                return gaugeEditor.Gauge.Scale;
            }
            return Value;
        }
    }
}
