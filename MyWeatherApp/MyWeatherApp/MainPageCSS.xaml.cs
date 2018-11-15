using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace MyWeatherApp
{
    public partial class MainPageCSS : ContentPage
    {
        MainPageViewModel viewModel;

        public MainPageCSS()
        {
            InitializeComponent();

            BindingContext = viewModel = new MainPageViewModel(Navigation, new DialogService());
        }
    }
}
