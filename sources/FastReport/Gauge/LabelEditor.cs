using System;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms;
using FastReport.Gauge;
using FastReport.Gauge.Radial;
using FastReport.Gauge.Simple.Progress;

namespace FastReport.TypeEditors
{
    /// <summary>
    /// Provides a user interface for editing an expression.
    /// </summary>
    public class LabelEditor : UITypeEditor
    {
        /// <summary>
        /// 
        /// </summary>
        protected GaugeObject gauge;
        /// <summary>
        /// 
        /// </summary>
        protected GaugeEditorForm gaugeEditor;

        internal LabelEditor() : base()
        {
            gauge = new GaugeObject();
            gaugeEditor = new GaugeEditorForm(gauge);
        }

        private void SetLabel(GaugeLabel label)
        {
            if (label is SimpleProgressLabel)
            {
                gauge = label.Parent as SimpleProgressGauge;
                gaugeEditor = new SimpleProgressGaugeEditorForm((SimpleProgressGauge)gauge);
            }
            else if (label is RadialLabel)
            {
                gauge = label.Parent as RadialGauge;
                gaugeEditor = new RadialGaugeEditorForm((RadialGauge)gauge);
            }
            gaugeEditor.pageControl1.ActivePage = (Panel)gaugeEditor.pageControl1.Pages.Find("pgLabel", false)[0];
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
            GaugeLabel label = (GaugeLabel)Value;
            Report report = context != null && context.Instance is Base ? (context.Instance as Base).Report : null;
            if (report != null)
            {
                SetLabel(label);
                gauge.InvokeEditor(gaugeEditor);
                return gauge.Label;
            }
            return Value;
        }
    }
}
