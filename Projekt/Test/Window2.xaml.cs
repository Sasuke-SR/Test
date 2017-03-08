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
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    /// 
    public partial class Window2 : Window
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
    }
}
