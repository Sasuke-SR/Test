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
    /// Interaktionslogik für Window4.xaml
    /// </summary>
    public partial class Window4 : MetroWindow
    {
        Basisklasse bk = new Basisklasse();
        OleDbDataReader dr;
        public Window4()
        {
            InitializeComponent();
        }


        private void listView_Load()
        {
            // Abteilungen
            dr = bk.Select("SELECT * FROM ABTEILUNG");
            List<Abteil> items = new List<Abteil>();
            while (dr.Read())
            {
                items.Add(new Abteil() { nr = dr.GetInt32(0), bez = dr.GetString(1) });
            }
            list.ItemsSource = items;
        }

        public class Abteil
        {
            public int nr { get; set; }
            public string bez { get; set; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(textBox_Name.Text))
                {
                    try
                    {
                        bk.Connection();
                        try
                        {
                            if (bk.IsAllowed(textBox_Name.Text.Trim(), true, true, true, "-.,"))
                            {
                                bk.Insert($"INSERT INTO Abteilung (Abt_Bez) VALUES ('{textBox_Name.Text.Trim()}');");
                                this.ShowMessageAsync("", "Die Abteilung wurde erstellt.");
                                bk.CloseCon();

                                #region Form neuladen
                                textBox_Name.Text = "";
                                try
                                {
                                    bk.Connection();
                                    try
                                    {
                                        dr = bk.Select("SELECT last(Abt_Nr) FROM Abteilung");
                                        dr.Read();
                                        try
                                        {
                                            int _tmpA = dr.GetInt32(0) + 1;
                                            Abteilung_Nr.Content = _tmpA.ToString();
                                            listView_Load();
                                            bk.CloseCon();
                                        }
                                        catch
                                        {
                                            Abteilung_Nr.Content = "1";
                                            listView_Load();
                                            bk.CloseCon();
                                        }
                                    }
                                    catch { this.ShowMessageAsync("Fehler", "Es ist ein Fehler aufgetreten."); bk.CloseCon(); }/*MessageBox.Show("Fehler", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon();*/
                                }
                                catch { this.ShowMessageAsync("Fehler", "Die Verbindung zur Datenbank konnte nicht hergestellt werden."); bk.CloseCon(); }/*MessageBox.Show("Die Verbindung konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon();*/
                                #endregion
                            }
                            else { this.ShowMessageAsync("Fehler", "Es dürfen keine Sonderzeichen eingegeben werden."); bk.CloseCon(); }/*MessageBox.Show("Es dürfen keine Sonderzeichen eingegeben werden.", "", MessageBoxButton.OK, MessageBoxImage.Error);*/
                        }
                        catch { this.ShowMessageAsync("Fehler", "Es ist ein Fehler aufgetreten."); bk.CloseCon(); }/*MessageBox.Show("Fehler", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon();*/
                    }
                    catch { this.ShowMessageAsync("Fehler", "Die Verbindung zur Datenbank konnte nicht hergestellt werden."); } 
                }
                else { this.ShowMessageAsync("Fehler", "Es muss ein Name angegeben werden."); }
            }
            catch { MessageBox.Show("Verbindung war nicht erfolgreich", "", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                bk.Connection();
                try
                {
                    dr = bk.Select("SELECT last(Abt_Nr) FROM Abteilung");
                    dr.Read();
                    try
                    {
                        int _tmp = dr.GetInt32(0) + 1;
                        Abteilung_Nr.Content = _tmp.ToString();
                        listView_Load();
                        bk.CloseCon();
                    }
                    catch
                    {
                        int _tmp = 1;
                        Abteilung_Nr.Content = _tmp.ToString();
                        listView_Load();
                        bk.CloseCon();
                    }
                }
                catch { this.ShowMessageAsync("Fehler", "Es ist ein fehler aufgetreten."); bk.CloseCon(); }/*MessageBox.Show("Fehler","",MessageBoxButton.OK,MessageBoxImage.Error); bk.CloseCon();*/
            }
            catch { this.ShowMessageAsync("Fehler", "Die Verbindung konnte nicht hergestellt werden."); bk.CloseCon(); this.Close(); }/*MessageBox.Show("Die Verbindung konnte nicht hergestellt werden.","",MessageBoxButton.OK,MessageBoxImage.Error); bk.CloseCon(); this.Close();*/
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}