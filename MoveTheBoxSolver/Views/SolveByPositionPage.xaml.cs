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
        private bool isLoading;
        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                isLoading = value;
                OnPropertyChanged("IsLoading");
                OnPropertyChanged("IsButtonEnabled");
                OnPropertyChanged("IsChange");
                OnPropertyChanged("IsShowClear");
            }
        }
        public bool IsButtonEnabled
        {
            get { return !IsLoading; }
        }

        public bool IsHasSolution { get; set; }
        
        private bool isChange = false;
        public bool IsChange
        {
            get { return isChange; }
            set
            {
                isChange = value;
                OnPropertyChanged("IsLoading");
                OnPropertyChanged("IsButtonEnabled");
                OnPropertyChanged("IsChange");
                OnPropertyChanged("IsShowClear");
            }
        }

        public bool IsShowClear
        {
            get { return IsChange && !IsLoading; }
        }

        public bool IsAppearingFirstTime = true;
        public SelectColorPage SelectPage;
        public Dictionary<TupleKey, BoxType> BoxsToSolve = new Dictionary<TupleKey, BoxType>();
        public List<HumanMoveArrow> Solution = new List<HumanMoveArrow>();

        public SolveByPositionPage()
        {
            InitializeComponent();
            this.BindingContext = this;
            SelectPage = new SelectColorPage(this);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

           
            if (IsAppearingFirstTime)
            {
                IsAppearingFirstTime = false;

                for (int i = 1; i <= 9; i++)
                {
                    IndexDefineGrid.Children.Add(new Label()
                    {
                        Text = i.ToString()
                        ,
                        HorizontalOptions = LayoutOptions.Center
                        ,
                        VerticalOptions = LayoutOptions.Center
                    }, 0, 9 - i);
                }

                for (int i = 1; i <= 7; i++)
                {
                    IndexDefineGrid.Children.Add(new Label()
                    {
                        Text = MappingColumnIndex(i - 1)
                        ,
                        HorizontalOptions = LayoutOptions.Center
                        ,
                        VerticalOptions = LayoutOptions.Center
                    }, i, 9);
                }

                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        BoxView box = new BoxView()
                        {
                            BindingContext = new BoxVM() { Type = BoxType.Empty }
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
            if (IsLoading)
            {
                return;
            }
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

        private void Clear_All_Clicked(object sender, EventArgs eventArgs)
        {
            Number_Of_Move.Text = "";
            Number_Of_Move.BackgroundColor = Color.Default;
            Solution = new List<HumanMoveArrow>();
            BoxsToSolve = new Dictionary<TupleKey, BoxType>();
            SelectPage = new SelectColorPage(this);
            foreach (var item in MainTableGrid.Children)
            {
                BoxView boxView = item as BoxView;
                if (boxView != null)
                {
                    BoxVM boxVM = boxView.BindingContext as BoxVM;
                    if (boxVM != null)
                    {
                        boxVM.Type = BoxType.Empty;
                    }
                }
            }
            IsChange = false;
        }

        private async void Solve_Clicked(object sender, EventArgs e)
        {

            string Text = Number_Of_Move.Text;
            int Limit;
            if (int.TryParse(Text, out Limit))
            {
                if (Limit > 0 && Limit <= 10)
                {
                    if (BoxsToSolve.Count <= 0)
                    {
                        await DisplayAlert("Slover", "No Box in Puzzle", "OK");
                        return;
                    }

                    bool AllEmpty = true;
                    foreach (var item in BoxsToSolve)
                    {
                        if (item.Value != BoxType.Empty)
                        {
                            AllEmpty = false;
                            break;
                        }
                    }

                    if (AllEmpty)
                    {
                        await DisplayAlert("Slover", "No Box in Puzzle", "OK");
                        return;
                    }

                    Number_Of_Move.BackgroundColor = Color.Default;
                    PuzzleTable puzzle = new PuzzleTable(7, 9);


                    puzzle.CreatePuzzle(BoxsToSolve);
                    Solver.Solver solver = new Solver.Solver();
                    if (!IsHasSolution)
                    {
                        try
                        {
                            IsLoading = true;
                            Solution = await solver.SolveAsync(puzzle, Limit);
                        }
                        finally
                        {
                            IsLoading = false;
                        }
                        IsHasSolution = true;
                    }

                    if (Solution == null)
                    {
                        await DisplayAlert("Slover", "No Solution", "OK");
                    }
                    else
                    {
                        await Navigation.PushModalAsync(new SolutionPage(Solution));
                        //DisplayAlert("Slover", SolutionText, "OK");
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

        private void Number_Of_Move_TextChanged(object sender, TextChangedEventArgs e)
        {
            Number_Of_Move.BackgroundColor = Color.Default;
            IsChange = true;
            IsHasSolution = false;
        }

        public static string MappingColumnIndex(int Index_X)
        {
            switch (Index_X)
            {
                case 0:
                    return "a";
                case 1:
                    return "b";
                case 2:
                    return "c";
                case 3:
                    return "d";
                case 4:
                    return "e";
                case 5:
                    return "f";
                case 6:
                    return "g";
                default:
                    return "";
            }
        }
    }
}