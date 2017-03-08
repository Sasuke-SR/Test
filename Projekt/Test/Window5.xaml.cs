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
using MahApps.Metro.Controls.Dialogs;

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
                    try { lPerNr.Content = dr.GetInt32(0) + 1; }
                    catch { lPerNr.Content = "1"; }
                    bk.CloseCon();
                }
                catch { this.ShowMessageAsync("Fehler", "Es ist ein Problem aufgetretten."); bk.CloseCon(); } /*MessageBox.Show("Es ist ein Problem aufgetretten.", "", MessageBoxButton.OK, MessageBoxImage.Error);*/
            }
            catch { this.ShowMessageAsync("Fehler", "Die Verbindung zur Datenbank konnte nicht hergestellt werden."); }/*MessageBox.Show("Die Verbindung zur Datenbank konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon();*/
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
                    try { lPerNr.Content = dr.GetInt32(0) + 1; }
                    catch { lPerNr.Content = "1"; }

                    bk.CloseCon();
                }
                catch { this.ShowMessageAsync("Fehler", "Fehler beim bestimmen der Personal Nummer") ; bk.CloseCon(); return; } //MessageBox.Show("Fehler beim bestimmen der Personal Nummer", "", MessageBoxButton.OK, MessageBoxImage.Error)

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
                catch { this.ShowMessageAsync("Fehler", "Beim Bestimmen der Abteilung ist ein Fehler aufgetreten."); bk.CloseCon(); return; } //MessageBox.Show("Fehler beim Bestimmen der Abteilungen", "", MessageBoxButton.OK, MessageBoxImage.Error)

                try
                {
                    bk.Connection();
                    dr = bk.Select("SELECT L_Bez FROM Lohngruppen;");

                    while (dr.Read()) { cbLgName.Items.Add(dr.GetString(0).ToString()); }
                    cbLgName.Items.Refresh();
                    bk.CloseCon();
                }
                catch { this.ShowMessageAsync("Fehler", "Beim Bestimmen der Abteilungen ist ein Fehler aufgetreten."); bk.CloseCon(); return; } //MessageBox.Show("Fehler beim Bestimmen der Abteilungen", "", MessageBoxButton.OK, MessageBoxImage.Error)

                lAbrNr.Content = bk.FormateNumber(lPerNr.Content.ToString(), lAbrNr.Content.ToString(), 6);

            }
            catch { this.ShowMessageAsync("Fehler", "Die Verbindung zur Datenbank konnte nicht hergestellt werden."); }/*MessageBox.Show("Die Verbindung zur Datenbank konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error);*/
        }

        private void bPers_Click(object sender, RoutedEventArgs e)
        {
            bk.Connection();
            try
            {
                if (!string.IsNullOrWhiteSpace(tbName.Text) && !string.IsNullOrWhiteSpace(tbNName.Text) && !string.IsNullOrWhiteSpace(tbAbtNr.Text) && !string.IsNullOrWhiteSpace(tbLgNr.Text))
                {
                    if (bk.IsAllowed(tbName.Text, true, false, true, "'.") && bk.IsAllowed(tbNName.Text, true, false, true, "'."))
                    {
                        //Erstellung
                        string _tmpQuery = string.Format("Insert INTO Personal (P_VName, P_NName, P_Abteilungs_Nr, P_Lohngruppen_Nr, P_Abrech_Nr) VALUES ('{0}', '{1}', {2}, {3}, {4})"
                                                        , tbName.Text, tbNName.Text, tbAbtNr.Text, tbLgNr.Text, lAbrNr.Content.ToString());
                        bk.Insert(_tmpQuery);
                        this.ShowMessageAsync("", $"Die Person {tbNName.Text}, {tbName.Text} wurde erstellt.");
                        bk.CloseCon();
                        // Neuladen der Maske
                        #region Maske neuladen
                        Mask_Load();
                        tbLgNr.Text = ""; tbName.Text = ""; tbNName.Text = ""; tbAbtNr.Text = ""; tbLgNr.Text = "";
                        cbAbtName.Text = ""; cbLgName.Text = ""; cbAbtName.SelectedItem = null; cbLgName.SelectedItem = null;
                        #endregion Maske neuladen
                    }
                    else { this.ShowMessageAsync("Fehler", "Es dürfen keine Sonderzeichen so wie Numerische Werte eingegeben werden."); bk.CloseCon(); }// MessageBox.Show("Es dürfen keine Sonderzeichen so wie Numerische Werte eingegeben werden", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else { this.ShowMessageAsync("Fehler", "Die Felder dürfen nicht leer gelassen werden."); bk.CloseCon(); }//MessageBox.Show("Die Felder dürfen nicht Leer sein","");

            }
            catch
            {
                this.ShowMessageAsync("Fehler", "Beim Einfügen der Person ist ein Fehler aufgetreten.");//MessageBox.Show("Fehler beim Einfügen der Person", "", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        lAbrNr.Content = bk.FormateNumber(tbAbtNr.Text, lAbrNr.Content.ToString(), 3);
                    }
                    catch { this.ShowMessageAsync("Fehler", "Beim Suchen der Abteilung ist ein Fehler aufgetreten."); bk.CloseCon(); }//MessageBox.Show("Fehler Suchen der Abteilung", "", MessageBoxButton.OK, MessageBoxImage.Error)
                }
                catch { this.ShowMessageAsync("Fehler", "Die Verbindung zur Datenbank konnte nicht hergestellt werden.") }//MessageBox.Show("Die Verbindung konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        lAbrNr.Content = bk.FormateNumber(tbLgNr.Text, lAbrNr.Content.ToString(), 0);
                    }
                    catch { this.ShowMessageAsync("Fehler", "Beim Suchen der Lohngruppe ist ein Fehler aufgetreten."); bk.CloseCon(); }//MessageBox.Show("Fehler Suchen der ALohngruppe", "", MessageBoxButton.OK, MessageBoxImage.Error)
                }
                catch { this.ShowMessageAsync("Fehler", "Die Verbindung zur Datenbank konnte nicht hergestellt werden."); }//MessageBox.Show("Die Verbindung konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}