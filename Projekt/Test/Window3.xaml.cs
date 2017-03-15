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
    /// Interaktionslogik für Window3.xaml
    /// </summary>
    public partial class Window3 : MetroWindow
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
                string _tmp = string.Format("{0}" + " €", dr.GetDouble(2));
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
                catch { this.ShowMessageAsync("Fehler", "Beim bestimmen der Lohngruppennummer ist ein Fehler aufgetreten."); bk.CloseCon(); }/*MessageBox.Show("Fehler", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon();*/
            }
            catch { this.ShowMessageAsync("Fehler", "Die Verbindung konnte nicht hergestellt werden."); }/*MessageBox.Show("Die Verbindung konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error);*/
        }

        private void bLgErs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bk.Connection();
                try
                {
                    if (!string.IsNullOrWhiteSpace(tbLgBet.Text) && !string.IsNullOrWhiteSpace(tbLgName.Text))
                    {
                        if (bk.IsAllowed(tbLgBet.Text.Trim(), false, true, false, "€.,"))
                        {
                            if (bk.IsAllowed(tbLgName.Text, true, true, true, ".-,"))
                            {
                                #region Lohngruppe erstellen
                                string _tmpA = tbLgBet.Text.Replace(",", ".").Replace("€", "").Trim();
                                string query = $"INSERT INTO Lohngruppen (L_Bez,L_Lohn) VALUES ('{tbLgName.Text.Trim()}',{_tmpA});";
                                bk.Insert(query);
                                this.ShowMessageAsync("", "Die Lohngruppe wurde erstellt.");
                                bk.CloseCon();
                                #endregion
                                #region Form neuladen
                                tbLgName.Text = "";
                                tbLgBet.Text = "";
                                try
                                {
                                    bk.Connection();
                                    try
                                    {
                                        dr = bk.Select("SELECT last(L_Nr) FROM Lohngruppen");
                                        dr.Read();
                                        Nr_Lohngruppe.Content = (dr.GetInt32(0) + 1).ToString();//Hab das +1 hizugefügt. Ich hoffe, dass das sinnvoll ist
                                        listView_Load();
                                        bk.CloseCon();
                                    }
                                    catch { this.ShowMessageAsync("Fehler", "Fehler beim bestimmen der Lohngruppennummer."); bk.CloseCon(); }/*MessageBox.Show("Fehler", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon();*/
                                }
                                catch { this.ShowMessageAsync("Fehler", "Die Verbindung zur Datenbank konnte nicht hergestellt werden."); } /*MessageBox.Show("Die Verbindung konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error);*/
                                #endregion
                            }
                            else { this.ShowMessageAsync("Fehler", "Der Lohngruppenname darf keine Sonderzeichen enthalten."); bk.CloseCon(); }
                        }
                        else { this.ShowMessageAsync("Fehler", "Der Stundensatz darf keine Buchstaben, Leerzeichen oder Sonderzeichen enthalten."); bk.CloseCon(); }/*MessageBox.Show("Es sind keine Leerzeichen so wie Sonderzeichen erlaubt."); bk.CloseCon();*/
                    }
                    else { this.ShowMessageAsync("Fehler", "Die Felder dürfen nicht Leer sein"); bk.CloseCon(); }/*MessageBox.Show("Die Felder dürfen nicht Leer sein",""); bk.CloseCon();*/
                }
                catch { this.ShowMessageAsync("Fehler", "");bk.CloseCon(); }/*MessageBox.Show("Fehler", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon();*/
            }
            catch { this.ShowMessageAsync("Fehler", "Die Verbindung zur Datenbank konnte nicht hergestellt werden."); }/*MessageBox.Show("Verbindung war nicht erfolgreich", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon();*/
        }
    }
}
