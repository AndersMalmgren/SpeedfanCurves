using System.Collections.Generic;
using System.Linq;

namespace SpeedfanCurves.Core.Model
{
    public class CurveConfig
    {
        public Curve Curve { get; }
        public List<FanControllerTemp> Temps { get; }

        public CurveConfig(Curve curve) :this(curve, Enumerable.Empty<FanControllerTemp>())
        {
        }

        public CurveConfig(Curve curve, IEnumerable<FanControllerTemp> temps)
        {
            Curve = curve;
            Temps = new List<FanControllerTemp>(temps);
        }

        public FanControllerTemp AddNewTemp(Temp temp)
        {
            var newTemp = new FanControllerTemp(temp);
            Temps.Add(newTemp);
            return newTemp;
        }
    }
}