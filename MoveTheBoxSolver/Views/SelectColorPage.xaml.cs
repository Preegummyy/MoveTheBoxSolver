using MoveTheBoxSolver.Solver.Models;
using MoveTheBoxSolver.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MoveTheBoxSolver.Solver.Models.PuzzleTable;

namespace MoveTheBoxSolver.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectColorPage : ContentPage
    {
        public bool IsAppearingFirstTime = true;
        public SolveByPositionPage ParentPage;
        public BoxVM boxTapped;
        public int Column { get; set; }
        public int Row { get; set; }
        public SelectColorPage(SolveByPositionPage parentPage)
        {
            InitializeComponent();
            ParentPage = parentPage;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (IsAppearingFirstTime)
            {
                IsAppearingFirstTime = false;
                //TapGestureRecognizer popuplayoutTapGestureRecognizer = new TapGestureRecognizer();
                //popuplayoutTapGestureRecognizer.Tapped += (s, e) =>
                //{
                //    Navigation.PopModalAsync();
                //};
                //popuplayout.GestureRecognizers.Add(popuplayoutTapGestureRecognizer);

                foreach (BoxType boxtype in (BoxType[])Enum.GetValues(typeof(BoxType)))
                {
                    SelectColorChoice Choice = new SelectColorChoice(boxtype);
                    TapGestureRecognizer boxTapGestureRecognizer = new TapGestureRecognizer();
                    boxTapGestureRecognizer.Tapped += (s, e) =>
                    {
                        ChoiceTapped(s, e);
                    };
                    Choice.GestureRecognizers.Add(boxTapGestureRecognizer);
                    popuplayout.Children.Add(Choice);
                }
            }
        }

        void ChoiceTapped(object sender, EventArgs e)
        {
            if (sender != null)
            {
                SelectColorChoice choice = sender as SelectColorChoice;
                if (choice != null)
                {
                    if (choice.Box != null)
                    {
                        boxTapped.Type = choice.Box.Type;
                        TupleKey index = new TupleKey(Column, 8 - Row);
                        try
                        {
                            var RefBox = ParentPage.BoxsToSolve[index];
                            ParentPage.BoxsToSolve[index] = boxTapped.Type;
                        }
                        catch (KeyNotFoundException)
                        {
                            ParentPage.BoxsToSolve.Add(index, boxTapped.Type);
                        }
                        Navigation.PopModalAsync();
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }

    }
}