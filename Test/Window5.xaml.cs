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
using System.Data;
using System.Data.OleDb;

namespace Test
{
    /// <summary>
    /// Interaktionslogik für Window5.xaml
    /// </summary>
    public partial class Window5 : Window
    {
        Basisklasse bk = new Basisklasse();
        OleDbDataReader dr;
        public Window5()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                bk.Connection();
                try
                {
                    dr = bk.Select("SELECT Last(P_Nr) FROM Personal;");
                    dr.Read();
                    try
                    {
                        int inter = dr.GetInt32(0);
                        lAbtNr.Content = inter;
                    }
                    catch
                    {
                        lAbtNr.Content = "1";
                    }
                    
                    bk.CloseCon();
                }
                catch { MessageBox.Show("Fehler beim bestimmen der Personal Nummer", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); return; }

                try
                {
                    bk.Connection();
                    dr = bk.Select("SELECT Abt_Bez FROM Abteilung;");
                    while (dr.Read())
                    {
                        string bob = dr.GetString(1);
                        cbAbtNr.Items.Add(bob);
                    }
                    cbAbtNr.Items.Refresh();
                    bk.CloseCon();

                }
                catch { MessageBox.Show("Fehler beim Bestimmen der Abteilungen", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); return; }

            }
            catch { MessageBox.Show("Die Verbindung zur Datenbank konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
    }
}
