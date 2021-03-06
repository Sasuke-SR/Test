﻿using System;
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
            if (Properties.Settings.Default.Setting == true) { dr = bk.Select("SELECT * FROM ABTEILUNG"); }
            else { dr = bk.Select("SELECT * FROM ABTEILUNG WHERE Abt_Deaktiviert = false"); }
            List<Abteil> items = new List<Abteil>();
            try
            {
                while (dr.Read())
                {
                    string _tmp = "";
                    if (dr.GetBoolean(2) == true) { _tmp = "Deaktiviert"; } else { _tmp = "Aktiviert"; }
                    items.Add(new Abteil() { nr = dr.GetInt32(0), bez = dr.GetString(1), deak = _tmp });
                }
            }
            catch (Exception a) { throw a; }
            list.ItemsSource = items;
        }

        public class Abteil
        {
            public int nr { get; set; }
            public string bez { get; set; }
            public string deak { get; set; }
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
                                this.ShowMessageAsync("Erfolgreich", "Die Abteilung wurde erstellt.");
                                bk.CloseCon();
                                int NextID = Convert.ToInt32(Abteilung_Nr.Content) + 1;
                                textBox_Name.Text = "";
                                Abteilung_Nr.Content = NextID.ToString();
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
                    dr = bk.Select($"SELECT last(abt_nr) FROM Abteilung");
                    dr.Read();
                    try { int s = dr.GetInt32(0) + 1; ; Abteilung_Nr.Content = s.ToString(); }
                    catch { Abteilung_Nr.Content = "1"; }
                    listView_Load();
                    bk.CloseCon();
                }
                catch { this.ShowMessageAsync("Fehler", "Es ist ein fehler aufgetreten."); bk.CloseCon(); }/*MessageBox.Show("Fehler","",MessageBoxButton.OK,MessageBoxImage.Error); bk.CloseCon();*/
            }
            catch { this.ShowMessageAsync("Fehler", "Die Verbindung konnte nicht hergestellt werden."); bk.CloseCon(); this.Close(); }/*MessageBox.Show("Die Verbindung konnte nicht hergestellt werden.","",MessageBoxButton.OK,MessageBoxImage.Error); bk.CloseCon(); this.Close();*/
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) => this.Close();

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (list.SelectedItem != null)
            {
                int abtNr = 0; string _tmp = "";
                foreach (Abteil item in list.SelectedItems)
                { abtNr = item.nr; _tmp = item.deak.ToString().Trim(); }
                if (_tmp != "Deaktiviert")
                {
                    try
                    {
                        bk.Connection();
                        try
                        {
                            bk.Update($"UPDATE Abteilung SET Abt_Deaktiviert = true WHERE Abt_Nr = {abtNr}");
                            this.ShowMessageAsync("Erfolgreich", "Die Abteilung wurde erfolgreich deaktiviert");
                            listView_Load();
                            bk.CloseCon();
                        }
                        catch (Exception a) { bk.CloseCon(); throw a; }
                    }
                    catch (Exception a) { bk.CloseCon(); throw a; }
                }
                else this.ShowMessageAsync("Fehler","Diese Abteilung ist schon bereits Deaktiviert");
            }
            else this.ShowMessageAsync("Fehler","Sie haben keine Abteilung ausgewählt");
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (list.SelectedItem != null)
            {
                int abtNr = 0; string _tmp = "";
                foreach (Abteil item in list.SelectedItems)
                { abtNr = item.nr; _tmp = item.deak.ToString().Trim(); }
                if (_tmp != "Aktiviert")
                {
                    try
                    {
                        bk.Connection();
                        try
                        {
                            bk.Update($"UPDATE Abteilung SET Abt_Deaktiviert = false WHERE Abt_Nr = {abtNr}");
                            this.ShowMessageAsync("Erfolgreich", "Die Abteilung wurde erfolgreich Aktiviert");
                            listView_Load();
                            bk.CloseCon();
                        }
                        catch (Exception a) { bk.CloseCon(); throw a; }
                    }
                    catch (Exception a) { bk.CloseCon(); throw a; }
                }
                else this.ShowMessageAsync("Fehler","Diese Abteilung ist schon bereits Aktiv");
            }
            else this.ShowMessageAsync("Fehler", "Sie haben keine Abteilung ausgewählt");
        }

        private void list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (list.SelectedItem != null)
            {
                int nr = 0; string bez = "";
                foreach (Abteil item in list.SelectedItems) { nr = item.nr; bez = item.bez; }
                Window12 usr = new Window12(nr, bez);
                usr.ShowDialog();
                System.Threading.Thread.Sleep(500);
                try
                {
                    bk.Connection();
                    try
                    {
                        listView_Load();
                        bk.CloseCon();
                    }
                    catch (Exception a) { throw a; }
                }
                catch (Exception a) { throw a; }
            }
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}