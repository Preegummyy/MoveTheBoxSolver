using MoveTheBoxSolver.Solver.Models;
using MoveTheBoxSolver.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MoveTheBoxSolver.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectColorChoice : ContentView
    {
        public Color Color { get; set; }
        public BoxVM Box { get; set; }
        public SelectColorChoice(BoxType boxType)
        {
            InitializeComponent();
            this.Box = new BoxVM() { Type = boxType };
            this.Color = Box.Color;
            this.BindingContext = this;
        }
    }
}