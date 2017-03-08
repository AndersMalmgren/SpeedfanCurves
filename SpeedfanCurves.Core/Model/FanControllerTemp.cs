using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedfanCurves.Core.Model
{
    public class FanControllerTemp
    {

        public FanControllerTemp(Temp temp)
        {
            Temp = temp;
        }

        public Temp Temp { get; }
        public int MinTemp { get; set; }
        public int MaxTemp { get; set; }
        public int Hysteresis { get; set; }
        public string PointsRaw { get; set; }
    }
}
