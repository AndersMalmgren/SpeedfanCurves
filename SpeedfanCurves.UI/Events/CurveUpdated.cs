using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpeedfanCurves.Core.Model;

namespace SpeedfanCurves.UI.Events
{
    public class CurveUpdated : CurveEvent
    {
        public CurveUpdated(Curve curve) : base(curve){}
    }
}
