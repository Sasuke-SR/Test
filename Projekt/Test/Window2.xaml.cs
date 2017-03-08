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

namespace Test
{
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    /// 
    public partial class Window2 : MetroWindow
    {
        Basisklasse bk = new Basisklasse();
        OleDbDataReader dr;

        #region Load_Methoden
        public void Load_ComboBox()
        {
            // Personal wird geladen
            dr = bk.Select("SELECT * FROM Personal");
            string _tmp = "";
            while (dr.Read())
            {
                if (dr.GetInt32(0) < 10){_tmp = "00" + dr.GetInt32(0).ToString();}
                else if (dr.GetInt32(0) > 9 && dr.GetInt32(0) < 100) {_tmp = "0" + dr.GetInt32(0).ToString();}
                else {_tmp = dr.GetInt32(0).ToString();}
                cbPnr.Items.Add($"{_tmp}" + " - " + $"{dr.GetString(1)}" + " " + $"{dr.GetString(2)}");
            }
            // Lohngruppen werden geladen
            dr = bk.Select("SELECT * FROM Lohngruppen");
            while (dr.Read())
            {
                if (dr.GetInt32(0) < 10) { _tmp = "0" + dr.GetInt32(0).ToString(); }
                else { _tmp = dr.GetInt32(0).ToString(); }
                cbLgNr.Items.Add($"{_tmp}" + " - " + $"{dr.GetString(1)}");
            }
        }
        #endregion


        public Window2()
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
                    Load_ComboBox();
                    bk.CloseCon();
                }
                catch (Exception a) { bk.CloseCon(); throw a; }
            }
            catch { MessageBox.Show("Es konnte keine Verbindung hergestellt werden.", ""); }
        }

        private void bHaupt_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void bHaupt_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cbPnr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPnr.SelectedItem != null)
            {
                string _tmp = cbPnr.SelectedItem.ToString();
                string pNr = _tmp.Substring(0, _tmp.IndexOf('-')).Trim();
                lbPNr.Content = pNr;
                try
                {
                    bk.Connection();
                    try
                    {
                        dr = bk.Select($"SELECT * FROM Personal WHERE P_Nr = {pNr}");
                        dr.Read();
                        tbName.Text = dr.GetString(1); tbNname.Text = dr.GetString(2); tbAbtNr.Text = dr.GetInt32(3).ToString();
                        dr = bk.Select($"SELECT * From Abteilung WHERE Abt_Nr = {dr.GetInt32(3)}");
                        dr.Read();
                        tbAbrName.Text = dr.GetString(1);
                        bk.CloseCon();
                    }
                    catch(Exception a) { bk.CloseCon(); throw a; }
                }
                catch(Exception a) { bk.CloseCon(); throw a; }
            }
        }

        private void cbLgNr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbLgNr.SelectedItem != null)
            {
                string _tmp = cbLgNr.SelectedItem.ToString();
                string lgNr = _tmp.Substring(0, _tmp.IndexOf('-')).Trim();
                try
                {
                    bk.Connection();
                    try
                    {
                        dr = bk.Select($"SELECT * FROM Lohngruppen WHERE L_Nr = {lgNr}");
                        dr.Read();
                        tbLgName.Text = dr.GetString(1); tbLgBet.Text = dr.GetDouble(2).ToString() + " €";
                        bk.CloseCon();
                    }
                    catch (Exception a) { bk.CloseCon(); throw a; }
                }
                catch (Exception a) { bk.CloseCon(); throw a; }
            }
        }
    }
}
