using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Configuration;
using System.Data.Common;
using PyGAP2019;
using System.Drawing;
using System.IO;
using System.Globalization;

namespace DSS19
{

    class Controller
    {
        private Persistence P = new Persistence();
        string connectionString;
        string dbpath;

        string pythonPath;
        string pythonScriptsPath;
        PythonRunner pyRunner;
        string strCustomer;
        IList<string> allCustomers;

        GAPclass GAP;

        /*public Controller(string _dbpath)
        {
            dbpath = _dbpath;
            string sdb = ConfigurationManager.AppSettings["dbServer"]; 

            switch (sdb)
            {
                case "SQLiteConn": 
                    connectionString = ConfigurationManager.ConnectionStrings["SQLiteConn"].ConnectionString;
                    connectionString = connectionString.Replace("DBFILE", dbpath); 
                    P.factory = ConfigurationManager.ConnectionStrings["SQLiteConn"].ProviderName;
                    break;
                case "LocalDbConn":
                    connectionString = ConfigurationManager.ConnectionStrings["LocalSqlServConn"].ConnectionString; 
                    P.factory = ConfigurationManager.ConnectionStrings["LocalSqlServConn"].ProviderName;
                    break;
                case "RemoteSqlServConn": 
                    connectionString = ConfigurationManager.ConnectionStrings["RemoteSQLConn"].ConnectionString;
                    P.factory = ConfigurationManager.ConnectionStrings["RemoteSQLConn"].ProviderName;
                    break;

            }
            P.connectionString = connectionString;
        }*/

        public Controller(string _pyPath, string _pyScriptPath)
        {
            this.pythonPath = _pyPath;
            this.pythonScriptsPath = _pyScriptPath;
            this.pyRunner = new PythonRunner(pythonPath, 20000);
        }


        public void readDb(string custID)
        {
            Trace.WriteLine("Controller read DB");
            if (custID == "")
            {
                P.selectFirstRecords();
            }
            else
            {
                P.selectOrdersByCustId(custID);
            }
        }

        public void insert(string cust)
        {
            //P.insert(cust);
            P.insertCustomertORM(dbpath, cust);
        }

        public void delete(string cust)
        {
            P.delete(cust);
        }

        public void update(string oldCust, string newCust)
        {
            //P.update(oldCust, newCust);
            P.updateCustomertORM(dbpath, oldCust, newCust);
        }

        public void readCustomerListORM(int n)
        {
            P.selectCustomerListORM(dbpath, n);
        }

        public void readQuantitiesListORM()
        {
            P.selectQuantitiesListORM(dbpath);
        }

        //legge una stringa di codice clienti da graficare
        public string readClientiDB(string dbOrdiniPath, int customerNumber)
        {
            int numSerie = customerNumber; //numero di clienti di cui leggere la serie
            strCustomer = P.selectCustomerListORMBis(dbOrdiniPath, numSerie);
            Trace.WriteLine($"Clienti: {strCustomer}");
            return strCustomer;
        }

        public IList<string> readAllClientiDB(string dbOrdiniPath)
        {
            //numero di clienti di cui leggere la serie
            allCustomers = P.selectAllCustomersListORMBis(dbOrdiniPath);
            //Trace.WriteLine($"Clienti: {strCustomers}");
            return allCustomers;
        }

        public async Task<Bitmap> readCustomerOrdersChart(string dbOrdiniPath, string pyScript)
        {
            Trace.WriteLine("Getting the orders chart...");
            //pythonScriptsPath = System.IO.Path.GetFullPath(pythonScriptsPath);
            pythonScriptsPath = @"C:\Users\federica.pecci2\Documents\GitHub\SSD19-1\DSS19\DSS19\python_scripts";

            try
            {

                Bitmap bmp = await pyRunner.getImageAsync(
                    pythonScriptsPath,
                    pyScript,  // chartOrders.py o nuovo script
                    pythonScriptsPath,
                    dbOrdiniPath,
                    strCustomer); //strCustomers riga dei customer restituita dal db, select dei customer random
                return bmp;
            } catch (Exception exception)
            {
                Trace.WriteLine("[CONTROLLER] errore: " + exception.Message);
                return null;
            }
        }

        public async Task<int> ForecastSpecificCustomerOrderChart(string dbOrdiniPath, string pyScript, string customer)
        {
            Trace.WriteLine("Getting the orders chart...");
            pythonScriptsPath = @"C:\Users\federica.pecci2\Documents\GitHub\SSD19-1\DSS19\DSS19\python_scripts";
            double fcast = double.NaN;
            try
            {
                string list = await pyRunner.getStringsAsync(
                    pythonScriptsPath,
                    pyScript,  // chartOrders.py o nuovo script
                    pythonScriptsPath,
                    dbOrdiniPath,
                    customer); //strCustomers riga dei customer restituita dal db, select dei customer random

                NumberFormatInfo provider = new NumberFormatInfo();
                provider.NumberDecimalSeparator = ".";
                string[] lines = list.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string s in lines)
                {
                    if (s.StartsWith("Actual"))
                    {
                        fcast = Convert.ToDouble(s.Substring(s.LastIndexOf(" ")), provider);    
                    }
                }
                Trace.WriteLine(customer);
                Trace.WriteLine(fcast);
            }
            catch (Exception exception)
            {
                Trace.WriteLine("[CONTROLLER] errore: " + exception.Message);
            }

            return (int)Math.Round(fcast); // da strina a double a intero
        }


        // TODO -> prendere il valore di forecast per tutti i customer e 
        // salvarli in un array 
        public async Task<int[]> ForecastAllCustomersOrderChart(string dbOrdiniPath, string pyScript)
        {
            IList<int> customerForecasts = new List<int>();
            int strCustomerForecast = 0;
            foreach(string customer in allCustomers)
            {
                strCustomerForecast = await ForecastSpecificCustomerOrderChart(dbOrdiniPath, pyScript, customer);
                customerForecasts.Add(strCustomerForecast);
            }
            return customerForecasts.ToArray();
        }

        // Ricerca locale di istanze GAP
        public async void OptimizeGAP(string dbPath, string pythonFile)
        {
            GAP = new GAPclass();

            if (File.Exists("GAPreq.dat"))
            {
                string[] txtData = File.ReadAllLines("GAPreq.dat");
                GAP.req = Array.ConvertAll<string, int>(txtData, new Converter<string, int>(i => int.Parse(i)));
            }
            else
            {
                GAP.req = await ForecastAllCustomersOrderChart(dbPath, pythonFile); //TO DO dbPath deve essere quello con i dati del 2019
                File.WriteAllLines("GAPreq.dat", GAP.req.Select(x => x.ToString()));
            }

            double zub = GAP.SimpleContruct();
            Trace.WriteLine($"Constructive, zub ) = {zub}");
            //zub = GAP.Opt10(GAP.c);
            //Trace.WriteLine($"Local search, zub = {zub}");
        }
    }
}
