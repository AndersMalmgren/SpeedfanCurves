using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using SpeedfanCurves.Core.Model;
using SpeedfanCurves.UI.Events;

namespace SpeedfanCurves.UI.Views
{
    public class FanViewModel : PropertyChangedBase
    {
        private readonly IEventAggregator eventAggregator;
        private Fan model;

        public FanViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public FanViewModel Init(Fan model)
        {
            this.model = model;
            return this;
        }

        public string Name => model.Name;
        public int Min => model.Min;
        public int Max => model.Max;
        public bool Variate => model.Variate;

        public bool Active
        {
            get { return model.Active; }
            set
            {
                model.Active = value;
                eventAggregator.PublishOnUIThread(new FanUpdated(model));
                NotifyOfPropertyChange(() => Active);
            }
        }
    }
}
