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
    public partial class Window7 : MetroWindow
    {
        Basisklasse bk = new Basisklasse();
        OleDbDataReader dr;
        OleDbDataReader dr2;
        OleDbDataReader dr3;

        public class UStunden
        {
            public string uNr { get; set; }
            public string uPersonal { get; set; }
            public string uStunden { get; set; }
            public string uGruppe { get; set; }
            public string uGStunden { get; set; }
        }

        public void ListView_Load()
        {
            dr = bk.Select($"SELECT * FROM UStunden_2");
            List<UStunden> items = new List<UStunden>();
            try
            {
                while(dr.Read())
                {
                    dr2 = bk.Select($"SELECT * FROM Personal WHERE {dr.GetInt32(2)}"); dr2.Read();
                    dr3 = bk.Select($"SELECT * FROM UStunden WHERE US_Nr = {dr.GetInt32(3)}"); dr3.Read();
                    items.Add(new UStunden() { uNr = dr.GetInt32(0).ToString(), uPersonal = dr2.GetString(1) + ", " + dr2.GetString(2), uStunden = dr.GetInt32(4).ToString(), uGruppe = dr3.GetString(1), uGStunden = dr3.GetDouble(2).ToString() + " €"});
                }
            }
            catch (Exception a) { throw a; }
            lvUeStd.ItemsSource = items;
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
                    dr = bk.Select("SELECT Last(US2_Nr) FROM UStunden_2;");
                    dr.Read();
                    try {lUeStdNr.Content = dr.GetInt32(0) + 1; }
                    catch {lUeStdNr.Content = 1; }
                    ListView_Load();
                    bk.CloseCon();
                }
                catch (Exception ex1) { MessageBox.Show("Fehler beim bestimmen der Überstundengruppen-Nummer", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); Console.WriteLine(ex1); return; }
            }
            catch (Exception ex) { MessageBox.Show("Die Verbindung zur Datenbank konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error); Console.WriteLine(ex); }
        }

        private void bUeStdErs_Click(object sender, RoutedEventArgs e)
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