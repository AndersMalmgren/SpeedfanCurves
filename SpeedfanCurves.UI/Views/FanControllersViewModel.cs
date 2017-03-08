using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using SpeedfanCurves.Core;
using SpeedfanCurves.Core.Model;

namespace SpeedfanCurves.UI.Views
{
    public class FanControllersViewModel : PropertyChangedBase
    {
        private readonly IRepository repository;
        private readonly Func<FanControllerViewModel> factory;
        private FanControllerViewModel selectedFanController;

        public FanControllersViewModel(IRepository repository, Func<FanControllerViewModel> factory)
        {
            this.repository = repository;
            this.factory = factory;
            FanControllers = new BindableCollection<FanControllerViewModel>(repository.Config.Select(c => factory().Init(c)));
        }

        public BindableCollection<FanControllerViewModel> FanControllers { get; private set; }

        public FanControllerViewModel SelectedFanController
        {
            get { return selectedFanController; }
            set
            {
                selectedFanController = value;
                NotifyOfPropertyChange(() => SelectedFanController);
            }
        }

        public void AddNewFanController()
        {
            var config = repository.AddNewFanController();
            var model = factory().Init(config);
            FanControllers.Add(model);
            SelectedFanController = model;
        }
    }
}
