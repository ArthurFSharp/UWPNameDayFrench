using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Nameday
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MainPageViewModel Logic => DataContext as MainPageViewModel;

        public MainPage()
        {
            this.InitializeComponent();
            Logic.PropertyChanged += Logic_PropertyChanged;
        }

        private void Logic_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainPageViewModel.LoadingState))
                VisualStateManager.GoToState(this, Logic.LoadingState.ToString(), true);
        }

        private async void btEmail_Click(object sender, RoutedEventArgs e)
        {
            var contact = ((FrameworkElement)sender).DataContext as ContactEx;
            if (contact != null)
                await ((MainPageViewModel)this.DataContext).SendEmailAsync(contact.Contact);
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPage));
        }

        private void appBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            splitView.IsPaneOpen = true;
        }
    }
}
