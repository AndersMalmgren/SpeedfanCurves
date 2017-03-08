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
    public class FanControllerViewModel : PropertyChangedBase, IHandle<CurveAdded>, IHandle<CurveDeleted>, IHandle<FanUpdated>
    {
        private readonly IRepository repository;
        private readonly Func<CurveConfigViewModel> factory;
        private FanControllerConfig model;
        private CurveConfigViewModel selectedCurve;

        public FanControllerViewModel(IRepository repository, Func<CurveConfigViewModel> factory, IEventAggregator eventAggregator) 
        {
            this.repository = repository;
            this.factory = factory;
            PickableCurves = new BindableCollection<Curve>();
            Methods = EnumViewModel<Methods>.Create();

            eventAggregator.Subscribe(this);
        }

        private IEnumerable<Fan> GetActiveFans()
        {
            return repository.Fans.Where(f => f.Active);
        }

        public FanControllerViewModel Init(FanControllerConfig model)
        {
            this.model = model;
            Curves = new BindableCollection<CurveConfigViewModel>(model.Curves.Select(c => factory().Init(c)));

            UpdatePickableCurves();
            return this;
        }


        public BindableCollection<CurveConfigViewModel> Curves { get; private set; }

        public CurveConfigViewModel SelectedCurve
        {
            get { return selectedCurve; }
            set
            {
                selectedCurve = value; 
                NotifyOfPropertyChange(() => SelectedCurve);
            }
        }


        public void Handle(CurveAdded message)
        {
            UpdatePickableCurves();
        }

        public void Handle(CurveDeleted message)
        {
            Curves.RemoveRange(Curves.Where(c => c.Equals(message.Curve)).ToList());

            if (SelectedPickableCurve != null && SelectedPickableCurve == message.Curve)
                SelectedPickableCurve = null;

            if (SelectedCurve != null && SelectedCurve.Equals(message.Curve))
                SelectedCurve = null;

            UpdatePickableCurves();
        }

        public void Handle(FanUpdated message)
        {
            NotifyOfPropertyChange(() => Fans);
            if (SelectedFan == null)
                SelectedFan = Fans.FirstOrDefault();
        }

        private void UpdatePickableCurves()
        {
            PickableCurves.Clear();
            PickableCurves.AddRange(repository
                .Curves
                .Where(c => model.Curves.All(c2 => c2.Curve != c)));

            NotifyOfPropertyChange(() => PickableCurves);
        }

        public BindableCollection<Curve> PickableCurves { get; }

        private Curve selectedPickableCurve;
        public Curve SelectedPickableCurve
        {
            get { return selectedPickableCurve; }
            set
            {
                selectedPickableCurve = value;
                CanAddCurve = value != null;
                NotifyOfPropertyChange(() => SelectedPickableCurve);
            }
        }

        private bool canAddCurve;

        public bool CanAddCurve
        {
            get { return canAddCurve; }
            private set
            {
                canAddCurve = value;
                NotifyOfPropertyChange(() => CanAddCurve);
            }
        }

        public void AddCurve()
        {

            var curveConfig = model.AddCurve(SelectedPickableCurve);
            var newModel = factory().Init(curveConfig);
            Curves.Add(newModel);
            SelectedCurve = newModel;

            SelectedPickableCurve = null;
            UpdatePickableCurves();
        }

        public IEnumerable<Fan> Fans
        {
            get { return repository.Fans.Where(f => f.Active); }
        }

        public Fan SelectedFan {
            get { return model.Controller.Fan; }
            set
            {
                model.Controller.Fan = value;
                NotifyOfPropertyChange(() => SelectedFan);
            }
        }
        
        public IEnumerable<EnumViewModel<Methods>> Methods { get; }

        private EnumViewModel<Methods> selectedMethod;

        public EnumViewModel<Methods> SelectedMethod
        {
            get { return selectedMethod ?? (selectedMethod = Methods.First(vm => vm.Value == model.Controller.Method)); }
            set
            {
                selectedMethod = value;
                model.Controller.Method = value.Value;
            }
        }

        public string Name
        {
            get { return model.Controller.Name; }
            set
            {
                model.Controller.Name = value; 
                NotifyOfPropertyChange(() => Name);
            }
        }

        public bool Active
        {
            get { return model.Controller.Active; }
            set { model.Controller.Active = value; }
        }
    }

    public class EnumViewModel<TEnum>
    {
        public EnumViewModel(TEnum value)
        {
            Name = value.ToString();
            Value = value;
        }

        public TEnum Value { get; }
        public string Name { get; }

        public static IEnumerable<EnumViewModel<TEnum>> Create()
        {
            var values = Enum.GetValues(typeof (TEnum));
            return values.Cast<TEnum>().Select(v => new EnumViewModel<TEnum>(v)).ToList();
        }
    }
}
