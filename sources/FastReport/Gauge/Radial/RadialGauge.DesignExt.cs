using FastReport.Utils;
using System.Windows.Forms;

namespace FastReport.Gauge.Radial
{
    public partial class RadialGauge : IHasEditor
    {
        /// <inheritdoc/>
        public override void OnBeforeInsert(int flags)
        {
            base.OnBeforeInsert(flags);
            // to avoid applying last formatting
            Border.Lines = BorderLines.None;
        }

        /// <inheritdoc />
        bool IHasEditor.InvokeEditor()
        {
            GaugeObject oldGauge = (GaugeObject)Clone();
            using (RadialGaugeEditorForm gaugeEditor = new RadialGaugeEditorForm(this))
            {
                if (gaugeEditor.ShowDialog() != DialogResult.OK)
                    this.Scale = oldGauge.Scale;
            }
            return true;
            //OnChange();
        }
    }
}
