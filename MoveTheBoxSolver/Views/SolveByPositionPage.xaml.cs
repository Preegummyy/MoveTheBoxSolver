using MoveTheBoxSolver.Solver;
using MoveTheBoxSolver.Solver.Models;
using MoveTheBoxSolver.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using static MoveTheBoxSolver.Solver.Models.PuzzleTable;

namespace MoveTheBoxSolver.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SolveByPositionPage : ContentPage
    {
        public PuzzleTableViewModels PuzzleTableVM { get; set; }
        public bool IsAppearingFirstTime = true;
        public SelectColorPage SelectPage;
        public Dictionary<TupleKey, BoxType> BoxsToSolve = new Dictionary<TupleKey, BoxType>();
        public SolveByPositionPage()
        {
            InitializeComponent();
            MainTableGrid.BindingContext = PuzzleTableVM;
            SelectPage = new SelectColorPage(this);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var DeviceWidth = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Width;
            var BoxSize = DeviceWidth / 7.0;

            if (IsAppearingFirstTime)
            {
                IsAppearingFirstTime = false;
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        BoxView box = new BoxView()
                        {
                            BindingContext = new BoxVM() { Type = BoxType.Empty },
                            WidthRequest = BoxSize,
                            HeightRequest = BoxSize
                        };
                        box.SetBinding(BoxView.ColorProperty, "Color");
                        TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
                        tapGestureRecognizer.Tapped += (s, e) =>
                        {
                            BoxTapped(s, e);
                        };
                        box.GestureRecognizers.Add(tapGestureRecognizer);
                        MainTableGrid.Children.Add(box, i, j);
                    }
                }
            }
        }

        void BoxTapped(object sender, EventArgs e)
        {
            if (sender != null)
            {
                BoxView boxTapped = sender as BoxView;
                if (boxTapped != null)
                {
                    BoxVM VM = boxTapped.BindingContext as BoxVM;
                    if (VM != null)
                    {
                        var Column = Grid.GetColumn(boxTapped);
                        var Row = Grid.GetRow(boxTapped);
                        SelectPage.boxTapped = VM;
                        SelectPage.Column = Column;
                        SelectPage.Row = Row;
                        Navigation.PushModalAsync(SelectPage);
                        //DisplayAlert("Box", $"type :{VM.Type},row :{Row},column :{Column}", "Cancel");
                        //VM.Type = BoxType.BlueSteel;
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

        private void Solve_Clicked(object sender, EventArgs e)
        {
            string Text = Number_Of_Move.Text;
            int Limit;
            if (int.TryParse(Text, out Limit))
            {
                if (Limit > 0 && Limit <= 10)
                {
                    PuzzleTable puzzle = new PuzzleTable(7, 9);
                    puzzle.CreatePuzzle(BoxsToSolve);
                    Solver.Solver solver = new Solver.Solver();
                    var Solution = solver.Solve(puzzle, Limit);
                    var SolutionText = "";
                    if (Solution == null)
                    {
                        DisplayAlert("Slover", "No Solution", "OK");
                    }
                    else
                    {
                        foreach (var item in Solution)
                        {
                            SolutionText += $"{item.Move.ToString()},from type:{item.FromMoveBoxType.ToString()},to type:{item.ToMoveBoxType.ToString()},floor:{item.StartIndex.Index_Y + 1},index:{item.StartIndex.Index_X + 1}{Environment.NewLine}";
                        }
                        DisplayAlert("Slover", SolutionText, "OK");
                    }
                    
                   
                }
                else
                {
                    InvalidMoveText();
                }
            }
            else
            {
                InvalidMoveText();
            }
        }

        private void InvalidMoveText()
        {
            Number_Of_Move.Text = "";
            Number_Of_Move.BackgroundColor = Color.Red;
            DisplayAlert("Slover", "Invalid move limit.", "OK");
        }
    }
}