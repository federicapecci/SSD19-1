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
        }


    }
