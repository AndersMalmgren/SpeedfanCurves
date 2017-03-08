using System.Collections.Generic;
using SpeedfanCurves.Core.Model;

namespace SpeedfanCurves.Core
{
    public interface IRepository
    {
        List<Curve> Curves { get; }
        List<FanControllerConfig> Config { get; }
        IEnumerable<Fan> Fans { get; }
        IEnumerable<Temp> Temps { get; }
        void Load();
        void Save();
        void RemoveCurve(Curve model);
        FanControllerConfig AddNewFanController();
    }
}