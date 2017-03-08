using System;
using Caliburn.Micro;

namespace SpeedfanCurves.UI.Views
{
    public class MainShellViewModel : Screen
    {
        public MainShellViewModel(CurvesViewModel curves, FanControllersViewModel fanControllers, FansViewModel fans)
        {
            DisplayName = "Speedfan curve configurator";

            Curves = curves;
            FanControllers = fanControllers;
            Fans = fans;
        }

        public FansViewModel Fans { get; set; }
        public CurvesViewModel Curves { get; }
        public FanControllersViewModel FanControllers { get; }
    }
}
