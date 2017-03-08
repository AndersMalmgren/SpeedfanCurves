using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap;

namespace SpeedfanCurves.Core.Bootstrap
{
    public static class Boostrapper
    {
        public static IContainer Create()
        {
            var container = new Container();
            container.Configure(config =>
            {
                config.For<IRepository>()
                .Use<Repository>()
                .Singleton();
            });

            return container;
        }
    }
}
