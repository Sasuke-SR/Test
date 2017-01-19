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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Test
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Basisklasse bk = new Basisklasse();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window2 nAbrechnung = new Window2();
            nAbrechnung.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try { bk.Connection(); }
            catch { MessageBox.Show("Die Verbindung konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Window3 nLohnG = new Window3();
            nLohnG.ShowDialog();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Window4 nAbteilung = new Window4();
            nAbteilung.ShowDialog(); 
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Window5 nPersonal = new Window5();
            nPersonal.ShowDialog();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Window6 nUGruppe = new Window6();
            nUGruppe.ShowDialog();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            Window7 nUStunden = new Window7();
            nUStunden.Show();
        }
    }
}
