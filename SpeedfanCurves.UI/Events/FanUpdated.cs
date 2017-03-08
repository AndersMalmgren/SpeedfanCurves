using System;
using SpeedfanCurves.Core.Model;

namespace SpeedfanCurves.UI.Events
{
    public class FanUpdated
    {
        public Fan Fan { get; }

        public FanUpdated(Fan fan)
        {
            Fan = fan;
        }
    }
}