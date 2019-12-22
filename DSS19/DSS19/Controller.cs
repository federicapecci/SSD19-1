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

        //actual e forecast per ogni cliente
        public async Task<int> ForecastSpecificCustomerOrderChart(string dbOrdiniPath, string pyScript, string customer)
        {

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
                        Trace.WriteLine(s);
                    }
                }
            }
            catch (Exception exception)
            {
                Trace.WriteLine("[CONTROLLER] errore: " + exception.Message);
            }

            return (int)Math.Round(fcast); // da strina a double a intero
        }

        //l'ultima FORECAST del customer
        public async Task<int> LastForecastSpecificCustomerOrderChart(string dbOrdiniPath, string pyScript, string customer)
        {

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
                Trace.WriteLine("Customer " + customer + ", forecast " + fcast);
         
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

        public async Task<int[]> LastForecastAllCustomersOrderChart(string dbOrdiniPath, string pyScript)
        {
            IList<int> customerForecasts = new List<int>();
            int strCustomerForecast = 0;
            IList<string> allCustomers = readAllClientiDB(dbOrdiniPath);
            foreach (string customer in allCustomers)
            {
                strCustomerForecast = await LastForecastSpecificCustomerOrderChart(dbOrdiniPath, pyScript, customer);
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
                GAP.req = await LastForecastAllCustomersOrderChart(dbPath, pythonFile); //TO DO dbPath deve essere quello con i dati del 2019
                File.WriteAllLines("GAPreq.dat", GAP.req.Select(x => x.ToString()));
            }

            readGAPinstance(dbPath);

            double zub = GAP.SimpleContruct();

            Trace.WriteLine($"Constructive, zub = {zub}");
            zub = GAP.opt10(GAP.c);  //cerca di ottimizzare la soluzione precedente zub = GAP.SimpleContruct();
            Trace.WriteLine($"opt10, zub = {zub}");
            zub = GAP.TabuSearch(30, 100); // zub = 31000
            Trace.WriteLine($"TabuSearch, zub = {zub}");
        }

        // Reads an instance from the db
        public void readGAPinstance(string dbOrdinipath)
        {
            int i, j;
            List<int> lstCap;
            List<double> lstCosts;

            try
            {
                using (var ctx = new SQLiteDatabaseContext(dbOrdinipath))
                {
                    lstCap = ctx.Database.SqlQuery<int>("SELECT cap from capacita").ToList();
                    GAP.m = lstCap.Count();
                    GAP.cap = new int[GAP.m];
                    for (i = 0; i < GAP.m; i++)
                        GAP.cap[i] = lstCap[i];

                    lstCosts = ctx.Database.SqlQuery<double>("SELECT cost from costi").ToList();
                    GAP.n = lstCosts.Count / GAP.m;
                    GAP.c = new double[GAP.m, GAP.n];
                    GAP.req = new int[GAP.n];
                    GAP.sol = new int[GAP.n];
                    GAP.solbest = new int[GAP.n];
                    GAP.zub = Double.MaxValue;
                    GAP.zlb = Double.MinValue;

                    for (i = 0; i < GAP.m; i++)
                        for (j = 0; j < GAP.n; j++)
                            GAP.c[i, j] = lstCosts[i * GAP.n + j];

                    for (j = 0; j < GAP.n; j++)
                        GAP.req[j] = -1;          // placeholder
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[readGAPinstance] Error:" + ex.Message);
            }

            Trace.WriteLine("Fine lettura dati istanza GAP");
        }
    }
}
