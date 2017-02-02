using System;
using System.Collections.Generic;
using System.Data.OleDb;
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

namespace Test
{
    /// <summary>
    /// Interaktionslogik für Window6.xaml
    /// </summary>
    public partial class Window6 : Window
    {
        Basisklasse bk = new Basisklasse();
        OleDbDataReader dr;

        public class Lohngruppe
        {
            public int Nr { get; set; }
            public string Bezeichnung { get; set; }
            public double Lohn { get; set; }
        }

        public Window6()
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
                    dr = bk.Select("SELECT Last(L_Nr) FROM Lohngruppen;");
                    dr.Read();
                    try
                    {
                        lUeGrNr.Content = dr.GetInt32(0) + 1;
                    }
                    catch
                    {
                        lUeGrNr.Content = 1;
                    }
                    bk.CloseCon();
                }
                catch
                { MessageBox.Show("Fehler beim bestimmen der Lohngruppennummer", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); return; }

                try
                {
                    bk.Connection();
                    dr = bk.Select("SELECT * FROM Lohngruppen;");
                    List<Lohngruppe> lgListe = new List<Lohngruppe>();
                    while (dr.Read())
                    {
                        Lohngruppe lg1 = new Lohngruppe() { Nr = dr.GetInt32(0), Bezeichnung = dr.GetString(1), Lohn = dr.GetDouble(2) };
                        lgListe.Add(lg1);
                    }
                    lvUeGr.ItemsSource = lgListe;
                }
                catch
                {

                }

            }
            catch
            {
                
            }
        }
    }
}