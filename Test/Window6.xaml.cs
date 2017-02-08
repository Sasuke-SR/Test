﻿using System;
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

        public class UStundenGr
        {
            public int Nr { get; set; }
            public string Bezeichnung { get; set; }
            public double Betrag { get; set; }
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
                    figureOutNr();
                    bk.CloseCon();
                }
                catch
                { MessageBox.Show("Fehler beim bestimmen der Überstundengruppennummer", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); return; }

                try
                {
                    bk.Connection();
                    try
                    {
                        fillLv();
                    }
                    catch { MessageBox.Show("Fehler beim bestimmen existierender Überstundengruppen", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); return; }
                    bk.CloseCon();
                }
                catch { MessageBox.Show("Die Verbindung zur Datenbank konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
            catch { MessageBox.Show("Die Verbindung zur Datenbank konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void bMainWin_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void bGrErs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bk.Connection();
                try
                {
                    bk.Insert($"INSERT INTO UStunden (US_Bez, US_Betrag) VALUES ('{tbUeBez.Text}', {tbUeBet.Text.Replace(',', '.')});");
                    try
                    {
                        lvUeGr.ItemsSource = null;
                        fillLv();
                        tbUeBet.Text = "";
                        tbUeBez.Text = "";
                        figureOutNr();
                    }
                    catch (Exception ex) { throw ex; }
                }
                catch { MessageBox.Show("Fehler beim Einfügen in die Datenbank", "", MessageBoxButton.OK, MessageBoxImage.Error); bk.CloseCon(); return; }
            }
            catch { MessageBox.Show("Die Verbindung zur Datenbank konnte nicht hergestellt werden.", "", MessageBoxButton.OK, MessageBoxImage.Error); }

        }

        private void fillLv()
        {
            dr = bk.Select("SELECT * FROM UStunden;");
            List<UStundenGr> usgListe = new List<UStundenGr>();
            while (dr.Read())
            {
                UStundenGr UsGr = new UStundenGr() { Nr = dr.GetInt32(0), Bezeichnung = dr.GetString(1), Betrag = dr.GetDouble(2) };
                usgListe.Add(UsGr);
            }
            lvUeGr.ItemsSource = usgListe;
            lvUeGr.Items.Refresh();
        }

        private void figureOutNr()
        {
            dr = bk.Select("SELECT Last(US_Nr) FROM UStunden;");
            dr.Read();
            try
            {
                lUeGrNr.Content = dr.GetInt32(0) + 1;
            }
            catch
            {
                lUeGrNr.Content = 1;
            }
        }
    }
}