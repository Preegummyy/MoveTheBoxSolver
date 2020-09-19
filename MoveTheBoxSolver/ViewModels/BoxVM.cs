using MoveTheBoxSolver.Solver.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace MoveTheBoxSolver.ViewModels
{
    public class BoxVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Color Color
        {
            get
            {
                switch (Type)
                {
                    case Solver.Models.BoxType.Empty:
                        return Color.Gray;
                    case Solver.Models.BoxType.BrownWood:
                        return Color.FromRgb(169, 133, 92);
                    case Solver.Models.BoxType.RedWood:
                        return Color.FromRgb(183, 71, 34);
                    case Solver.Models.BoxType.GreenWood:
                        return Color.FromRgb(156, 164, 52);
                    case Solver.Models.BoxType.BrownLeather:
                        return Color.FromRgb(102, 65, 38);
                    case Solver.Models.BoxType.BrownPaper:
                        return Color.FromRgb(189, 140, 61);
                    case Solver.Models.BoxType.BlueSteel:
                        return Color.FromRgb(149, 165, 168);
                    default:
                        return Color.Black;
                }
            }
        }

        private BoxType type;
        public BoxType Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
                OnPropertyChanged("Color");
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
