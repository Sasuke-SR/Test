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
    /// Interaktionslogik für Window14.xaml
    /// </summary>
    public partial class Window14 : MetroWindow
    {
        int ID; new string Name; double Satz;
        Basisklasse bk = new Basisklasse();
        public Window14(int nr, string bez, double betrag)
        {
            ID = nr; Name = bez; Satz = betrag;
            InitializeComponent();
        }

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(tbUSName.Text))
            {
                if (!String.IsNullOrWhiteSpace(tbUSBetrag.Text))
                {
                    if (bk.IsAllowed(tbUSName.Text, true, true, false, "-") != false)
                    {
                        if (bk.IsAllowed(tbUSBetrag.Text, false, true, true, "€,.") != false)
                        {
                            try
                            {
                                bk.Connection();
                                try
                                {
                                    bk.Update($"UPDATE UStunden SET US_Bez = '{tbUSName.Text.Trim()}', US_Betrag = {tbUSBetrag.Text.Replace("€", "").Trim().Replace(",", ".")} WHERE US_Nr = {ID}");
                                    MessageBox.Show("Die Überstundengruppe wurde erfolgreich gespeichert.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                    bk.CloseCon();
                                    this.Close();
                                }
                                catch (Exception a) { bk.CloseCon(); throw a; }
                            }
                            catch (Exception a) { throw a; }
                        }
                        else this.ShowMessageAsync("Fehler", "Es dürfen keine Nicht-Numerische Zeichen als Betrag eingegeben werden");
                    }
                    else this.ShowMessageAsync("Fehler", "Im Feld 'Lohngruppennamen' dürfen keine Sonderzeichen vorhanden sein");
                }
                else this.ShowMessageAsync("Fehler", "Es muss ein Stundensatz eingetragen sein");
            }
            else this.ShowMessageAsync("Fehler", "Das Feld 'Lohngruppenname' darf nicht Leer sein");
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            lbUSNr.Content = ID.ToString();
            tbUSName.Text = Name.Trim();
            tbUSBetrag.Text = Satz.ToString() + " €";
        }
    }
}
