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
    public partial class SolutionPage : ContentPage
    {
        public List<HumanMoveArrow> Solution { get; set; }
        public SolutionPage(List<HumanMoveArrow> solution)
        {
            InitializeComponent();
            this.Solution = solution;
            this.BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            int step = 1;
            foreach (var item in Solution)
            {
                var Stack = new StackLayout() { Orientation = StackOrientation.Horizontal };
                Stack.Children.Add(new Label() { Text = $"Step {step} : Move " });
                Stack.Children.Add(new BoxView() { Color = new BoxVM() { Type = item.FromMoveBoxType }.Color });
                Stack.Children.Add(new Label() { Text = $" in {SolveByPositionPage.MappingColumnIndex(item.StartIndex.Index_X)}{item.StartIndex.Index_Y + 1} {item.Move.ToString()}" });
                MainStack.Children.Add(Stack);
                step++;
            }
        }
    }
}