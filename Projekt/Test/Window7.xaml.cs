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
    /// Interaktionslogik für Window7.xaml
    /// </summary>
    public partial class Window7 : Window
    {
        Basisklasse bk = new Basisklasse();
        OleDbDataReader dr;
        private class Person
        {
            public int pNr { get; set; }
            public string vName { get; set; }
            public string nName { get; set; }
        }
        public Window7()
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
                    dr = bk.Select("SELECT Last(US_Nr) FROM UStunden;");
                    dr.Read();
                    try {lUeStdNr.Content = dr.GetInt32(0) + 1; }
                    catch {lUeStdNr.Content = 1; }
                    bk.CloseCon();
                }
                catch (Exception ex1) { MessageBox.Show("Fehler beim bestimmen der Überstundengruppen-Nummer", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); Console.WriteLine(ex1); return; }

                try
                {
                    bk.Connection();
                    dr = bk.Select("SELECT US_NR, US_Bez FROM UStunden;");
                    while (dr.Read())
                    {
                        cbUeStdGr.Items.Add($"{dr.GetInt32(0)} - {dr.GetString(1)}");
                    }
                    cbUeStdGr.Items.Refresh();
                    bk.CloseCon();

                }
                catch (Exception ex2) { MessageBox.Show("Fehler beim bestimmen der Überstundengruppen", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); Console.WriteLine(ex2); return; }

                try
                {
                    bk.Connection();
                    try
                    {
                        dr = bk.Select("SELECT P_Nr, P_VName, P_NName FROM Personal;");
                        while (dr.Read())
                        {
                            cbPer.Items.Add($"{dr.GetInt32(0)} - {dr.GetString(2)}, {dr.GetString(1)}");
                        }
                    }
                    catch (Exception ex3) { MessageBox.Show("Fehler beim bestimmen der Personen", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); Console.WriteLine(ex3); return; }
                    cbPer.Items.Refresh();
                    bk.CloseCon();
                }
                catch (Exception ex4) {MessageBox.Show("Die Verbindung zur Datenbank konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error); Console.WriteLine(ex4); bk.CloseCon();}

            }
            catch (Exception ex) { MessageBox.Show("Die Verbindung zur Datenbank konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error); Console.WriteLine(ex); }
        }

        private void bUeStdErs_Click(object sender, RoutedEventArgs e)//Hier wird gearbeitet // Sollte so gehen, konnte aufgrund des Fehles der richtigen DB noch nicht getestet werden
        {
            if (!string.IsNullOrWhiteSpace(cbPer.Text) && !string.IsNullOrWhiteSpace(cbUeStdGr.Text) && !string.IsNullOrWhiteSpace(tbUeStd.Text) && !string.IsNullOrWhiteSpace(dpDat.Text))
            {
                if (bk.IsAllowed(tbUeStd.Text, false, true, false))
                {
                    string tmpPer = cbPer.Text.Split('-')[0].Trim();
                    string tmpUeGr = cbUeStdGr.Text.Split('-')[0].Trim();
                    try
                    {
                        bk.Connection();
                        try
                        {
                            bk.Insert($"INSERT INTO UStunden_2(US2_Datum, US2_Stunden, US2_Personal_Nr, US2_UStunden_Nr) VALUES ({DateTime.Parse(dpDat.Text).ToString("yyyy-MM-dd")}, {tbUeStd.Text}, " +
                                  $"{tmpPer}, {tmpUeGr});");
                        }
                        catch (Exception ex5)
                        { MessageBox.Show("Fehler beim Einfügen in die Datenbank.", "", MessageBoxButton.OK, MessageBoxImage.Error); Console.WriteLine(ex5); bk.CloseCon(); }
                    }
                    catch (Exception ex6)
                    { MessageBox.Show("Die Verbindung zur Datenbank konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error); Console.WriteLine(ex6); }
                }
                else { MessageBox.Show("Das Überstundenfeld enhält ungültige Zeichen.", "", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
            else { MessageBox.Show("Bitte alle Felder ausfüllen.", "", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void bMainWin_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}