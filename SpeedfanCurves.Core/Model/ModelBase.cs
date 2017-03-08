using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedfanCurves.Core.Model
{
    public abstract class ModelBase
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
    }
}
