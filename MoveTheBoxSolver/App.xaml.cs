using MoveTheBoxSolver.Views;
using System;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MoveTheBoxSolver
{
    public partial class App : Application
    {
        public static Assembly assembly { get; set; }
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage());
            App.assembly = IntrospectionExtensions.GetTypeInfo(typeof(App)).Assembly;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
