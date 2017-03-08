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
using MahApps.Metro.Controls;

namespace Test
{
    /// <summary>
    /// Interaktionslogik für Window5.xaml
    /// </summary>
    public partial class Window5 : MetroWindow
    {
        Basisklasse bk = new Basisklasse();
        OleDbDataReader dr;

        public Window5()
        {
            InitializeComponent();
        }

        private void Mask_Load()
        {
            try
            {
                bk.Connection();
                try
                {
                    // Letzten Personal Datensatz auslesen
                    dr = bk.Select("SELECT last(P_Nr) FROM Personal");
                    dr.Read();
                    try { lAbtNr.Content = dr.GetInt32(0) + 1; }
                    catch { lAbtNr.Content = "1"; }
                    bk.CloseCon();
                }
                catch { MessageBox.Show("Es ist ein Problem aufgetretten.", "", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
            catch { MessageBox.Show("Die Verbindung zur Datenbank konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); }
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
                    try { lAbtNr.Content = dr.GetInt32(0) + 1; }
                    catch { lAbtNr.Content = "1"; }

                    bk.CloseCon();
                }
                catch { MessageBox.Show("Fehler beim bestimmen der Personal Nummer", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); return; }

                try
                {
                    bk.Connection();
                    dr = bk.Select("SELECT Abt_Bez FROM Abteilung;");
                    while (dr.Read())
                    {
                        cbAbtName.Items.Add(dr.GetString(0).ToString());
                    }
                    cbAbtName.Items.Refresh();
                    bk.CloseCon();

                }
                catch { MessageBox.Show("Fehler beim Bestimmen der Abteilungen", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); return; }

                try
                {
                    bk.Connection();
                    dr = bk.Select("SELECT L_Bez FROM Lohngruppen;");

                    while (dr.Read()) { cbLgName.Items.Add(dr.GetString(0).ToString()); }
                    cbLgName.Items.Refresh();
                    bk.CloseCon();
                }
                catch { MessageBox.Show("Fehler beim Bestimmen der Abteilungen", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); return; }

            }
            catch { MessageBox.Show("Die Verbindung zur Datenbank konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void bPers_Click(object sender, RoutedEventArgs e)
        {
            bk.Connection();
            try
            {
                if (!String.IsNullOrWhiteSpace(tbName.Text) && !String.IsNullOrWhiteSpace(tbNName.Text))
                {
                    if (bk.IsAllowed(tbName.Text, true, false, true, "'.") && bk.IsAllowed(tbNName.Text, true, false, true, "'."))
                    {
                        //Erstellung
                        string _tmpQuery = string.Format("Insert INTO Personal (P_VName, P_NName, P_Abteilungs_Nr, P_Lohngruppen_Nr) VALUES ('{0}', '{1}', {2}, {3})", tbName.Text, tbNName.Text, tbAbtNr.Text, tbLgNr.Text);
                        bk.Insert(_tmpQuery);
                        string _tmpName = string.Format("Die Person {0}, {1} wurde erstellt.", tbNName.Text, tbName.Text);
                        MessageBox.Show(_tmpName, "", MessageBoxButton.OK, MessageBoxImage.Information);
                        bk.CloseCon();
                        // Neuladen der Maske
                        Mask_Load();
                        tbLgNr.Text = ""; tbName.Text = ""; tbNName.Text = ""; tbAbtNr.Text = ""; tbLgNr.Text = "";
                        cbAbtName.Text = ""; cbLgName.Text = ""; cbAbtName.SelectedItem = null; cbLgName.SelectedItem = null;
                    }
                    else { bk.CloseCon(); MessageBox.Show("Es dürfen keine Sonderzeichen so wie Numerische Werte eingegeben werden", "", MessageBoxButton.OK, MessageBoxImage.Error); }
                }
                else MessageBox.Show("Die Felder dürfen nicht Leer sein","");

            }
            catch
            {
                MessageBox.Show("Fehler beim Einfügen der Person", "", MessageBoxButton.OK, MessageBoxImage.Error);
                bk.CloseCon();
            }
        }

        private void bMainWin_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cbAbtName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbAbtName.SelectedItem != null)
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
                    catch { MessageBox.Show("Fehler Suchen der Abteilung", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); }
                }
                catch { MessageBox.Show("Die Verbindung konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
        }

        private void cbLgName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbLgName.SelectedItem != null)
            {
                try
                {
                    bk.Connection();
                    try
                    {
                        dr = bk.Select($"SELECT L_Nr FROM Lohngruppen WHERE L_Bez = '{cbLgName.SelectedItem.ToString()}';");
                        dr.Read();
                        tbLgNr.Text = dr.GetValue(0).ToString(); ;
                        bk.CloseCon();
                    }
                    catch { MessageBox.Show("Fehler Suchen der ALohngruppe", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); }
                }
                catch { MessageBox.Show("Die Verbindung konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
        }
    }
}