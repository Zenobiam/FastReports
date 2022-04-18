using System.Windows.Forms;

namespace FastReport.Gauge
{
    public partial class GaugeObject : IHasEditor
    {
        /// <inheritdoc/>
        public override void OnBeforeInsert(int flags)
        {
            base.OnBeforeInsert(flags);
            // to avoid applying last formatting
            Border.Lines = BorderLines.All;
        }

        /// <inheritdoc />
        public virtual bool InvokeEditor()
        {
            using (GaugeEditorForm gaugeEditor = new GaugeEditorForm(this))
            {
                return InvokeEditor(gaugeEditor);
            }
        }

        /// <inheritdoc />
        public virtual bool InvokeEditor(GaugeEditorForm gaugeEditor)
        {
            GaugeObject oldGauge = (GaugeObject)Clone();
            using (gaugeEditor)
            {
                if (gaugeEditor.ShowDialog() != DialogResult.OK)
                {
                    Scale = oldGauge.Scale;
                    Pointer = oldGauge.Pointer;
                    Label = oldGauge.Label;
                    Fill = oldGauge.Fill;
                }
                else
                    Report.Designer.SetModified(this, "Change");
            }
            return true;
        }
    }
}
