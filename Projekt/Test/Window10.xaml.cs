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
    /// <summary>
    /// Interaktionslogik für Window10.xaml
    /// </summary>
    public partial class Window10 : MetroWindow
    {
        private int _Nr;
        Basisklasse bk = new Basisklasse();
        OleDbDataReader dr;
        public Window10(int nr)
        {
            _Nr = nr;
            InitializeComponent();
        }

        private void bMainWin_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            laNr.Content = _Nr.ToString();
            try
            {
                bk.Connection();
                try
                {
                    // ComboBoxen füllen
                    dr = bk.Select("SELECT * FROM Abteilung");
                    while (dr.Read())
                    { cbAb.Items.Add(dr.GetInt32(0).ToString() + " - " + dr.GetString(1)); }

                    dr = bk.Select("SELECT * FROM Lohngruppen");
                    while (dr.Read())
                    { cbLG.Items.Add(dr.GetInt32(0).ToString() + " - " + dr.GetString(1)); }
                    // Boxen füllen
                    dr = bk.Select($"SELECT * FROM Personal WHERE P_Nr = {_Nr}");
                    while (dr.Read())
                    {
                        OleDbDataReader dr1; OleDbDataReader dr2;
                        if (dr.GetInt32(4) < 10) { abrechNR.Content = "00" + dr.GetInt32(6).ToString(); }
                        else if (dr.GetInt32(4) > 10 && dr.GetInt32(4) < 100) { abrechNR.Content = "0" + dr.GetInt32(6).ToString(); }

                        dr1 = bk.Select($"SELECT * FROM Lohngruppen WHERE L_Nr = {dr.GetInt32(4)}"); dr1.Read();
                        cbLG.SelectedItem = $"{dr.GetInt32(4)} - {dr1.GetString(1)}";

                        dr2 = bk.Select($"SELECT * FROM Abteilung WHERE Abt_Nr = {dr.GetInt32(3)}"); dr2.Read();
                        cbAb.SelectedItem = $"{dr.GetInt32(3)} - {dr2.GetString(1)}";

                        tbNName.Text = dr.GetString(2);  tbVName.Text = dr.GetString(1);
                        if (dr.GetBoolean(5) == true)  CheckFired.IsChecked = true;

                        dr1.Close(); dr2.Close();
                    }
                    bk.CloseCon();
                }
                catch { this.ShowMessageAsync("", "Es ist ein Fehler aufgetretetn."); bk.CloseCon(); }
            }
            catch { this.ShowMessageAsync("", "Die Verbindung konnte nicht geöffnet werden."); bk.CloseCon(); }
        }

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbNName.Text) && !string.IsNullOrWhiteSpace(tbVName.Text))
            {
                if (bk.IsAllowed(tbNName.Text, true, false, false, ".-") && bk.IsAllowed(tbVName.Text, true, false, false, ".-"))
                {
                    if (cbAb.SelectedItem != null && cbLG.SelectedItem != null)
                    {
                        try
                        {
                            bk.Connection();
                            try
                            {
                                //Variablen
                                bool _tmp;
                                if (CheckFired.IsChecked == true) { _tmp = true; } else _tmp = false;
                                string PANR = cbAb.SelectedItem.ToString(); string PLNR = cbLG.SelectedItem.ToString();
                                //SQL Befehl
                                bk.Update($"UPDATE Personal SET P_VName='{tbVName.Text}',P_NName='{tbNName.Text}',P_Abteilungs_Nr={int.Parse(PANR.Substring(0, PANR.IndexOf("-")).Trim())}, P_Lohngruppen_Nr={int.Parse(PLNR.Substring(0, PLNR.IndexOf("-")).Trim())},P_Deaktiviert={_tmp} WHERE P_Nr = {_Nr}");
                                MessageBox.Show("Die Person wurde erfolgreich gespeichert.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                bk.CloseCon();
                                this.Close();
                            }
                            //catch { this.ShowMessageAsync("", "Die Veränderung konnte nicht gespeichert werden."); bk.CloseCon(); }
                            catch(Exception a) { throw a; }
                        }
                        //catch { this.ShowMessageAsync("", "Die Verbindung konnte nicht hergestellt werden."); bk.CloseCon(); }
                        catch(Exception a) { throw a; }
                    }
                    else this.ShowMessageAsync("", "Die Person muss einer Abteilung und Lohngruppe zugeordnet werden.");
                }
                else this.ShowMessageAsync("", "Es sind keine Sonderzeichen & Zahlen in den Namensfeldern erlaubt.");
            }
            else this.ShowMessageAsync("", "Die Felder dürfen nicht Leer gelassen werden.");
        }
    }
}
