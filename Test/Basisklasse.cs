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

        public void Connection()
        {try {con.Open(); } catch(Exception a) { throw a; }}

        public void CloseCon()
        { try { con.Close(); } catch (Exception a) { throw a; } }

    }
}
