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
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace Test
{
    public partial class Window9 : MetroWindow
    {
        private int bNr;
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
                    checkMonat.SelectedIndex = dr.GetInt32(3) - 1;
                    bk.CloseCon();
                }
                catch { this.ShowMessageAsync("Fehler", "Es ist ein Fehler aufgetreten."); bk.CloseCon(); } //MessageBox.Show("Es ist ein Fehler aufgetreten.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch { this.ShowMessageAsync("Fehler", "Die Verbindung zur Datenbank konnte nicht hergestellt werden."); bk.CloseCon();  } //MessageBox.Show("Die Verbindung konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error)
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
                                bool _tmpb;
                                if (cBStatus.IsChecked != false)
                                    _tmpb = true;
                                else _tmpb = false;
                                // Ob ein Aktiver Bonus Existiert
                                dr = bk.Select($"SELECT * FROM Bonus WHERE B_Aktiv = true AND B_Monat = {checkMonat.SelectedIndex + 1}");
                                dr.Read();
                                try
                                {
                                    if (dr.HasRows)
                                    {
                                        if(dr.GetInt32(0) != Convert.ToInt32(laNr.Content.ToString()))
                                        { this.ShowMessageAsync("", "Es existiert bereits ein Bonus in diesem Monat;"); }
                                        //else { //Alles ok!}
                                    }
                                    else
                                    {
                                        string _tmpBP = tbBP.Text.Replace(',', '.').Replace("%", "").Trim();
                                        bk.Update($"UPDATE Bonus SET B_Bez='{tbBez.Text}',B_Zuschlag={_tmpBP},B_Monat={checkMonat.SelectedIndex + 1},B_Aktiv={_tmpb} WHERE B_Nr = {bNr}");
                                        this.ShowMessageAsync("", "Dieser Bonus wurde erfolgreich geändert.");
                                        bk.CloseCon();
                                        this.Close();
                                    }
                                    //if (dr.GetBoolean(4) == true)
                                    //    BAktivE = true;
                                    //else BAktivE = false;
                                }
                                catch { this.ShowMessageAsync("Fehler", "Es ist ein Fehler aufgetreten."); bk.CloseCon(); }
                            }
                            catch { this.ShowMessageAsync("Fehler", "Es ist ein Fehler aufgetretten."); }
                        }
                        catch { this.ShowMessageAsync("Fehler", "Die Verbindung zur Datenbank konnte nicht hergestellt werden."); }
                    }
                    else { this.ShowMessageAsync("Fehler", "Es muss ein Monat ausgewählt werden."); }
                }
                else { this.ShowMessageAsync("Fehler", "Der Prozentsatz darf ausschließlich Numerisch sein."); }
            }
            else { this.ShowMessageAsync("Fehler", "Die Textfelder dürfen nicht Leer gelassen werden."); }
        }
    }
}
