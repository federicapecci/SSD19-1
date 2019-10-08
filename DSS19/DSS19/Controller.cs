using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Configuration;

namespace DSS19
{

    class Controller
    {
        private Persistence P = new Persistence();
        string connectionString;
       // public string dbpath;

        public Controller(string dbpath)
        {
           
            //string dbpath = @"C:\Users\Enrico\Desktop\ordiniMI2018.sqlite";
            string sdb = ConfigurationManager.AppSettings["dbServer"]; 

            switch (sdb)
            {
                case "SQLiteConn": connectionString = ConfigurationManager.ConnectionStrings["SQLiteConn"].ConnectionString;
                                   connectionString = connectionString.Replace("DBFILE", dbpath); 
                                   break;
                    //factory = ConfigurationManager.ConnectionStrings["SQLiteConn"].ProviderName; break;
                case "LocalDbConn":connectionString = ConfigurationManager.ConnectionStrings["LocalSqlServConn"].ConnectionString; break;
                //factory = ConfigurationManager.ConnectionStrings["LocalSqlServConn"].ProviderName; break;
                case "RemoteSqlServConn": connectionString = ConfigurationManager.ConnectionStrings["RemoteSQLConn"].ConnectionString; break;
            }
            P.connectionString = connectionString;
        }

        public void readDb(string custID) 
        {
            Trace.WriteLine("Controller read DB");
            if(custID == "")
            {
                P.readDb();
            }
            else
            {
                P.readDb(custID);
            }
        }

        public void insert(string cust)
        {
            P.insert(cust);
        }

        public void delete(string cust)
        {
            P.delete(cust);
        }

        public void update(string oldCust, string newCust)
        {
            P.update(oldCust, newCust);
        }
    }
}
