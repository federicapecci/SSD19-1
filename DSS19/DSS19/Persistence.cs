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
using PyGAP2019;

namespace DSS19
    {
        class Persistence //model
        {
            public string connectionString;
            private IDbConnection conn = null;
            public string factory = "";


            private IDbConnection openConnection()
            {
               //factory per creare connection giusta in base al db che abbiamo 

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

                    //per leggere soluzione della query
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

            public void selectFirstRecords() 
            {
                try
                {
                    string queryText = "";
                    if (factory == "System.Data.SQLite")
                    {
                        queryText = "select id, customer, time, quant from ordini LIMIT 100 ";
                    }
                    else
                    {
                        queryText = "select TOP (100) id, customer, time, quant from ordini ";

                    }

                    using (IDataReader reader = executeQuery(queryText))
                    {
                        while (reader.Read())
                        {
                            Trace.WriteLine(reader["id"] + " " + reader["customer"] + " " + reader["time"] + " " + reader["quant"]); //view.textConsole = ...
                        }
                    }

                }
                catch (Exception ex)
                {
                    errorLog("allOrder " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
                Trace.WriteLine("[PERSISTANCE] fine lettura dati ");
            }

            public void selectOrdersByCustId(string custID) 
            {
                List<int> quantLst = new List<int>();
                try
                {
                    string queryText = "SELECT id, customer, time, quant from ordini where customer = \'"+custID+"\'";
                    using (IDataReader reader = executeQuery(queryText))
                    {
                        while (reader.Read())
                        {
                            Trace.WriteLine(reader["id"] + " " + reader["customer"] + " " + reader["time"] + " " + reader["quant"]); //view.textConsole = ...
                            quantLst.Add(Convert.ToInt32(reader["quant"]));

                        }
                    }
               
                }
                catch (Exception ex)
                {
                    errorLog("customerID " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
                Trace.WriteLine("Quantità: " + string.Join(",", quantLst));
                Trace.WriteLine("[PERSISTANCE] fine lettura dati ");
            }

            public void insert(string cust)
            {
                try
                {
                    string queryText = @"INSERT INTO ordini (customer) VALUES ('" + cust + "') ";

                    using (IDataReader reader = executeQuery(queryText)) {

                        selectOrdersByCustId(cust); //to verify the insert
                    }
                }
                catch (Exception ex)
                {
                    errorLog("insert " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
                Trace.WriteLine("[PERSISTANCE] insert done");
             
            }
        
    
            public void delete(string cust )
            {
            try
            {
                string queryText = @"DELETE FROM ordini WHERE customer = '" + cust + "'";
                using (IDataReader reader = executeQuery(queryText))
                {
                    selectOrdersByCustId(cust); //to verify the delete
                }
            }
            catch (Exception ex)
            {
                errorLog("delete " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
            
                Trace.WriteLine("[PERSISTANCE] delete done");
                conn.Close();
            }
        
            public void update(string oldCust, string newCust)
            {
                try
                {
                    string queryText = @"UPDATE ordini SET customer = '"+newCust+"'  WHERE customer = '" + oldCust + "'";
                    using (IDataReader reader = executeQuery(queryText))
                    {
                        selectOrdersByCustId(newCust); //to verify the insert
                    }
                }
                catch (Exception ex)
                {
                    errorLog("update " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
                Trace.WriteLine("[PERSISTANCE] update done");
            
            }

            private void errorLog(string errTxt)
            {
                Trace.WriteLine("[PERSISTANCE] errore: " + errTxt);
            }

            public void selectCustomerListORM(string dbpath, int n) {

                List<string> custLst = new List<string>();
                try
                {
          
                    string queryText = "SELECT distinct customer from ordini ORDER BY RANDOM() LIMIT " + n;
                    using (IDataReader reader = executeQuery(queryText))
                    {
                        while (reader.Read())
                        {
                            custLst.Add(Convert.ToString(reader["customer"]));
                  
                    }
                    }

                }
                catch (Exception ex)
                {
                    errorLog("customerID " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
         
                Trace.WriteLine("Quantità: " + string.Join(",", custLst));
                Trace.WriteLine("[PERSISTANCE] fine lettura dati ");

        }


        public string selectQuantitiesListORM(string dbpath)
        {

            List<int> quantList;
            string ret = "Error reading DB";
            try
            {
                //var ctx = new SQLiteDatabaseContext(dbpath);
                using (var ctx = new SQLiteDatabaseContext(dbpath))
                {
                    quantList = ctx.Database.SqlQuery<int>("SELECT quant from ordini limit 100").ToList();
                }

                // legge solo alcuni clienti (si poteva fare tutto nella query)
                ret = string.Join(",", quantList);
                Trace.WriteLine(ret);
                // Aggiungere tutti gli altri casi. Update, delete, insert ..
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error: {ex.Message}");
            }

            return ret;
        }


        //insert ORM

        public void insertCustomertORM(string dbpath, string cust)
        {
            try
            {
                //var ctx = new SQLiteDatabaseContext(dbpath);
                using (var ctx = new SQLiteDatabaseContext(dbpath))
                {
                    ctx.Database.ExecuteSqlCommand("INSERT INTO ordini (customer) VALUES ('" + cust + "') ");
                }
            }
            catch (Exception ex)
            {
                errorLog("insert ORM" + ex.Message);
            }          
        }

        //update ORM

        public void updateCustomertORM(string dbpath, string oldCust, string newCust)
        {
            try
            {
                //var ctx = new SQLiteDatabaseContext(dbpath);
                using (var ctx = new SQLiteDatabaseContext(dbpath))
                {
                    ctx.Database.ExecuteSqlCommand("UPDATE ordini SET customer = '" + newCust + "'  WHERE customer = '" + oldCust + "'");



                }
            }
            catch (Exception ex)
            {
                errorLog("insert ORM" + ex.Message);
            }
        }

        //update ORM

        /*public void deleteCustomertORM(string dbpath, string oldCust, string newCust)
        {
            try
            {
                //var ctx = new SQLiteDatabaseContext(dbpath);
                using (var ctx = new SQLiteDatabaseContext(dbpath))
                {
                    ctx.Database.ExecuteSqlCommand("DELETE FROM ordini WHERE customer = '" + cust + "');



                }
            }
            catch (Exception ex)
            {
                errorLog("insert ORM" + ex.Message);
            }
        }*/

        public string selectCustomerListORMBis(string dbpath, int n)
        {
            String ret = "";
            List<string> custLst = null;


            try
            {
                //var ctx = new SQLiteDatabaseContext(dbpath);
                using (var ctx = new SQLiteDatabaseContext(dbpath))
                {
                    custLst = ctx.Database.SqlQuery<string>("SELECT distinct customer from ordini ORDER BY RANDOM() LIMIT " + n).ToList();
                }
                IList<string> lstOutString = new List<string>();
                foreach (var cust in custLst)
                {
                    lstOutString.Add("'" + cust + "'");
                }
                ret = string.Join(",", lstOutString);
            }

            catch (Exception ex)
            {
                Trace.WriteLine("[PERSISTENCE] " + ex.Message);
            }

            return ret;
        }

        public IList<string> selectAllCustomersListORMBis(string dbpath)
                             
        {
            List<string> custLst = null;
            IList<string> lstOutString = new List<string>();

            try
            {
                //var ctx = new SQLiteDatabaseContext(dbpath);
                using (var ctx = new SQLiteDatabaseContext(dbpath))
                {
                    custLst = ctx.Database.SqlQuery<string>("SELECT distinct customer from ordini").ToList();
                }
                lstOutString = new List<string>();
                foreach (var cust in custLst)
                {
                    lstOutString.Add("'" + cust + "'");
                }           
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[PERSISTENCE] " + ex.Message);
            }
            return lstOutString;
        }

        // Reads an instance from the db
        public GAPclass readGAPinstance(string dbpath)
        {
            int i, j;
            List<int> lstCap;
            List<double> lstCosts;
            GAPclass G = new GAPclass();

            try
            {
                using (var ctx = new SQLiteDatabaseContext(dbpath))
                {
                    lstCap = ctx.Database.SqlQuery<int>("SELECT cap from capacita").ToList();
                    G.m = lstCap.Count();
                    G.cap = new int[G.m];
                    for (i = 0; i < G.m; i++)
                        G.cap[i] = lstCap[i];

                    lstCosts = ctx.Database.SqlQuery<double>("SELECT cost from costi").ToList();
                    G.n = lstCosts.Count / G.m;
                    G.c = new double[G.m, G.n];
                    G.req = new int[G.n];
                    G.sol = new int[G.n];
                    G.solbest = new int[G.n];
                    G.zub = Double.MaxValue;
                    G.zlb = Double.MinValue;

                    for (i = 0; i < G.m; i++)
                        for (j = 0; j < G.n; j++)
                            G.c[i, j] = lstCosts[i * G.n + j];

                    for (j = 0; j < G.n; j++)
                        G.req[j] = -1;          // placeholder
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[readGAPinstance] Error:" + ex.Message);
            }

            Trace.WriteLine("Fine lettura dati istanza GAP");
            return G;
        }

    }
}





    
