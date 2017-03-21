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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.OleDb;
using System.Collections;
using System.ComponentModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace Test
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        Basisklasse bk = new Basisklasse();
        OleDbDataReader dr;
        OleDbDataReader dr1;
        OleDbDataReader dr2;
        OleDbDataReader dr3;

        #region Public Klassen
        public class Personal
        {
            public int pNr { get; set; }
            public string pVName { get; set; }
            public string pNName { get; set; }
            public string pAbteilung { get; set; }
            public string pStatus { get; set; }
        }

        public class Abteilungen
        {
            public int aNr { get; set; }
            public string aName { get; set; }
        }

        public class Lohnabrechnung
        {
            public string laDatum { get; set; }
            public int laNr { get; set; }
            public string laPerson { get; set; }
            public string laBrutto { get; set; }
            public string laEndlohn { get; set; }
        }

        public class UStunden
        {
            public string uDatum { get; set; }
            public string uGruppe { get; set; }
            public int uStd { get; set; }
            public string uPersonal { get; set; }
            public string uGesamt { get; set; }
        }
        #endregion

        #region ListView's
        private void uListView_Load()
        {
            dr = bk.Select("SELECT * FROM UStunden2 ORDER BY US2_Datum ASC");
            List<UStunden> US = new List<UStunden>();
            try
            {
                while (dr.Read())
                {
                    dr2 = bk.Select($"SELECT * FROM UStunden WHERE US_Nr = {dr.GetInt32(1)}"); dr2.Read();
                    dr3 = bk.Select($"SELECT * FROM Personal WHERE P_Abrech_Nr = {dr.GetInt32(3)}"); dr3.Read();
                    DateTime datum = dr.GetDateTime(0);
                    double gesamt = Convert.ToDouble(dr.GetInt32(2)) * dr.GetDouble(4);
                    US.Add(new UStunden() { uDatum = datum.ToString("dd/MM/yyyy"), uGruppe = dr2.GetString(1), uStd = dr.GetInt32(1), uPersonal = $"{dr3.GetInt32(0).ToString()} - {dr3.GetString(2)}, {dr3.GetString(1)}", uGesamt = gesamt.ToString("C") });
                }
            }
            catch (Exception a) { throw a; }
            list2.ItemsSource = US;
        }

        private void laListView_Load()
        {
            dr = bk.Select("SELECT * FROM Abrechnung_Datum ORDER BY Ab_Datum ASC");
            List<Lohnabrechnung> LA = new List<Lohnabrechnung>();
            try
            {
                while (dr.Read())
                {
                    if (dr.HasRows)
                    {
                        dr1 = bk.Select($"SELECT * FROM Personal WHERE P_Abrech_Nr = {dr.GetInt32(3)}");
                        dr2 = bk.Select($"SELECT * FROM Lohngruppen WHERE L_Nr = {dr.GetInt32(4)}");
                        dr1.Read(); dr2.Read();
                        double brutto = Convert.ToDouble(dr.GetInt32(1)) * dr2.GetDouble(2); Console.WriteLine(brutto);
                        double endlohn = 0; double uSumme = 0; double pSatz = 0;
                        dr2.Close();
                        dr2 = bk.Select($"SELECT * FROM Bonus WHERE B_Nr = {dr.GetInt32(2)}"); dr2.Read(); pSatz = dr2.GetDouble(2); dr2.Close();
                        //Endlohn -> Rechnung -> Anfang
                        DateTime datum = dr.GetDateTime(0);
                        dr2 = bk.Select($"SELECT * FROM UStunden2 WHERE YEAR(US2_Datum) = '{datum.Year}' AND MONTH(US2_Datum) = '{datum.Month}' AND US2_Abrech_Nr = {dr.GetInt32(3)}");
                        while (dr2.Read())
                        {
                            if (dr2.HasRows)
                            {
                                uSumme += dr2.GetDouble(4) * Convert.ToDouble(dr2.GetInt32(2));
                                Console.WriteLine(uSumme);
                            }
                            else uSumme += 0;
                        }
                        double BSumme = (brutto / 100) * pSatz; double bGesamt = brutto + BSumme;
                        endlohn = uSumme + bGesamt;
                        //Endlohn -> Rechnung -> Ende
                        LA.Add(new Lohnabrechnung() { laDatum = dr.GetDateTime(0).ToString("dd/MM/yyyy"), laNr = dr.GetInt32(3), laPerson = dr1.GetString(2) + ", " + dr1.GetString(1), laBrutto = brutto.ToString("C"), laEndlohn = endlohn.ToString("C") });
                    }
                }
            }
            catch (Exception a) { throw a; }
            list.ItemsSource = LA;
        }

        private void pListView_Load()
        {
            if (Properties.Settings.Default.Setting == true) { dr = bk.Select("SELECT * FROM Personal");}
            else { dr = bk.Select("SELECT * FROM Personal WHERE P_Deaktiviert = false"); }
            List<Personal> items = new List<Personal>();
            try
            {
                while (dr.Read())
                {
                    string _tmp = "";
                    dr1 = bk.Select($"SELECT * FROM Abteilung WHERE Abt_Nr = {dr.GetInt32(3)}");
                    dr1.Read();
                    if (dr.GetBoolean(5) == true) { _tmp = "Entlassen"; } else _tmp = "Angestellt";
                    items.Add(new Personal() { pNr = dr.GetInt32(0), pVName = dr.GetString(1), pNName = dr.GetString(2), pAbteilung = dr1.GetString(1), pStatus = _tmp });
                }
            }
            catch (Exception a) { throw a; }
            list3.ItemsSource = items;
        }
        #endregion


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window2 nAbrechnung = new Window2();
            nAbrechnung.ShowDialog();
            try
            {
                bk.Connection();
                try
                {
                    laListView_Load();
                    uListView_Load();
                    bk.CloseCon();
                }
                catch (Exception a) { bk.CloseCon(); throw a; }
            }
            catch (Exception a) { bk.CloseCon(); throw a; }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                bk.Connection();
                try
                {
                    pListView_Load();
                    laListView_Load();
                    uListView_Load();
                    bk.CloseCon();
                }
                catch { this.ShowMessageAsync("Fehler", "Die Listen konnten nicht geladen werden"); bk.CloseCon(); }
            }
            catch { this.ShowMessageAsync("Titel", "Die Liste konnte nicht geldaden werden."); bk.CloseCon(); /*MessageBox.Show("Die Verbindung konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error);*/ }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Window3 nLohnG = new Window3();
            nLohnG.ShowDialog();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Window4 nAbteilung = new Window4();
            nAbteilung.ShowDialog();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Window5 nPersonal = new Window5();
            nPersonal.ShowDialog();
            try
            {
                bk.Connection();
                try { pListView_Load(); bk.CloseCon(); }
                catch { MessageBox.Show("Es ist ein Fehler aufgetreten.", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); }
            }
            catch { MessageBox.Show("Die Verbindung konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Window6 nUGruppe = new Window6();
            nUGruppe.ShowDialog();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            Window7 nUStunden = new Window7();
            nUStunden.Show();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            Window8 nBonus = new Window8();
            nBonus.ShowDialog();
        }

        private void list3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (list3.SelectedItem != null)
            {
                string _tmp = "";
                foreach (Personal item in list3.SelectedItems) { _tmp = item.pNr.ToString(); }
                Window10 bPersonal = new Window10(int.Parse(_tmp));
                bPersonal.ShowDialog();
                try
                {
                    bk.Connection();
                    try
                    {
                        pListView_Load();
                        uListView_Load();
                        laListView_Load();
                        bk.CloseCon();
                    }
                    catch { MessageBox.Show("Die ListView konnte nicht geladen werden", ""); bk.CloseCon(); }
                }
                catch { MessageBox.Show("Die Verbindung konnte nicht hergestellt werden", ""); bk.CloseCon(); }
            }
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (list.SelectedItem != null)
            {
                string _tmp = ""; string _tmp2 = "";
                foreach (Lohnabrechnung item in list.SelectedItems) { _tmp = item.laNr.ToString(); _tmp2 = item.laDatum.ToString(); }
                Window1 laAnsehen = new Window1(int.Parse(_tmp), _tmp2);
                laAnsehen.ShowDialog();
                list.SelectedItem = null;
            }
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            Window11 settings = new Window11();
            settings.ShowDialog();
            try
            {
                bk.Connection();
                try {pListView_Load(); bk.CloseCon(); }
                catch(Exception a) { bk.CloseCon(); throw a; }
            }
            catch(Exception a) { bk.CloseCon(); throw a; }
        }


        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            if (e.Cancel == true)
            {
                Properties.Settings.Default.Save();
                Console.WriteLine("Config gespeichert");
                base.OnClosing(e);
            }
        }
    }
}
