using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Configuration;
using System.Data.Common;

namespace DSS19
{

    class Controller
    {
        private Persistence P = new Persistence();
        string connectionString;
        string dbpath;

        public Controller(string _dbpath)
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
        }

        public void readDb(string custID) 
        {
            Trace.WriteLine("Controller read DB");
            if(custID == "")
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

      


    }
}
