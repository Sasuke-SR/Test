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
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;

namespace Test
{
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class Window1 : MetroWindow
    {
        string abDatum;
        int abrechNr;
        OleDbDataReader dr;
        Basisklasse bk = new Basisklasse();
        List<UStunden> items = new List<UStunden>();
        string[] Monate = { "Keine", "Januar", "Februar", "März", "April", "Mai", "Juni", "Juli", "August", "September", "Oktober", "November", "Dezember" };

        public class UStunden
        {
            public string uDatum { get; set; }
            public string uGruppe { get; set; }
            public double uSatz { get; set; }
            public int uStd { get; set; }
            public string uGesamt { get; set; }
        }


        public Window1(int aNr, string Datum)
        {
            abDatum = Datum;
            abrechNr = aNr;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lvUeStdGr.ItemsSource = items;
            // SQL-Abfrage
            try
            {
                bk.Connection();
                try
                {
                    DateTime dp = DateTime.Parse(abDatum);
                    dr = bk.Select($"SELECT * FROM Abrechnung_Datum WHERE DAY(Ab_Datum) = {dp.Day} AND Month(Ab_Datum) = {dp.Month} AND YEAR(Ab_Datum) = {dp.Year} AND Ab_Abrech_Nr = {abrechNr}");
                    dr.Read();
                    if (dr.HasRows)
                    {
                        // Befüllung der Maske
                        dpDatum.Text = dr.GetDateTime(0).ToString("dd/MM/yyyy");
                        lbPNr.Content = dr.GetInt32(3).ToString();
                        tbAstd.Text = dr.GetInt32(1).ToString();
                        OleDbDataReader _tmpDR;
                        _tmpDR = bk.Select($"SELECT * FROM Personal WHERE P_Abrech_Nr = {dr.GetInt32(3)}");
                        _tmpDR.Read();
                        cbPnr.Text = _tmpDR.GetInt32(0).ToString();
                        tbNname.Text = _tmpDR.GetString(2);
                        tbName.Text = _tmpDR.GetString(1);
                        int AbtNr = _tmpDR.GetInt32(3); int LohnNr = _tmpDR.GetInt32(4);
                        _tmpDR.Close();
                        _tmpDR = bk.Select($"SELECT * FROM Abteilung WHERE Abt_Nr = {AbtNr}");
                        _tmpDR.Read();
                        tbAbtNr.Text = AbtNr.ToString();
                        tbAbrName.Text = _tmpDR.GetString(1);
                        _tmpDR.Close();
                        _tmpDR = bk.Select($"SELECT * FROM Lohngruppen WHERE L_Nr = {LohnNr}");
                        _tmpDR.Read();
                        cbLgNr.Text = _tmpDR.GetInt32(0).ToString() + " - " + _tmpDR.GetString(1);
                        tbLgName.Text = _tmpDR.GetString(1);
                        tbLgBet.Text = _tmpDR.GetDouble(2).ToString("C");
                        double lSatz = _tmpDR.GetDouble(2);
                        _tmpDR.Close();
                        _tmpDR = bk.Select($"SELECT * FROM Bonus WHERE B_Nr = {dr.GetInt32(2)}");
                        _tmpDR.Read();
                        if (_tmpDR.GetInt32(0) == 0)
                        { tbBonusText.Text = "Kein Bonus vorhanden"; tbBonusSum.Text = "0 %"; }
                        else
                        {
                            tbBonusText.Text = $"Bonus für {Monate[_tmpDR.GetInt32(3)]}";
                            tbBonusSum.Text = _tmpDR.GetDouble(2).ToString() + " %";
                        }
                        double SummeRegel = lSatz * double.Parse(tbAstd.Text);
                        tbRaStdSum.Text = SummeRegel.ToString("C");
                        _tmpDR.Close();
                        _tmpDR = bk.Select($"SELECT * FROM UStunden2 WHERE DAY(US2_Datum) = {dp.Day} AND Month(US2_Datum) = {dp.Month} AND YEAR(US2_Datum) = {dp.Year} AND US2_Abrech_Nr = {abrechNr}");
                        double _tmpUG = 0;
                        while (_tmpDR.Read())
                        {
                            OleDbDataReader ab = bk.Select($"SELECT * FROM UStunden WHERE US_Nr = {_tmpDR.GetInt32(1)}");
                            ab.Read();
                            double uG = double.Parse(_tmpDR.GetInt32(2).ToString()) * _tmpDR.GetDouble(4);
                            _tmpUG += uG;
                            items.Add(new UStunden() {uDatum = _tmpDR.GetDateTime(0).ToString("dd/MM/yyyy"), uGruppe = ab.GetString(1), uSatz = _tmpDR.GetDouble(4), uStd = _tmpDR.GetInt32(2), uGesamt = uG.ToString("C")});
                        }
                        tbUeStdSum2.Text = _tmpUG.ToString("C");
                        double Brutto = SummeRegel + _tmpUG;
                        tbBrutto.Text = Brutto.ToString("C");
                        double Bonus = double.Parse(tbBonusSum.Text.Replace("%", "").Trim()); double mBonus = Brutto / 100; double eBonus = mBonus * Bonus; double Entlohnung = Brutto + eBonus;
                        tbEndLohn.Text = Entlohnung.ToString("C");
                    }
                    else { this.ShowMessageAsync("Fehler","Es wurde keine Abrechnung mit diesen Informationen gefunden"); this.Close(); }
                }
                catch(Exception a) { bk.CloseCon(); throw a; }
            }
            catch(Exception a) { bk.CloseCon(); throw a; }
        }

        private void bHaupt_Click_1(object sender, RoutedEventArgs e) => this.Close();

        private void lvUeStdGr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvUeStdGr.SelectedItem != null)
            {
                foreach (UStunden item in lvUeStdGr.SelectedItems)
                {
                    cbUGruppe.Text = item.uGruppe.ToString();
                    tbUeStdBet.Text = item.uSatz.ToString("C");
                    tbUeStdAnz.Text = item.uGruppe.ToString();
                    dpUDatum.Text = item.uDatum.ToString();
                }
            }
        }
    }
}
