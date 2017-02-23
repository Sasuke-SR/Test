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
using System.Data.OleDb;

namespace Test
{
    /// <summary>
    /// Interaktionslogik für Window9.xaml
    /// </summary>
    public partial class Window9 : Window
    {
        private int bNr;
        bool BAktivE;
        Basisklasse bk = new Basisklasse();
        OleDbDataReader dr;
        public Window9(int nr)
        {
            bNr = nr;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Laden der Boxen
            checkMonat.Items.Insert(0, "Januar");
            checkMonat.Items.Insert(1, "Februar");
            checkMonat.Items.Insert(2, "März");
            checkMonat.Items.Insert(3, "April");
            checkMonat.Items.Insert(4, "Mai");
            checkMonat.Items.Insert(5, "Juni");
            checkMonat.Items.Insert(6, "Juli");
            checkMonat.Items.Insert(7, "August");
            checkMonat.Items.Insert(8, "September");
            checkMonat.Items.Insert(9, "Oktober");
            checkMonat.Items.Insert(10, "November");
            checkMonat.Items.Insert(11, "Dezember");

            laNr.Content = bNr.ToString();
            try
            {
                bk.Connection();
                try
                {
                    dr = bk.Select($"SELECT * FROM Bonus WHERE B_Nr = {bNr}");
                    dr.Read();
                    tbBez.Text = dr.GetString(1);
                    tbBP.Text = dr.GetDouble(2).ToString();
                    if (dr.GetBoolean(4) == true)
                        cBStatus.IsChecked = true;
                    else
                        cBStatus.IsChecked = false;
                    int _index = dr.GetInt32(3) - 1;
                    checkMonat.SelectedIndex = _index;
                    bk.CloseCon();
                }
                catch { MessageBox.Show("Es ist ein Fehler aufgetreten.", "", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
            catch { MessageBox.Show("Die Verbindung konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void bMainWin_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            // Bei Schließen der Maske den Select auflösen.
        }

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            string _Error = "";
            if (!string.IsNullOrWhiteSpace(tbBez.Text) && !string.IsNullOrWhiteSpace(tbBP.Text))
            {
                if (bk.IsAllowed(tbBP.Text, false, true, true, "%,."))
                {
                    if (checkMonat.SelectedItem != null)
                    {
                        try
                        {
                            bk.Connection();
                            try
                            {
                                int _tmp = checkMonat.SelectedIndex + 1; bool _tmpb;
                                if (cBStatus.IsChecked != false)
                                    _tmpb = true;
                                else _tmpb = false;
                                // Ob ein Aktiver Bonus Existiert
                                dr = bk.Select("SELECT * FROM Bonus WHERE B_Aktiv = true");
                                dr.Read();
                                try
                                {
                                    if (dr.GetBoolean(4) == true)
                                        BAktivE = true;
                                    else BAktivE = false;
                                }
                                catch { }
                                if (BAktivE != true && cBStatus.IsChecked == false)
                                {
                                    string _tmpBP = tbBP.Text.Replace(',', '.').Replace("%", "").Trim();
                                    bk.Update($"UPDATE Bonus SET B_Bez='{tbBez.Text}',B_Zuschlag={_tmpBP},B_Monat={_tmp},B_Aktiv={_tmpb} WHERE B_Nr = {bNr}");
                                    MessageBox.Show("Dieser Bonus wurde erfolgreich geändert", "", MessageBoxButton.OK, MessageBoxImage.Error);
                                    bk.CloseCon();
                                    this.Close();
                                }
                                else MessageBox.Show("Es besteht schon eine Aktivierten Bonus", "", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            catch { MessageBox.Show("Es ist ein Fehler aufgetretten", "", MessageBoxButton.OK, MessageBoxImage.Error); }
                        }
                        catch { MessageBox.Show("Die Verbindung konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error); }
                    }
                    else _Error += "Es muss ein Monat ausgewählt werden";
                }
                else _Error += "Der Prozentsatz muss Numerisch sein";
            }
            else { _Error += "Die Textfelder dürfen nicht Leer sein"; MessageBox.Show(_Error, "", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
    }
}
