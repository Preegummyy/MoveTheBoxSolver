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
        public string SolutionText { get; set; }
        public SolutionPage(string solutionText)
        {
            InitializeComponent();
            this.SolutionText = solutionText;
            this.BindingContext = this;
        }
    }
}