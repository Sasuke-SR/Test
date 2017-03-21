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
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;

namespace Test
{
    /// <summary>
    /// Interaktionslogik für Window13.xaml
    /// </summary>
    public partial class Window13 : MetroWindow
    {
        Basisklasse bk = new Basisklasse();
        int ID; string Name; double Satz;

        public Window13(int nr, string bez, double betrag)
        {
            ID = nr; Name = bez; Satz = betrag;
            InitializeComponent();
        }

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(tbLGName.Text))
            {
                if (!String.IsNullOrWhiteSpace(tbLGBetrag.Text))
                {
                    if (bk.IsAllowed(tbLGName.Text, true, true, false, "-") != false)
                    {
                        if (bk.IsAllowed(tbLGBetrag.Text, false, true, false, "€,.") != false)
                        {
                            try
                            {
                                bk.Connection();
                                try
                                {
                                    bk.Update($"UPDATE Lohngruppen SET L_Bez = '{tbLGName.Text.Trim()}', L_Lohn = {tbLGBetrag.Text.Replace("€", "").Trim().Replace(",", ".")} WHERE L_Nr = {ID}");
                                    this.ShowMessageAsync("Erfolgreich", "Die Lohngruppe wurde erfolgreich bearbeitet");
                                    bk.CloseCon();
                                    this.Close();
                                }
                                catch (Exception a) { bk.CloseCon(); throw a; }
                            }
                            catch(Exception a) { throw a; }
                        }
                        else this.ShowMessageAsync("Fehler","Es dürfen keine Nicht-Numerische Zeichen als Betrag eingegeben werden");
                    }
                    else this.ShowMessageAsync("Fehler","Im Feld 'Lohngruppennamen' dürfen keine Sonderzeichen vorhanden sein");
                }
                else this.ShowMessageAsync("Fehler","Es muss ein Stundensatz eingetragen sein");
            }
            else this.ShowMessageAsync("Fehler","Das Feld 'Lohngruppenname' darf nicht Leer sein");
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            lbLGNr.Content = ID.ToString();
            tbLGName.Text = Name.Trim();
            tbLGBetrag.Text = Satz.ToString() + " €";
        }
    }
}
