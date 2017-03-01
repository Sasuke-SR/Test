using System;
using System.Collections.Generic;
using System.Data.OleDb;
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
using MahApps.Metro.Controls;

namespace Test
{
    /// <summary>
    /// Interaktionslogik für Window6.xaml
    /// </summary>
    public partial class Window6 : MetroWindow
    {
        Basisklasse bk = new Basisklasse();
        OleDbDataReader dr;

        public class UStundenGr
        {
            public int Nr { get; set; }
            public string Bezeichnung { get; set; }
            public string Betrag { get; set; }
        }

        public Window6()
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
                    figureOutNr();
                    bk.CloseCon();
                }
                catch
                { MessageBox.Show("Fehler beim bestimmen der Überstundengruppennummer", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); return; }

                try
                {
                    bk.Connection();
                    try
                    {
                        fillLv();
                    }
                    catch { MessageBox.Show("Fehler beim bestimmen existierender Überstundengruppen", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); return; }
                    bk.CloseCon();
                }
                catch { MessageBox.Show("Die Verbindung zur Datenbank konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
            catch { MessageBox.Show("Die Verbindung zur Datenbank konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void bMainWin_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void bGrErs_Click(object sender, RoutedEventArgs e)
        {
            if (bk.IsAllowed(tbUeBez.Text, true, true, true) && bk.IsAllowed(tbUeBet.Text, false, true, false, ",.€"))
            {
                if (!String.IsNullOrWhiteSpace(tbUeBet.Text) && !String.IsNullOrWhiteSpace(tbUeBez.Text))
                {
                    try
                    {
                        bk.Connection();
                        try
                        {
                            bk.Insert($"INSERT INTO UStunden (US_Bez, US_Betrag) VALUES ('{tbUeBez.Text}', {tbUeBet.Text.Replace(',', '.').Replace("€", "").Trim()});");
                            MessageBox.Show("Die Überstundengruppe wurde erfolgreich erstellt.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            try
                            {
                                lvUeGr.ItemsSource = null;
                                fillLv();
                                tbUeBet.Text = "";
                                tbUeBez.Text = "";
                                figureOutNr();
                                bk.CloseCon();
                            }
                            catch { MessageBox.Show("Es ist ein Fehler aufgetreten","",MessageBoxButton.OK,MessageBoxImage.Error); bk.CloseCon(); }
                        }
                        catch { MessageBox.Show("Fehler beim Einfügen in die Datenbank", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); return; }
                    }
                    catch { MessageBox.Show("Die Verbindung zur Datenbank konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error); }
                }
                else MessageBox.Show("Die Eingabe Felder dürfen nicht Leer sein.\n", "");
            }
            else {MessageBox.Show("In der Bezeichnung oder dem Betrag sind unzulässige Zeichen vorhanden.", ""); }


        }

        private void fillLv()
        {
            dr = bk.Select("SELECT * FROM UStunden;");
            List<UStundenGr> usgListe = new List<UStundenGr>();
            while (dr.Read())
            {
                UStundenGr UsGr = new UStundenGr() { Nr = dr.GetInt32(0), Bezeichnung = dr.GetString(1), Betrag = dr.GetDouble(2).ToString("C") };
                usgListe.Add(UsGr);
            }
            lvUeGr.ItemsSource = usgListe;
            lvUeGr.Items.Refresh();
        }

        private void figureOutNr()
        {
            dr = bk.Select("SELECT Last(US_Nr) FROM UStunden;");
            dr.Read();
            try
            {
                lUeGrNr.Content = dr.GetInt32(0) + 1;
            }
            catch
            {
                lUeGrNr.Content = 1;
            }
        }
    }
}