using System;
using System.Drawing;
using FastReport.Utils;

namespace FastReport
{
    public partial class DigitalSignatureObject
    {

        public override bool InvokeEditor()
        {
            return false;
        }

        public override void OnBeforeInsert(int flags)
        {
            base.OnBeforeInsert(flags);
            // TODO: Place appearence in the designer here
            this.Width = 3 * Utils.Units.Centimeters;
            this.Height = 1 * Utils.Units.Centimeters;
        }
    }
}