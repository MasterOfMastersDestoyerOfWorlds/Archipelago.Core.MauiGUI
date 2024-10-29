using Archipelago.Core.MauiGUI.Models;
using Archipelago.Core.MauiGUI.ViewModels;

namespace Archipelago.Core.MauiGUI
{
    public partial class MainPage : FlyoutPage
    {

        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
        }

    }

}
