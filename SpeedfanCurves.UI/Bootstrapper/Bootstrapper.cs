using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using SpeedfanCurves.Core;
using SpeedfanCurves.UI.Views;
using StructureMap;

namespace SpeedfanCurves.UI.Bootstrapper
{
    public class Bootstrapper : BootstrapperBase
    {
        private IContainer container;

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            container = Core.Bootstrap.Boostrapper.Create();
            container.Configure(config =>
            {
                config.For<IWindowManager>().Use<WindowManager>().Singleton();
                config.For<IEventAggregator>().Use<EventAggregator>().Singleton();

            });

            container.GetInstance<IRepository>().Load();
        }

        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            yield return container.GetAllInstances(service);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainShellViewModel>();
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            container.GetInstance<IRepository>().Save();

            base.OnExit(sender, e);
        }
    }
}
