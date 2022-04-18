using System.Windows.Forms;

namespace FastReport.Gauge.Simple
{
    public partial class SimpleGauge : IHasEditor
    {
        /// <inheritdoc/>
        public override void OnBeforeInsert(int flags)
        {
            base.OnBeforeInsert(flags);
            // to avoid applying last formatting
            Border.Lines = BorderLines.All;
        }

        /// <inheritdoc />
        bool IHasEditor.InvokeEditor()
        {
            GaugeObject oldGauge = (GaugeObject)Clone();
            using (SimpleGaugeEditorForm gaugeEditor = new SimpleGaugeEditorForm(this))
            {
                if (gaugeEditor.ShowDialog() != DialogResult.OK)
                    this.Scale = oldGauge.Scale;
            }
            return true;
        }
    }
}
