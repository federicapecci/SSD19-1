using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Data.Common;

namespace DSS19
{
    class Persistence //model
    {
        public string connectionString;
        private IDbConnection conn = null;
        public string factory = "";

        public void readDb()
        {
            Trace.WriteLine("Persistance write DB");
            queryAllOrder();
        }

        public void readDb(string custID)
        {
            Trace.WriteLine("Persistance write DB with custID");
            queryCustomerID(custID);
        }

        private IDbConnection openConnection()
        {
            // string dbpath = @"C:\Users\Enrico\Desktop\ordiniMI2018.sqlite"; //connessione al DB sqlite
            // string sqlLiteConnString = @"Data Source=" + dbpath + "; Version=3";
            //conn = new SQLiteConnection(connectionString);
            //conn = new SqlConnection(connectionString);

            DbProviderFactory dbFactory = DbProviderFactories.GetFactory(factory);
            conn = dbFactory.CreateConnection();
            try
            {
                conn.ConnectionString = connectionString;
                Trace.WriteLine("[PERSISTANCE] Connessione DB aperta");
                conn.Open();
                return conn;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[PERSISTANCE] errore: " + ex.Message);
            }
            return null;

            
        }

        private IDataReader executeQuery(string queryText)
        {
            conn = openConnection();
            IDbCommand com = conn.CreateCommand();
            try
            {
                com.CommandText = queryText;
                IDataReader reader = com.ExecuteReader();
                Trace.WriteLine("[PERSISTANCE] query done ");
                return reader;
            }
            catch (Exception ex)
            {
                errorLog("executeQuery " + ex.Message);
            }
            return null;
        }

        private void queryAllOrder() 
        {
            try 
            {
                string queryText = "select TOP (100) id, customer, time, quant from ordini ";
                 queryText = "select id, customer, time, quant from ordini LIMIT 100 "; //sqlLite
                IDataReader reader = executeQuery(queryText);
                while (reader.Read())
                {
                    Trace.WriteLine(reader["id"] + " " + reader["customer"] + " " + reader["time"] + " " + reader["quant"]); //view.textConsole = ...
                }
                reader.Close(); 
            }
            catch(Exception ex)
            {
                errorLog("allOrder " + ex.Message);
            }
            Trace.WriteLine("[PERSISTANCE] fine lettura dati ");
            conn.Close();

        }

        private void queryCustomerID(string custID) 
        {
            List<int> quantLst = new List<int>();
            try
            {
                string queryText = "SELECT id, customer, time, quant from ordini where customer = \'"+custID+"\'";
                IDataReader reader = executeQuery(queryText);
                while (reader.Read())
                {
                    Trace.WriteLine(reader["id"] + " " + reader["customer"] + " " + reader["time"] + " " + reader["quant"]); //view.textConsole = ...
                    quantLst.Add(Convert.ToInt32(reader["quant"]));

                }
                reader.Close();
            }
            catch (Exception ex)
            {
                errorLog("customerID " + ex.Message);
            }
            Trace.WriteLine("Quantità: " + string.Join(",", quantLst));
            Trace.WriteLine("[PERSISTANCE] fine lettura dati ");
        }

        public void insert(string cust)
        {
            try
            {
                string queryText = @"INSERT INTO ordini (customer) VALUES ('"+cust+"') ";
                IDataReader reader = executeQuery(queryText);
                queryCustomerID(cust); //to verify the insert
                reader.Close();
            }
            catch (Exception ex)
            {
                errorLog("insert " + ex.Message);
            }
            Trace.WriteLine("[PERSISTANCE] insert done");
            conn.Close();
        }
        
        public void delete(string cust )
        {
            try
            {
                string queryText = @"DELETE FROM ordini WHERE customer = '"+cust+"'";
                IDataReader reader = executeQuery(queryText);
                queryCustomerID(cust); //to verify the delete
                reader.Close();
            }
            catch (Exception ex)
            {
                errorLog("delete " + ex.Message);
            }
            
            Trace.WriteLine("[PERSISTANCE] delete done");
            conn.Close();
        }
        
        public void update(string oldCust, string newCust)
        {
            try
            {
                string queryText = @"UPDATE ordini SET customer = '"+newCust+"'  WHERE customer = '" + oldCust + "'";
                IDataReader reader = executeQuery(queryText);
                queryCustomerID(newCust); //to verify the insert
                reader.Close();
            }
            catch (Exception ex)
            {
                errorLog("update " + ex.Message);
            }
            Trace.WriteLine("[PERSISTANCE] update done");
            conn.Close();
        }

        private void errorLog(string errTxt)
        {
            Trace.WriteLine("[PERSISTANCE] errore: " + errTxt);
        }
    }


}
