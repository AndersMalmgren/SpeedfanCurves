using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using SpeedfanCurves.Core;
using SpeedfanCurves.Core.Common;
using SpeedfanCurves.Core.Model;
using SpeedfanCurves.UI.Events;


namespace SpeedfanCurves.UI.Views
{
    public class CurveConfigViewModel : PropertyChangedBase, IHandle<CurveUpdated>
    {
        private readonly IRepository repository;
        private CurveConfig model;

        public CurveConfigViewModel(IRepository repository, IEventAggregator eventAggregator)
        {
            this.repository = repository;
            PickableTemps = new BindableCollection<Temp>();
            eventAggregator.Subscribe(this);
        }

        public CurveConfigViewModel Init(CurveConfig model)
        {
            this.model = model;

            UpdateCurve();
            Temps = new BindableCollection<FanControllerTempViewModel>(model.Temps.Select(t => new FanControllerTempViewModel(t, model)));

            UpdatePickableTemps();
            return this;
        }

        public void Handle(CurveUpdated message)
        {
            if (model.Curve == message.Curve)
                UpdateCurve();
        }

        public bool Equals(Curve curve)
        {
            return model.Curve == curve;
        }

        private void UpdateCurve()
        {
            var points = model.Curve.Points;
            Points = CurveMath.GetInterpolatedCubicSplinedCurve(model.Curve.Points);
        }

        private void UpdatePickableTemps()
        {
            PickableTemps.Clear();
            PickableTemps.AddRange(repository
                .Temps
                .Where(t => model.Temps.All(t2 => t2.Temp != t)));

            NotifyOfPropertyChange(() => PickableTemps);
        }

        public BindableCollection<Temp> PickableTemps { get; }

        private Temp selectedPickableTemp;
        public Temp SelectedPickableTemp
        {
            get { return selectedPickableTemp; }
            set
            {
                selectedPickableTemp = value;
                CanAddNewTemp = value != null;
                NotifyOfPropertyChange(() => SelectedPickableTemp);
            }
        }


        private bool canDeleteTemp;
        public bool CanDeleteTemp
        {
            get { return canDeleteTemp; }
            private set
            {
                canDeleteTemp = value;
                NotifyOfPropertyChange(() => CanDeleteTemp);
            }
        }

        public void DeleteTemp()
        {
            SelectedTemp.Delete();
            Temps.Remove(SelectedTemp);
            SelectedTemp = null;
            UpdatePickableTemps();
        }

        private bool canAddNewTemp;
        public bool CanAddNewTemp
        {
            get { return canAddNewTemp; }
            private set
            {
                canAddNewTemp = value; 
                NotifyOfPropertyChange(() => CanAddNewTemp);
            }
        }

        public void AddNewTemp()
        {
            var temp = model.AddNewTemp(SelectedPickableTemp); 
            var newModel = new FanControllerTempViewModel(temp, model);
            Temps.Add(newModel);
            SelectedTemp = newModel;

            SelectedPickableTemp = null;
            UpdatePickableTemps();
        }

        public BindableCollection<FanControllerTempViewModel> Temps { get; private set; }
        
        private FanControllerTempViewModel selectedTemp;

        public FanControllerTempViewModel SelectedTemp
        {
            get { return selectedTemp; }
            set
            {
                selectedTemp = value;;
                CanDeleteTemp = value != null;

                NotifyOfPropertyChange(() => SelectedTemp);
            }
        }

        private IEnumerable<Point> points;
        public IEnumerable<Point> Points
        {
            get { return points; }
            set
            {
                points = value; 
                NotifyOfPropertyChange(() => Points);
            }
        }

        public string Name => model.Curve.Name;
    }
}