using System.Windows.Forms;

namespace FastReport.Gauge.Simple.Progress
{
    public partial class SimpleProgressGauge : IHasEditor
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
            SimpleProgressGauge oldGauge = (SimpleProgressGauge)Clone();
            using (SimpleProgressGaugeEditorForm gaugeEditor = new SimpleProgressGaugeEditorForm(this))
            {
                if (gaugeEditor.ShowDialog() != DialogResult.OK)
                {
                    SimpleProgressPointer ptr = Pointer as SimpleProgressPointer;
                    ptr = oldGauge.Pointer as SimpleProgressPointer;
                }
                return true;
                //OnChange();
            }
        }
    }
}
