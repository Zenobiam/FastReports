using System.Windows.Forms;

namespace FastReport.Gauge.Linear
{
    public partial class LinearGauge : IHasEditor
    {
        /// <inheritdoc/>
        public override void OnBeforeInsert(int flags)
        {
            base.OnBeforeInsert(flags);
            // to avoid applying last formatting
            Border.Lines = BorderLines.All;
        }

        /// <inheritdoc />
        public override bool InvokeEditor()
        {
            GaugeObject oldGauge = (GaugeObject)Clone();
            using (LinearGaugeEditorForm gaugeEditor = new LinearGaugeEditorForm(this))
            {
                if (gaugeEditor.ShowDialog() != DialogResult.OK)
                    Inverted = (oldGauge as LinearGauge).Inverted;
            }
            return true;
        }
    }
}
