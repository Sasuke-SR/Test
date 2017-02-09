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
                int _tmp = 0;
                string _tmpFehler = "";

                tbLgBet.Text = tbLgBet.Text.Replace(",", ".");
                if (String.IsNullOrWhiteSpace(tbLgName.Text) || String.IsNullOrWhiteSpace(tbLgBet.Text))
                {
                    _tmpFehler += "Die Felder dürfen nicht leer sein.\n";
                }
                else { _tmp = 1; }

                if (bk.IsNumeric(tbLgBet.Text) == false)
                { _tmpFehler += "Im Feld Lohn dürfen nur Numerische Zahlen stehen"; _tmp = 0; }
                else
                    _tmp = 1;

                bk.Connection();
                try
                {
                    if (_tmp == 1)
                    {
                        #region Lohngruppe erstellen
                        string query = string.Format("INSERT INTO Lohngruppen (L_Bez,L_Lohn) VALUES ('{0}',{1});", tbLgName.Text, tbLgBet.Text);// = $"INSERT INTO Abteilung (Abt_Bez) Values ({textBox_Name.Text});"
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
                    else
                    {
                        MessageBox.Show(_tmpFehler, "", MessageBoxButton.OK, MessageBoxImage.Error);
                        bk.CloseCon();
                    }

                }
                catch { MessageBox.Show("Fehler", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); }
            }
            catch { MessageBox.Show("Verbindung war nicht erfolgreich", "", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
    }
}
