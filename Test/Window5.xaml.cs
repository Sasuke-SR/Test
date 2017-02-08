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
                        int inter = dr.GetInt32(0) + 1;
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
                        cbAbtName.Items.Add(dr.GetString(0));
                    }
                    cbAbtName.Items.Refresh();
                    bk.CloseCon();

                }
                catch { MessageBox.Show("Fehler beim Bestimmen der Abteilungen", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); return; }

                try
                {
                    bk.Connection();
                    dr = bk.Select("SELECT L_Bez FROM Lohngruppen;");
                    while (dr.Read())
                    {
                        cbLgName.Items.Add(dr.GetString(0));
                    }
                    cbLgName.Items.Refresh();
                    bk.CloseCon();

                }
                catch { MessageBox.Show("Fehler beim Bestimmen der Abteilungen", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); return; }

            }
            catch { MessageBox.Show("Die Verbindung zur Datenbank konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void bPers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bk.Connection();
                try
                {
                    bk.Insert($"INSERT INTO Personal (P_VName, P_NName, P_Abteilungs_Nr, P_Lohngruppen_Nr) VALUES ('{tbName.Text}', '{tbNName.Text}', {tbAbtNr.Text}," +
                              $"{tbLgNr.Text});");
                }
                catch
                {
                    MessageBox.Show("Fehler beim Einfügen der Person", "", MessageBoxButton.OK, MessageBoxImage.Error);
                    bk.CloseCon();
                }

            }
            catch { MessageBox.Show("Die Verbindung zur Datenbank konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void bMainWin_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cbAbtName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                bk.Connection();
                try
                {
                    dr = bk.Select($"SELECT Abt_Nr FROM Abteilung WHERE Abt_Bez = '{cbAbtName.SelectedItem.ToString()}';");
                    dr.Read();
                    tbAbtNr.Text = dr.GetValue(0).ToString();
                    bk.CloseCon();
                }
                catch
                {
                    MessageBox.Show("Fehler Suchen der Abteilung", "", MessageBoxButton.OK, MessageBoxImage.Error);
                    bk.CloseCon();
                }
            }
            catch
            {
                MessageBox.Show("Die Verbindung konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cbLgName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                bk.Connection();
                try
                {
                    dr = bk.Select($"SELECT L_Nr FROM Lohngruppen WHERE L_Bez = '{cbLgName.SelectedItem.ToString()}';");
                    dr.Read();
                    tbLgNr.Text = dr.GetValue(0).ToString();
                    bk.CloseCon();
                }
                catch
                {
                    MessageBox.Show("Fehler Suchen der ALohngruppe", "", MessageBoxButton.OK, MessageBoxImage.Error);
                    bk.CloseCon();
                }
            }
            catch
            {
                MessageBox.Show("Die Verbindung konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
