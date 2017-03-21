using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace Test
{
    /// <summary>
    /// Interaktionslogik für Window11.xaml
    /// </summary>
    public partial class Window11 : MetroWindow
    {
        public Window11()
        {
            InitializeComponent();
        }

        private void cbJa_Checked(object sender, RoutedEventArgs e)
        {
            if (cbJa.IsChecked == true) { cbNein.IsChecked = false; }
        }

        private void cbNein_Checked(object sender, RoutedEventArgs e)
        {
            if (cbNein.IsChecked == true) { cbJa.IsChecked = false; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (cbJa.IsChecked == true)
            {
                Properties.Settings.Default.Setting = true;
            }
            else Properties.Settings.Default.Setting = false;

            this.ShowMessageAsync("Erfolgreich","Die Einstellung wurden erfolgreich gespeichert");
            this.Close();
        }

        private void Button_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.Setting == false)
            { cbNein.IsChecked = true; }
            else cbJa.IsChecked = true;
        }
    }
}
