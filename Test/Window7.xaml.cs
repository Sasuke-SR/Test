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

namespace Test
{
    /// <summary>
    /// Interaktionslogik für Window7.xaml
    /// </summary>
    public partial class Window7 : Window
    {
        Basisklasse bk = new Basisklasse();
        OleDbDataReader dr;
        private class Person
        {
            public int pNr { get; set; }
            public string vName { get; set; }
            public string nName { get; set; }
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
                    dr = bk.Select("SELECT Last(US_Nr) FROM UStunden;");
                    dr.Read();
                    try
                    {
                        lUeStdNr.Content = dr.GetInt32(0) + 1;
                    }
                    catch
                    {
                        lUeStdNr.Content = 1;
                    }
                    bk.CloseCon();
                }
                catch (Exception ex1)
                { MessageBox.Show("Fehler beim bestimmen der Überstundengruppen-Nummer", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); Console.WriteLine(ex1); return; }

                try
                {
                    bk.Connection();
                    dr = bk.Select("SELECT US_Bez FROM UStunden;");
                    while(dr.Read())
                    {
                        cbUeStdGr.Items.Add(dr.GetString(1));
                    }
                    cbUeStdGr.Items.Refresh();
                    bk.CloseCon();

                }
                catch (Exception ex2)
                { MessageBox.Show("Fehler beim bestimmen der Überstundengruppen", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); Console.WriteLine(ex2); return; }

                try
                {
                    List<Person> zBobs; // Hier wird gerade dran gearbeitet
                    bk.Connection();
                    dr = bk.Select("SELECT P_Nr, P_VName, P_NName FROM Personal;");
                    while(dr.Read())
                    {
                        cbPer.Items.Add(dr.GetString(0));
                    }
                }
                catch
                {

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Die Verbindung zur Datenbank konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error); Console.WriteLine(ex);
            }
        }
    }
}
