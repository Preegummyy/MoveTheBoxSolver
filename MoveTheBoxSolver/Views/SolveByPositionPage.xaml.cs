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

namespace MoveTheBoxSolver.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SolveByPositionPage : ContentPage
    {
        public PuzzleTableViewModels PuzzleTableVM { get; set; }
        public bool IsAppearingFirstTime = true;

        public SolveByPositionPage()
        {
            InitializeComponent();
            MainTableGrid.BindingContext = PuzzleTableVM;
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
                        DisplayAlert("Box", $"type :{VM.Type},row :{Row},column :{Column}", "Cancel");
                        //OpenPopup();
                        VM.Type = BoxType.BlueSteel;
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

        //private async void OpenPopup()
        //{
        //    if (!this.popuplayout.IsVisible)
        //    {
        //        this.popuplayout.IsVisible = !this.popuplayout.IsVisible;
        //        this.popuplayout.AnchorX = 1;
        //        this.popuplayout.AnchorY = 1;

        //        Animation scaleAnimation = new Animation(
        //            f => this.popuplayout.Scale = f,
        //            0.5,
        //            1,
        //            Easing.SinInOut);

        //        Animation fadeAnimation = new Animation(
        //            f => this.popuplayout.Opacity = f,
        //            0.2,
        //            1,
        //            Easing.SinInOut);

        //        scaleAnimation.Commit(this.popuplayout, "popupScaleAnimation", 250);
        //        fadeAnimation.Commit(this.popuplayout, "popupFadeAnimation", 250);
        //    }
        //    else
        //    {
        //        await Task.WhenAny<bool>
        //          (
        //            this.popuplayout.FadeTo(0, 200, Easing.SinInOut)
        //          );

        //        this.popuplayout.IsVisible = !this.popuplayout.IsVisible;
        //    }
        //}
    }
}