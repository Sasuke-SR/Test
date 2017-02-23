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
    /// Interaktionslogik für Window4.xaml
    /// </summary>
    public partial class Window4 : Window
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
                int _tmp = 0;
                string _tmpFehler = "";
                if (String.IsNullOrWhiteSpace(textBox_Name.Text))
                {
                    _tmpFehler += "Die Felder dürfen nicht leer sein.";
                }
                else
                    _tmp = 1;

                bk.Connection();
                try
                {
                    if (_tmp == 1)
                    {
                        if (bk.IsAllowed(textBox_Name.Text, true, true, true, "-.,"))
                        {
                            string query = string.Format("INSERT INTO Abteilung (Abt_Bez) VALUES ('{0}');", textBox_Name.Text);// = $"INSERT INTO Abteilung (Abt_Bez) Values ({textBox_Name.Text});"
                            bk.Insert(query);
                            MessageBox.Show("Die Abteilung wurde erstellt.", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            bk.CloseCon();
                            //Neu Laden der Forms
                            textBox_Name.Text = "";
                            #region Form neuladen
                            try
                            {
                                bk.Connection();
                                try
                                {
                                    dr = bk.Select("SELECT last(Abt_Nr) FROM Abteilung");
                                    dr.Read();
                                    try
                                    {
                                        int _tmpA = dr.GetInt32(0); _tmp += 1;
                                        Abteilung_Nr.Content = _tmp.ToString();
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
                                catch { MessageBox.Show("Fehler", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); }
                            }
                            catch { MessageBox.Show("Die Verbindung konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); }
                            #endregion
                        }
                        else MessageBox.Show("Es dürfen keine Sonderzeichen eingegeben werden.","",MessageBoxButton.OK,MessageBoxImage.Error);
                    }
                    else
                        MessageBox.Show(_tmpFehler, "", MessageBoxButton.OK, MessageBoxImage.Error);
                    bk.CloseCon();
                }
                catch { MessageBox.Show("Fehler", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); }
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
                            int _tmp = dr.GetInt32(0); _tmp += 1;
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
                catch { MessageBox.Show("Fehler","",MessageBoxButton.OK,MessageBoxImage.Error); bk.CloseCon(); }
            }
            catch { MessageBox.Show("Die Verbindung konnte nicht hergestellt werden.","",MessageBoxButton.OK,MessageBoxImage.Error); bk.CloseCon(); this.Close(); }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
