using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;

namespace Test
{
    class Basisklasse
    {
        OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0; Data Source=Datenbank.accdb");
        OleDbCommand cmd;
        OleDbDataReader dr;

        public void Connection() {try {con.Open(); } catch(Exception a) { throw a; }}

        public void CloseCon() { try { con.Close(); } catch (Exception a) { throw a; } }

        public OleDbDataReader Select(string query)
        {
            try
            {
                cmd = new OleDbCommand(query,con);
                dr = cmd.ExecuteReader();
            }
            catch (Exception a) { throw a; }
            return dr;
        }

        public void Insert(string query)
        {
            try
            {
                cmd = new OleDbCommand(query, con);
                cmd.ExecuteNonQuery();
            }
            catch(Exception a) { throw a; }
        }

    }
}
