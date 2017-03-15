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
                    int i = 0;
                    dr = bk.Select("SELECT * FROM Abteilung");
                    while (dr.Read())
                    { cbAb.Items.Insert(i, dr.GetString(1)); i++; }

                    i = 0;
                    dr = bk.Select("SELECT * FROM Lohngruppen");
                    while (dr.Read())
                    { cbLG.Items.Insert(i, dr.GetString(1)); i++; }
                    // Boxen füllen
                    dr = bk.Select($"SELECT * FROM Personal WHERE P_Nr = {_Nr}");
                    while (dr.Read())
                    {
                        tbNName.Text = dr.GetString(2);
                        tbVName.Text = dr.GetString(1);
                        if (dr.GetBoolean(5) == true)
                            CheckFired.IsChecked = true;
                        cbAb.SelectedIndex = dr.GetInt32(3) - 1;
                        cbLG.SelectedIndex = dr.GetInt32(4) - 1;
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
                                bool _tmp;
                                if (CheckFired.IsChecked == true) { _tmp = true; } else _tmp = false;
                                bk.Update($"UPDATE Personal SET P_VName='{tbVName.Text}',P_NName='{tbNName.Text}',P_Abteilungs_Nr={cbAb.SelectedIndex + 1},P_Lohngruppen_Nr={cbLG.SelectedIndex + 1},P_Fired={_tmp} WHERE P_Nr = {_Nr}");
                                MessageBox.Show("Die Person wurde erfolgreich gespeichert.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                bk.CloseCon();
                                this.Close();
                            }
                            catch { this.ShowMessageAsync("", "Die Veränderung konnte nicht gespeichert werden."); bk.CloseCon(); }
                        }
                        catch { this.ShowMessageAsync("", "Die Verbindung konnte nicht hergestellt werden."); bk.CloseCon(); }
                    }
                    else this.ShowMessageAsync("", "Die Person muss einer Abteilung und Lohngruppe zugeordnet werden.");
                }
                else this.ShowMessageAsync("", "Es sind keine Sonderzeichen & Zahlen in den Namensfeldern erlaubt.");
            }
            else this.ShowMessageAsync("", "Die Felder dürfen nicht Leer gelassen werden.");
        }
    }
}
