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

        public void Update(string query)
        {
            try
            {
                cmd = new OleDbCommand(query, con);
                cmd.ExecuteNonQuery();
            }
            catch(Exception a) { throw a; }
        }

        //public bool IsNumeric(string x)
        //{
        //    double result;
        //    double.TryParse(x, out result);
        //    if (result == 0)
        //        return false; 
        //    else
        //        return true;
        //}

        #region IsAllowed

        public bool IsAllowed(string stringToCheck, bool allowLetters)
        {
            return IsAllowed(stringToCheck, allowLetters, false);
        }

        public bool IsAllowed(string stringToCheck, bool allowLetters, bool allowDigits)
        {
            return IsAllowed(stringToCheck, allowLetters, allowDigits, false);
        }

        public bool IsAllowed(string stringToCheck, bool allowLetters, bool allowDigits, bool allowSpace)
        {
            return IsAllowed(stringToCheck, allowLetters, allowDigits, allowSpace, ""); //geht zum dem, mit der string überladung und nicht der unter diesem
        }

        public bool IsAllowed(string stringToCheck, bool allowLetters, bool allowDigits, bool allowSpace, char allowThis)
        {
            return IsAllowed(stringToCheck, allowLetters, allowDigits, allowSpace, allowThis.ToString());
        }

        public bool IsAllowed(string stringToCheck, bool allowLetters, bool allowDigits, bool allowSpace, string allowThese)
        {
            foreach (char c in stringToCheck)
            {
                if ((allowLetters && char.IsLetter(c)) || (allowDigits && char.IsDigit(c)) || (allowSpace && c == ' '))
                {
                    //das Symbol ist in Ordnung
                }
                else
                {
                    foreach (char c2 in allowThese)
                    {
                        if (c == c2)
                        { return true; }
                    }
                    return false;
                }
            }
            return true;
        }
        #endregion IsAllowed

        #region FormateNumber
        public string FormateNumber(string inputNumber, string outputText, int toBeReplacedff)
        {
            char[] _tmpca = outputText.ToCharArray();

            if (inputNumber.Length == 3)
            {
                return inputNumber;
            }
            else if (inputNumber.Length == 2)
            {
                _tmpca[toBeReplacedff] = '0';
                _tmpca[toBeReplacedff + 1] = inputNumber[0];
                _tmpca[toBeReplacedff + 2] = inputNumber[1];
            }
            else //inputText.Length == 1
            {
                _tmpca[toBeReplacedff] = '0';
                _tmpca[toBeReplacedff + 1] = '0';
                _tmpca[toBeReplacedff + 2] = inputNumber[0];
            }
            return new string(_tmpca);
        }

        //public string FormateNumber(string[] inputNumbers, string outputText, string[] toBeReplaced) //inputNrs.Length == toBeReplaced.Length|= true
        //{
        //    for (int count = 0; count >= inputNumbers.Length; count++)
        //    {
        //        if (inputNumbers[count].Length == 3)
        //        { }
        //        else if (inputNumbers[count].Length == 2)
        //        { outputText = outputText.Replace(toBeReplaced[count], $"0{inputNumbers[count]}"); }
        //        else //inputText.Length == 1
        //        { outputText = outputText.Replace(toBeReplaced[count], $"00{inputNumbers[count]}"); }
        //    }
        //    return outputText;
        //}
        #endregion formateNumber

        public void TheInevitableMethodOfIndefiniteRecursion()
        {
            TheInevitableMethodOfIndefiniteRecursion();
        }



    }
}
