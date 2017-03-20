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
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    /// 
    public partial class Window2 : MetroWindow
    {
        Basisklasse bk = new Basisklasse();
        OleDbDataReader dr;
        int aMonth = 0;
        double pSatz = 0;
        int bNr = 0;
        string[] Monate = { "Keine", "Januar", "Februar", "März", "April", "Mai", "Juni", "Juli", "August", "September", "Oktober", "November", "Dezember" };

        #region Class
        public class uStunden
        {
            public string uGruppe { get; set; }
            public string ugBetrag { get; set;  }
            public int uAStunden { get; set; }
            public string uDatum { get; set; }
            public string uSumme { get; set; }
            public int uPersonalNr { get; set; }
        }
        List<uStunden> items = new List<uStunden>();

        #endregion

        #region Methoden
        public void Active_Month()
        {
            try
            {
                bk.Connection();
                try
                {
                    DateTime Datum = Convert.ToDateTime(dpDatum.Text);
                    dr = bk.Select($"SELECT * FROM Bonus WHERE B_Aktiv = true AND B_Monat = {Datum.Month}");
                    dr.Read();
                    try
                    {
                        if (dr.HasRows)
                        {
                            aMonth = dr.GetInt32(3);
                            pSatz = dr.GetDouble(2);
                            bNr = dr.GetInt32(0);
                        }
                        else { aMonth = Datum.Month; pSatz = 0; bNr = 0; }
                        bk.CloseCon();
                    }
                    catch (Exception a) { throw a; }
                }
                catch (Exception a) { bk.CloseCon(); throw a; }
            }
            catch(Exception a) { throw a; }
        }
        public double Calculate_uSumme()
        {
            double _tmp = 0;
            foreach (uStunden item in lvUeStdGr.Items)
            { _tmp += double.Parse(item.uSumme.Trim().Replace("€","")); }
            return _tmp;
        }
        public void Load_ComboBox()
        {
            // Personal wird geladen
            dr = bk.Select("SELECT * FROM Personal WHERE P_Deaktiviert = 0");
            string _tmp = "";
            while (dr.Read())
            {
                if (dr.GetInt32(0) < 10){_tmp = "00" + dr.GetInt32(0).ToString();}
                else if (dr.GetInt32(0) > 9 && dr.GetInt32(0) < 100) {_tmp = "0" + dr.GetInt32(0).ToString();}
                else {_tmp = dr.GetInt32(0).ToString();}
                cbPnr.Items.Add($"{_tmp}" + " - " + $"{dr.GetString(1)}" + " " + $"{dr.GetString(2)}");
            }
            // Überstundengruppen werden geladen
            dr = bk.Select("SELECT * FROM UStunden");
            while (dr.Read())
            {
                if (dr.GetInt32(0) < 10) { _tmp = "0" + dr.GetInt32(0).ToString(); }
                else { _tmp = dr.GetInt32(0).ToString(); }
                cbUGruppe.Items.Add($"{_tmp}" + " - " + $"{dr.GetString(1)}");
            }
        }
        #endregion


        public Window2()
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
                    Load_ComboBox();
                    lvUeStdGr.ItemsSource = items;
                    bk.CloseCon();
                }
                catch (Exception a) { bk.CloseCon(); throw a; }
            }
            catch { MessageBox.Show("Es konnte keine Verbindung erstellt werden.", ""); }
        }

        private void bHaupt_Click_1(object sender, RoutedEventArgs e) => this.Close();

        private void cbPnr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPnr.SelectedItem != null)
            {
                string _tmp = cbPnr.SelectedItem.ToString();
                string pNr = _tmp.Substring(0, _tmp.IndexOf('-')).Trim();
                try
                {
                    bk.Connection();
                    try
                    {
                        cbLgNr.SelectedIndex = -1; cbLgNr.Items.Clear();
                        dr = bk.Select($"SELECT * FROM Personal WHERE P_Nr = {pNr}");
                        dr.Read(); int lNr = dr.GetInt32(4);
                        tbName.Text = dr.GetString(1); tbNname.Text = dr.GetString(2); tbAbtNr.Text = dr.GetInt32(3).ToString();
                        if (dr.GetInt32(4) < 10) { lbPNr.Content = "00" + dr.GetInt32(6).ToString(); } else if (dr.GetInt32(4) > 10 && dr.GetInt32(4) < 100) { lbPNr.Content = "0" + dr.GetInt32(6).ToString(); }
                        dr = bk.Select($"SELECT * From Abteilung WHERE Abt_Nr = {dr.GetInt32(3)}");
                        dr.Read();
                        tbAbrName.Text = dr.GetString(1);
                        dr = bk.Select($"SELECT * FROM Lohngruppen WHERE L_Nr = {lNr}");
                        dr.Read();
                        cbLgNr.Items.Add($"{dr.GetInt32(0)} - {dr.GetString(1)}");
                        bk.CloseCon();
                        cbLgNr.SelectedIndex = 0;
                    }
                    catch(Exception a) { bk.CloseCon(); throw a; }
                }
                catch(Exception a) { bk.CloseCon(); throw a; }
            }
        }

        private void cbLgNr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbLgNr.SelectedItem != null)
            {
                string _tmp = cbLgNr.SelectedItem.ToString();
                string lgNr = _tmp.Substring(0, _tmp.IndexOf('-')).Trim();
                try
                {
                    bk.Connection();
                    try
                    {
                        dr = bk.Select($"SELECT * FROM Lohngruppen WHERE L_Nr = {lgNr}");
                        dr.Read();
                        tbLgName.Text = dr.GetString(1); tbLgBet.Text = dr.GetDouble(2).ToString() + " €";
                        bk.CloseCon();
                    }
                    catch (Exception a) { bk.CloseCon(); throw a; }
                }
                catch (Exception a) { bk.CloseCon(); throw a; }
                
                if (tbAstd.Text != "")
                {
                    tbRaStdSum.Text = "";
                    string _stmp = tbLgBet.Text;
                    double _dtmp;
                    try { _dtmp = double.Parse(tbAstd.Text) * double.Parse(_stmp.Substring(0, _stmp.IndexOf("€")).Trim()); }
                    catch { _dtmp = 0; }
                    tbRaStdSum.Text = _dtmp.ToString("C");
                }
            }
        }

        private void tbAstd_PreviewTextInput(object sender, TextCompositionEventArgs e) => e.Handled = !e.Text.Any(x => char.IsDigit(x));

        private void tbAstd_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (cbLgNr.SelectedItem != null)
            {
                tbRaStdSum.Text = "";
                string _stmp = tbLgBet.Text;
                double _dtmp;
                try { _dtmp = double.Parse(tbAstd.Text) * double.Parse(_stmp.Substring(0, _stmp.IndexOf("€")).Trim()); }
                catch { _dtmp = 0; }
                tbRaStdSum.Text = _dtmp.ToString("C");
            }
        }

        private void tbUeStdAnz_PreviewTextInput(object sender, TextCompositionEventArgs e) => e.Handled = !e.Text.Any(x => char.IsDigit(x));

        private void dpUDatum_GotMouseCapture(object sender, MouseEventArgs e) => dpUDatum.IsDropDownOpen = true;

        private void dpUDatum_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => dpUDatum.IsDropDownOpen = false;

        private void cbUGruppe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbUGruppe.SelectedItem != null)
            {
                string _stmp = cbUGruppe.SelectedItem.ToString();
                string _Nr = _stmp.Substring(0, _stmp.IndexOf("-")).Trim();
                try
                {
                    bk.Connection();
                    try
                    {
                        dr = bk.Select($"SELECT * FROM UStunden WHERE US_Nr = {_Nr}");
                        dr.Read();
                        tbUeStdBet.Text = dr.GetDouble(2).ToString() + " €";
                        bk.CloseCon();
                    }
                    catch (Exception a) { bk.CloseCon(); throw a; }
                }
                catch(Exception a) { bk.CloseCon(); throw a; }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (cbPnr.SelectedItem != null)
            {
                if (cbUGruppe.SelectedItem != null)
                {
                    if (!string.IsNullOrWhiteSpace(tbUeStdAnz.Text.Trim()))
                    {
                        if (!string.IsNullOrWhiteSpace(dpUDatum.Text))
                        {
                            if (!string.IsNullOrWhiteSpace(dpDatum.Text))
                            {
                                DateTime UDatum = Convert.ToDateTime(dpUDatum.Text);
                                DateTime Datum = Convert.ToDateTime(dpDatum.Text);
                                if (UDatum.Month == Datum.Month && UDatum.Year == Datum.Year)
                                {
                                    bool _bTmp = false;
                                    if (items.Count != 0)
                                    {
                                        foreach (uStunden ustd in items)
                                        {
                                            DateTime _dp = DateTime.Parse(ustd.uDatum);
                                            if (_dp == UDatum) { _bTmp = true; break; }
                                            else { _bTmp = false; }
                                        }
                                    }
                                    else _bTmp = false;

                                    if (_bTmp == false)
                                    {
                                        lvUeStdGr.ItemsSource = null;
                                        try
                                        {
                                            lvUeStdGr.ItemsSource = null;
                                            string _bet = tbUeStdBet.Text.Trim(); double a = double.Parse(tbUeStdAnz.Text.Trim()) * double.Parse(_bet.Replace("€", "").Trim());
                                            items.Add(new uStunden() { uGruppe = cbUGruppe.SelectedItem.ToString(), ugBetrag = _bet, uAStunden = int.Parse(tbUeStdAnz.Text.Trim()), uDatum = DateTime.Parse(dpUDatum.Text.Trim()).ToString("dd/MM/yyyy"), uSumme = a.ToString("C"), uPersonalNr = int.Parse(lbPNr.Content.ToString().Trim()) });
                                        }
                                        catch (Exception a) { throw a; }
                                        lvUeStdGr.ItemsSource = items;
                                        tbUeStdAnz.Text = ""; tbUeStdSum2.Text = ""; tbUeStdBet.Text = ""; cbUGruppe.SelectedItem = null; dpUDatum.Text = "";
                                        tbUeStdSum2.Text = Calculate_uSumme().ToString("C");
                                    }
                                    else this.ShowMessageAsync("Fehler", "Es darf nur eine Überstunde pro Tag erfolgen");
                                }
                                else this.ShowMessageAsync("Fehler", "Sie dürfen nur die Überstunden im selben Monat und Jahr wie im Abrechnungs Datum nutzen");
                            }
                            else this.ShowMessageAsync("Fehler", "Sie haben kein Abrechnungs Datum ausgewählt");
                        }
                        else this.ShowMessageAsync("Fehler","Sie haben kein Datum ausgewählt");
                    }
                    else this.ShowMessageAsync("Fehler", "Sie haben keine Stundenanzahl eingegeben.");
                }
                else this.ShowMessageAsync("Fehler", "Sie haben keine Überstundengruppe ausgewählt.");
            }
            else this.ShowMessageAsync("Fehler", "Personal wurde nicht ausgewählt");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (lvUeStdGr.SelectedItem != null)
            {
                items.Remove(lvUeStdGr.SelectedItem as uStunden);
                lvUeStdGr.Items.Refresh();
    
                try
                {
                    double a;
                    if (string.IsNullOrWhiteSpace(tbRaStdSum.Text)) { a = 0; } else a = double.Parse(tbRaStdSum.Text.Replace("€", "").Trim());
                    double _tmp = double.Parse(tbUeStdSum2.Text.Replace("€", "").Trim()) + a;
                    tbBrutto.Text = _tmp.ToString("C");
                    tbUeStdSum2.Text = Calculate_uSumme().ToString("C");
                }
                catch (Exception a) { throw a; }
            }
            else this.ShowMessageAsync("Fehler","Sie haben keine Überstunden ausgewählt zum Löschen");
        }

        private void tbRaStdSum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbRaStdSum.Text))
            {
                try
                {
                    double a;
                    if (string.IsNullOrWhiteSpace(tbUeStdSum2.Text)) { a = 0; } else a = double.Parse(tbUeStdSum2.Text.Replace("€", "").Trim());
                    double _tmp = double.Parse(tbRaStdSum.Text.Replace("€", "").Trim()) + a;
                    tbBrutto.Text = _tmp.ToString("C");
                }
                catch (Exception a) { throw a; }
            }
        }

        private void tbUeStdSum2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbUeStdSum2.Text))
            {
                try
                {
                    double a;
                    if (string.IsNullOrWhiteSpace(tbRaStdSum.Text)) { a = 0; } else a = double.Parse(tbRaStdSum.Text.Replace("€", "").Trim());
                    double _tmp = double.Parse(tbUeStdSum2.Text.Replace("€", "").Trim()) + a;
                    tbBrutto.Text = _tmp.ToString("C");
                }
                catch (Exception a) { throw a; }
            }
        }

        private void dpDatum_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime tmp_date = Convert.ToDateTime(dpDatum.Text);
            Active_Month();
            if (tmp_date.Month == aMonth)
            {
                tbBonusSum.Text = pSatz.ToString() + "%";
                tbBonusText.Text = $"Bonus für {Monate[aMonth]}";

                if (lvUeStdGr.Items.Count != 0)
                {
                    items.Clear();
                    lvUeStdGr.Items.Refresh();
                    this.ShowMessageAsync("","Die Überstunden wurden gelöscht weil das Datum verändert wurde");
                }
            }
            else { tbBonusSum.Text = "0 %"; tbBonusText.Text = "Kein Bonus vorhanden"; }
        }

        private void tbBrutto_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbBonusSum.Text) && !string.IsNullOrWhiteSpace(tbBrutto.Text))
            {
                try
                {
                    double a = double.Parse(tbBrutto.Text.Replace("€", "").Trim()); double b = double.Parse(tbBonusSum.Text.Replace("%", "").Replace(",", ".").Trim()) / 100;
                    double c = a * b; double _tmp = a + c;
                    tbEndLohn.Text = _tmp.ToString("C");
                }
                catch(Exception a) { throw a; }
            }
        }

        private void bAbrErs_Click(object sender, RoutedEventArgs e)
        {
            if (cbPnr.SelectedItem != null)
            {
                if (cbLgNr.SelectedItem != null)
                {
                    if (!string.IsNullOrWhiteSpace(dpDatum.Text))
                    {
                        if (!string.IsNullOrWhiteSpace(tbEndLohn.Text))
                        {
                            if (!string.IsNullOrWhiteSpace(tbAstd.Text))
                            {
                                //Abfrage ob die Abrechnung in der Datenbank existiert
                                try
                                {
                                    bk.Connection();
                                    try
                                    {
                                        DateTime Datum = DateTime.Parse(dpDatum.Text);
                                        dr = bk.Select($"SELECT * FROM Abrechnung_Datum WHERE YEAR(Ab_Datum)='{Datum.Year}' AND MONTH(Ab_Datum) = '{Datum.Month}' AND Ab_Abrech_Nr = {lbPNr.Content}");
                                        dr.Read();
                                        try
                                        {
                                            if (dr.HasRows) { bk.CloseCon(); this.ShowMessageAsync("Fehler", "Es wurde eine Abrechnung mit der gleichen AbrechnungsNr und dem gleichen Monat gefunden"); }
                                            else
                                            {
                                                // Lohnabrechnung erstellen
                                                string _tmp = cbLgNr.SelectedItem.ToString();
                                                dr = bk.Select($"SELECT * FROM Lohngruppen WHERE L_Nr = {_tmp.Substring(0, _tmp.IndexOf("-")).Trim()}"); dr.Read();
                                                bk.Insert($"INSERT INTO Abrechnung_Datum(Ab_Datum, Ab_AStunden, Ab_Bonus_Nr, Ab_Abrech_Nr, Ab_Lohngruppe_Nr, Ab_Lohngruppe_Satz) VALUES('{Datum}',{int.Parse(tbAstd.Text)},{bNr},{int.Parse(lbPNr.Content.ToString())},{int.Parse(_tmp.Substring(0, _tmp.IndexOf("-")).Trim())},'{dr.GetDouble(2)}')");
                                                System.Threading.Thread.Sleep(500);
                                                try
                                                {
                                                    Console.WriteLine("Lohnabrechnung erstellt");
                                                    try
                                                    {
                                                        //Überstunden in die Datenbank eintragen
                                                        foreach (uStunden ustd in items)
                                                        {
                                                            string _tmp2 = ustd.uGruppe.ToString(); Console.WriteLine(_tmp2); Console.WriteLine(ustd.uDatum);
                                                            dr = bk.Select($"SELECT * FROM UStunden WHERE US_Nr = {int.Parse(_tmp2.Substring(0, _tmp2.IndexOf("-")).Trim())}"); dr.Read();
                                                            bk.Insert($"INSERT INTO UStunden2 (US2_Datum,US2_US_Nr,US2_Stunden,US2_Abrech_Nr,US2_US_Satz) VALUES ('{Datum}',{int.Parse(_tmp2.Substring(0, _tmp2.IndexOf("-")).Trim())},{ustd.uAStunden},{int.Parse(lbPNr.Content.ToString())}, '{dr.GetDouble(2)}')");

                                                            try { Console.WriteLine("Überstunden erstellt"); }
                                                            catch (Exception a) { bk.CloseCon(); throw a; }
                                                        }
                                                        this.ShowMessageAsync("Erfolgreich", "Die Lohnabrechnung konnte erstellt werden");
                                                        bk.CloseCon();
                                                        this.Close();
                                                    }
                                                    catch (Exception a) { bk.CloseCon(); Console.WriteLine(a); throw a; } // {"Der Datensatz kann nicht hinzugefügt oder geändert werden, da ein Datensatz in der Tabelle 'Abrechnung_Datum' mit diesem Datensatz in Beziehung stehen muss."}
                                                }
                                                catch { this.ShowMessageAsync("Fehler", "Die Lohnabrechnung konnte nicht erstellt werden"); }
                                                //catch (Exception a) { bk.CloseCon(); throw a; }
                                            }
                                        }
                                        catch (Exception a) { bk.CloseCon(); throw a; }
                                    }
                                    catch (Exception a) { bk.CloseCon(); throw a; }
                                }
                                catch {  this.ShowMessageAsync("Fehler","Es konnte keine Verbindung hergestellt werden"); }
                                //catch (Exception a) { this.ShowMessageAsync("", a.ToString()); Console.WriteLine(a); }
                            }
                            else this.ShowMessageAsync("Fehler","Es müssen Arbeits Stunden angegeben werden");
                        }
                        else this.ShowMessageAsync("Fehler","Es wurden nicht alle Felder ausgefüllt");
                    }
                    else this.ShowMessageAsync("Fehler", "Es wurde kein Abrechnungsdatum ausgewählt");
                }
                else this.ShowMessageAsync("Fehler", "Es wurde keine Lohngruppe ausgewählt");
            }
            else this.ShowMessageAsync("Fehler","Es wurde kein Personal ausgewählt");
        }
    }
}
