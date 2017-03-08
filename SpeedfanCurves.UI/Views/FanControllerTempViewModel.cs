using System.ComponentModel;
using Caliburn.Micro;
using SpeedfanCurves.Core.Model;

namespace SpeedfanCurves.UI.Views
{
    public class FanControllerTempViewModel : PropertyChangedBase
    {
        private readonly FanControllerTemp model;
        private readonly CurveConfig config;

        public FanControllerTempViewModel(FanControllerTemp model, CurveConfig config)
        {
            this.model = model;
            this.config = config;
            Name = model.Temp.Name;
        }

        public string Name { get; }

        public int MinTemp
        {
            get { return model.MinTemp; }
            set
            {
                model.MinTemp = value;
                NotifyOfPropertyChange(() => MinTemp);
            }
        }

        public int MaxTemp
        {
            get { return model.MaxTemp; }
            set
            {
                model.MaxTemp = value;
                NotifyOfPropertyChange(() => MaxTemp);
            }
        }

        public int Hysteresis
        {
            get { return model.Hysteresis; }
            set
            {
                model.Hysteresis = value;
                NotifyOfPropertyChange(() => Hysteresis);
            }
        }

        public void Delete()
        {
            config.Temps.Remove(model);
        }
    }
}