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
                        return Color.FromHex("#d59667");
                    case Solver.Models.BoxType.RedWood:
                        return Color.Red;
                    case Solver.Models.BoxType.GreenWood:
                        return Color.FromHex("#90ee90");
                    case Solver.Models.BoxType.BrownLeather:
                        return Color.FromHex("#6a432d");
                    case Solver.Models.BoxType.BrownPaper:
                        return Color.Orange;
                    case Solver.Models.BoxType.BlueSteel:
                        return Color.FromHex("#ADD8E6");
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
