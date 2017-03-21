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
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;

namespace Test
{
    /// <summary>
    /// Interaktionslogik für Window12.xaml
    /// </summary>
    public partial class Window12 : MetroWindow
    {
        new string Name; int ID;
        Basisklasse bk = new Basisklasse();

        public Window12(int nr, string bez)
        {
            Name = bez; ID = nr;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(tbAb.Text))
            {
                if (bk.IsAllowed(tbAb.Text, true, true, false, "-") != false)
                {
                    try
                    {
                        bk.Connection();
                        try
                        {
                            bk.Update($"UPDATE Abteilung SET Abt_Bez = '{tbAb.Text.Trim()}' WHERE Abt_Nr = {ID}");
                            //this.ShowMessageAsync("Erfolgreich","Die Abteilung wurde erfolgreich geändert.");
                            MessageBox.Show("Die Abteilung wurde erfolgreich gespeichert.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            bk.CloseCon();
                            this.Close();
                        }
                        catch(Exception a) { bk.CloseCon(); throw a; }
                    }
                    catch (Exception a) { throw a; }
                }
                else this.ShowMessageAsync("Fehler","Es dürfen keine Sonderzeichen sowie Leerzeichen enthalten sein.");
            }
            else this.ShowMessageAsync("Fehler","Das Feld darf nicht leer sein");
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            lbNr.Content = ID.ToString();
            tbAb.Text = Name;
        }
    }
}
