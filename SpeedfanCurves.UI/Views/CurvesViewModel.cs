using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using SpeedfanCurves.Core;
using SpeedfanCurves.Core.Model;
using SpeedfanCurves.UI.Events;

namespace SpeedfanCurves.UI.Views
{
    public class CurvesViewModel : PropertyChangedBase
    {
        private readonly IRepository repository;
        private readonly IEventAggregator eventAggregator;
        private readonly Func<CurveViewModel> factory;
        private CurveViewModel selectedCurve;

        public CurvesViewModel(IRepository repository, IEventAggregator eventAggregator, Func<CurveViewModel> factory)
        {
            this.repository = repository;
            this.eventAggregator = eventAggregator;
            this.factory = factory;
            Curves = new BindableCollection<CurveViewModel>(repository.Curves.Select((c, i) => factory().Init(i, c)));

            MinSpeed = 30;
            MaxSpeed = 100;
        }

        public BindableCollection<CurveViewModel> Curves { get; private set; }

        public CurveViewModel SelectedCurve
        {
            get { return selectedCurve; }
            set
            {
                selectedCurve = value;
                CanDeleteCurve = value != null;
                NotifyOfPropertyChange(() => SelectedCurve);
            }
        }

        public int MinSpeed { get; set; }
        public int MaxSpeed { get; set; }

        public void AddNewCurve()
        {
            var curve = Curve.Create(MinSpeed, MaxSpeed);
            repository.Curves.Add(curve);

            var viewModel = factory().Init(Curves.Last().Id + 1, curve);
            Curves.Add(viewModel);
            SelectedCurve = viewModel;
            eventAggregator.PublishOnUIThread(new CurveAdded(curve));
        }

        private bool canDeleteCurve;
        public bool CanDeleteCurve
        {
            get { return canDeleteCurve; }
            set
            {
                canDeleteCurve = value; 
                NotifyOfPropertyChange(() => CanDeleteCurve);
            }
        }

        public void DeleteCurve()
        {
            var model = SelectedCurve.Model;
            Curves.Remove(SelectedCurve);
            repository.RemoveCurve(model);

            SelectedCurve = null;
            eventAggregator.PublishOnUIThread(new CurveDeleted(model));
        }

    }
}
