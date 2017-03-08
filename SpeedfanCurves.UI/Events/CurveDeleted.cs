﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpeedfanCurves.Core.Model;

namespace SpeedfanCurves.UI.Events
{
    public class CurveDeleted : CurveEvent
    {
        public CurveDeleted(Curve curve) : base(curve){}
    }
}
