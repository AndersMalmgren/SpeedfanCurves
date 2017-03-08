using System;
using System.Collections.Generic;
using System.Linq;
using SpeedfanCurves.Core;
using SpeedfanCurves.Core.Model;

namespace SpeedfanCurves.UI.Views
{
    public class FansViewModel
    {
        public FansViewModel(IRepository repository, Func<FanViewModel> factory)
        {
            Fans = repository.Fans.Select(f => factory().Init(f));
        }

        public IEnumerable<FanViewModel> Fans { get; set; }
    }
}