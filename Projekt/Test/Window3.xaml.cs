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

namespace Test
{
    /// <summary>
    /// Interaktionslogik für Window3.xaml
    /// </summary>
    public partial class Window3 : Window
    {
        OleDbDataReader dr;
        Basisklasse bk = new Basisklasse();
        public Window3()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void listView_Load()
        {
            dr = bk.Select("SELECT * FROM Lohngruppen");
            List<Lohngruppe> items = new List<Lohngruppe>();
            while (dr.Read())
            {
                string _tmp = String.Format("{0}" + " €", dr.GetDouble(2));
                items.Add(new Lohngruppe() { nr = dr.GetInt32(0), bez = dr.GetString(1), betrag = _tmp });
            }
            lvLg.ItemsSource = items;
        }

        public class Lohngruppe
        {
            public int nr { get; set; }
            public string bez { get; set; }
            public string betrag { get; set; }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                bk.Connection();
                try
                {
                    dr = bk.Select("SELECT last(L_Nr) FROM Lohngruppen");
                    dr.Read();
                    try
                    {
                        int _tmp = dr.GetInt32(0); _tmp += 1;
                        Nr_Lohngruppe.Content = _tmp.ToString();
                        listView_Load();
                    }
                    catch
                    {
                        int _tmp = 1;
                        Nr_Lohngruppe.Content = _tmp.ToString();
                        listView_Load();
                    }
                    bk.CloseCon();
                }
                catch { MessageBox.Show("Fehler", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); }
            }
            catch { MessageBox.Show("Die Verbindung konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void bLgErs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bk.Connection();
                try
                {
                    if (!String.IsNullOrWhiteSpace(tbLgBet.Text) && !String.IsNullOrWhiteSpace(tbLgName.Text))
                    {
                        if (bk.IsAllowed(tbLgBet.Text, false, true, false, "€.,") && bk.IsAllowed(tbLgName.Text, true, true, true, ".-,"))
                        {
                            #region Lohngruppe erstellen
                            string _tmpA = tbLgBet.Text.Replace(",", ".").Replace("€", "").Trim();
                            string query = string.Format("INSERT INTO Lohngruppen (L_Bez,L_Lohn) VALUES ('{0}',{1});", tbLgName.Text, _tmpA);// = $"INSERT INTO Abteilung (Abt_Bez) Values ({textBox_Name.Text});"
                            bk.Insert(query);
                            MessageBox.Show("Die Lohngruppe wurde erstellt.", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            bk.CloseCon();
                            //Neu Laden der Forms
                            tbLgName.Text = "";
                            tbLgBet.Text = "";
                            #endregion
                            #region Form neuladen
                            try
                            {
                                bk.Connection();
                                try
                                {
                                    dr = bk.Select("SELECT last(L_Nr) FROM Lohngruppen");
                                    dr.Read();
                                    Nr_Lohngruppe.Content = dr.GetInt32(0).ToString();
                                    listView_Load();
                                    bk.CloseCon();
                                }
                                catch { MessageBox.Show("Fehler", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); }
                            }
                            catch { MessageBox.Show("Die Verbindung konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error); }
                            #endregion
                        }
                        else MessageBox.Show("Es sind keine Leerzeichen so wie Sonderzeichen erlaubt."); bk.CloseCon();
                    }
                    else MessageBox.Show("Die Felder dürfen nicht Leer sein",""); bk.CloseCon();
                }
                catch { MessageBox.Show("Fehler", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); }
            }
            catch { MessageBox.Show("Verbindung war nicht erfolgreich", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); }
        }
    }
}
