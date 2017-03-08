using System.Collections.Generic;

namespace SpeedfanCurves.Core.Model
{
    public class FanControllerConfig
    {
        public FanControllerConfig ()
        {
            Curves = new List<CurveConfig>();
        }

        public FanController Controller { get; set; }
        public List<CurveConfig> Curves { get; set; }

        public CurveConfig AddCurve(Curve curve)
        {
            var config = new CurveConfig(curve);
            Curves.Add(config);
            return config;
        }
    }
}