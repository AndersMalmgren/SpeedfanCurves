using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Caliburn.Micro;
using SpeedfanCurves.Core.Common;
using SpeedfanCurves.Core.Extensions;
using SpeedfanCurves.Core.Model;
using SpeedfanCurves.UI.Common.Visiblox;
using SpeedfanCurves.UI.Events;

namespace SpeedfanCurves.UI.Views
{
    public class CurveViewModel : PropertyChangedBase
    {
        private readonly IEventAggregator eventAggregator;
        private readonly Timer timer;

        private int? selectedPointIndex;

        public CurveViewModel(IEventAggregator eventAggregator)
        {
            timer = new Timer();
            timer.Interval = 100;
            timer.AutoReset = false;
            timer.Elapsed += (s, e) => eventAggregator.PublishOnUIThread(new CurveUpdated(Model));
        }

        public CurveViewModel Init(int id, Curve model)
        {
            Id = id;
            Model = model;

            InitCurve();

            return this;
        }

        public string Name => Model.Name;
        public int Id { get; private set; }
        public Curve Model { get; private set; }

        private void InitCurve()
        {
            SetSelectablePoints();
            Points = CalculateNewPoints();
        }

        private void SetSelectablePoints()
        {
            SelectablePoints = Model
                .Points
                .Skip(1)
                .TakeAllButLast();
        }

        //public IEnumerable<IResult> Delete()
        //{
            //var message = resultFactory.ShowMessageBox(string.Format("Delete {0}?", Curve.Name), "Curve will be deleted, continue?", MessageBoxButton.OKCancel);
            //yield return message;

            //if (message.Result == System.Windows.MessageBoxResult.OK)
            //    eventAggregator.Publish(new DeleteCurveEvent(this));
        //}

        public bool HasSelectedPoint
        {
            get { return selectedPointIndex.HasValue; }
        }

        private bool canSetDefault;
        public bool CanSetDefault
        {
            get { return canSetDefault; }
            set
            {
                canSetDefault = value;
                NotifyOfPropertyChange(() => canSetDefault);
            }
        }

        public void ApplyNewValuesToSelectedPoint()
        {
            ApplyNewSelectedPoint(new Point(SelectedPointX, SelectedPointY));
        }


        public void OnPointSelected(MovePointBehaviour.PointSelectedEventArgs e)
        {
            var index = Model.IndexOf(e.Point);

            selectedPointIndex = index;

            UpdateSelectedPoint();

            CanSetDefault = selectedPointIndex == Model.Points.Count - 1;
            NotifyOfPropertyChange(() => HasSelectedPoint);
        }

        private void UpdateSelectedPoint()
        {
            SelectedPointX = GetSelectedPoint().X;
            SelectedPointY = GetSelectedPoint().Y;
        }

        private double selectedPointX;
        public double SelectedPointX
        {
            get { return selectedPointX; }
            set
            {
                selectedPointX = value;
                NotifyOfPropertyChange(() => SelectedPointX);
            }
        }

        private double selectedPointY;
        public double SelectedPointY
        {
            get { return selectedPointY; }
            set
            {
                selectedPointY = value;
                NotifyOfPropertyChange(() => SelectedPointY);
            }
        }

        private void ApplyNewSelectedPoint(Point newPoint)
        {
            var args = new MovePointBehaviour.PointMoveEventArgs
            {
                OldPoint = GetSelectedPoint(),
                NewPoint = newPoint
            };
            OnPointDragged(args);
            SetSelectablePoints();
        }

        private Point GetSelectedPoint()
        {
            if (selectedPointIndex.HasValue)
                return Model.Points[selectedPointIndex.Value];

            return new Point();
        }

        public void OnPointDragged(MovePointBehaviour.PointMoveEventArgs e)
        {
            var oldPoint = e.OldPoint;
            var newPoint = e.NewPoint;

            newPoint.X = oldPoint.X;
            newPoint.Y = Math.Round(newPoint.Y);

            var index = Model.IndexOf(e.OldPoint);

            e.NewPoint = newPoint;
            Model.Points[index] = e.NewPoint;

            Points = CalculateNewPoints();
            UpdateSelectedPoint();

            timer.Stop();
            timer.Start();
        }

        private IEnumerable<Point> CalculateNewPoints()
        {
            return CurveMath.GetInterpolatedCubicSplinedCurve(Model.Points);
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

        private IEnumerable<Point> selectablePoints;

        public IEnumerable<Point> SelectablePoints
        {
            get { return selectablePoints; }
            set
            {
                selectablePoints = value;
                NotifyOfPropertyChange(() => SelectablePoints);
            }
        }
    }
}