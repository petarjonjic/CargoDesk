using CargoDesk.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CargoDesk
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Otvori aplikaciju na formi Primka
            MainFrame.Navigate(typeof(Views.PagePrimka));

            // Test konekcije na bazu – samo jednom nakon aktivacije
            this.Activated += MainWindow_Activated;
        }

        private async void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            // Skidamo event da se ne izvršava više puta
            this.Activated -= MainWindow_Activated;

            try
            {
                await Database.TestKonekcijeAsync();
                System.Diagnostics.Debug.WriteLine("Database connection successful.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database connection error: " + ex.Message);
            }
        }

        // =========================
        // NAVIGACIJA – PRIMKA
        // =========================
        private void BtnPrimka_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(typeof(Views.PagePrimka));
        }

        // =========================
        // NAVIGACIJA – OTPREMNICA
        // =========================
        private void BtnOtpremnica_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(typeof(Views.OtpremnicaPage));
        }
    }
}
