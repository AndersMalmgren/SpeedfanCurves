using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedfanCurves.Core.Model
{
    public class Fan : ModelBase
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public bool Variate { get; set; }
        public bool Logged { get; set; }
    }
}
