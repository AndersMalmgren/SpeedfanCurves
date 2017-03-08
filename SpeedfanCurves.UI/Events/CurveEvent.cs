using SpeedfanCurves.Core.Model;

namespace SpeedfanCurves.UI.Events
{
    public abstract class CurveEvent
    {
        public Curve Curve { get; }

        public CurveEvent(Curve curve)
        {
            Curve = curve;
        }
    }
}