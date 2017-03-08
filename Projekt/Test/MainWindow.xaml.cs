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
            public int aNr {get;set;}
            public string aName { get; set; }
        }
        #endregion

        #region ListView's
        private void pListView_Load()
        {
            dr = bk.Select("SELECT * FROM Personal");
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

        private void pListView_Search(string query, int status)
        {
            try
            {
                if (status == 0)
                {
                    dr = bk.Select(query);
                    List<Personal> items = new List<Personal>();
                    try
                    {
                        while (dr.Read())
                        {
                            string _tmp = "";
                            dr1 = bk.Select($"SELECT * FROM Abteilung WHERE Abt_Nr = {dr.GetInt32(3)}");
                            dr1.Read();
                            if (dr1.GetBoolean(5) == true) { _tmp = "Gefeuert"; } else _tmp = "Angestellt";
                            items.Add(new Personal() { pNr = dr.GetInt32(0), pVName = dr.GetString(1), pNName = dr.GetString(2), pAbteilung = dr1.GetString(1), pStatus = _tmp });
                        }
                    }
                    catch (Exception a) { throw a; }
                    list3.ItemsSource = items;
                }
                else
                {
                    try
                    {
                        dr1 = bk.Select($"Select * FROM Abteilung WHERE Abt_Bez LIKE '%{tbSuche.Text}%'");
                        dr1.Read();
                        int aID = dr1.GetInt32(0);
                        dr = bk.Select($"SELECT * FROM Personal WHERE P_Abteilungs_Nr LIKE '%{aID}%'");
                        List<Personal> items = new List<Personal>();
                        try
                        {
                            while (dr.Read())
                            {
                                string _tmp = "";
                                dr1 = bk.Select($"SELECT * FROM Abteilung WHERE Abt_Nr = {dr.GetInt32(3)}");
                                dr1.Read();
                                if (dr1.GetBoolean(5) == true) { _tmp = "Gefeuert"; } else _tmp = "Angestellt";
                                items.Add(new Personal() { pNr = dr.GetInt32(0), pVName = dr.GetString(1), pNName = dr.GetString(2), pAbteilung = dr1.GetString(1), pStatus = _tmp });
                            }
                        }
                        catch (Exception a) { throw a; }
                        list3.ItemsSource = items;
                    }
                    catch(Exception a) { throw a; }
                }
            }
            catch(Exception a) { throw a; }
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
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                bk.Connection();
                try
                {
                    pListView_Load();
                    bk.CloseCon();
                }
                catch
                { MessageBox.Show("Die Listen konnten nicht geladen werden.","",MessageBoxButton.OK,MessageBoxImage.Error); bk.CloseCon(); }
            }
            catch { this.ShowMessageAsync("Titel", "Die Liste konnte nicht geldaden werden."); /*MessageBox.Show("Die Verbindung konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error);*/ }
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
                catch { MessageBox.Show("Es ist ein Fehler aufgetreten.","",MessageBoxButton.OK,MessageBoxImage.Error); bk.CloseCon(); }
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
        {
            string tabItem = ((sender as TabControl).SelectedItem as TabItem).Header as string;
            switch (tabItem)
            {
                case "Lohnabrechnungen":
                    cbSuche.Items.Clear();
                    cbSuche.Items.Insert(0, "Nummer");
                    cbSuche.Items.Insert(1, "Monat");
                    cbSuche.Items.Insert(2, "Lohngruppe");
                    cbSuche.Items.Insert(3, "Brutto-Summe");
                    cbSuche.Items.Insert(4, "Netto-Summe");
                    break;
                case "Überstunden":
                    break;
                case "Personal":
                    cbSuche.Items.Clear();
                    cbSuche.Items.Insert(0, "Nummer");
                    cbSuche.Items.Insert(1, "Nachname");
                    cbSuche.Items.Insert(2, "Vorname");
                    cbSuche.Items.Insert(3, "Abteilung");
                    //tbSuche.Text = "";
                    cbSuche.SelectedIndex = 0;
                    break;
            }
        }

        private void tbSuche_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                bk.Connection();
                try
                {
                    if (tbSuche.Text != "")
                    {
                        if (tcPersonal.IsSelected == true)
                        {
                            switch (cbSuche.SelectedIndex)
                            {
                                case 0:
                                    list3.ItemsSource = null;
                                    list3.Items.Clear();
                                    pListView_Search($"SELECT * FROM Personal WHERE P_Nr LIKE '%{tbSuche.Text}%'",0);
                                    bk.CloseCon();
                                    break;
                                case 1:
                                    list3.ItemsSource = null;
                                    list3.Items.Clear();
                                    pListView_Search($"SELECT * FROM Personal WHERE P_NName LIKE '%{tbSuche.Text}%'",0);
                                    bk.CloseCon();
                                    break;
                                case 2:
                                    list3.ItemsSource = null;
                                    list3.Items.Clear();
                                    pListView_Search($"SELECT * FROM Personal WHERE P_VName LIKE '%{tbSuche.Text}%'",0);
                                    bk.CloseCon();
                                    break;
                                case 3:
                                    list3.ItemsSource = null;
                                    list3.Items.Clear();
                                    pListView_Search("",1);
                                    bk.CloseCon();
                                    break;

                            }
                        }
                        else if (tcUStunden.IsSelected == true)
                        { }
                        else if (tcPersonal.IsSelected == true)
                        { }
                    }
                    else
                    { pListView_Load(); bk.CloseCon(); }
                }
                catch { bk.CloseCon(); }
            }
            catch { MessageBox.Show("Verbindung konnte nicht hergestellt werden.","",MessageBoxButton.OK,MessageBoxImage.Error); }
        }

        private void cbSuche_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tbSuche.IsReadOnly = false;
        }

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
                        bk.CloseCon();
                    }
                    catch { MessageBox.Show("Die ListView konnte nicht geladen werden", ""); bk.CloseCon(); }
                }
                catch { MessageBox.Show("Die Verbindung konnte nicht hergestellt werden", ""); }
            }
        }
    }
}
