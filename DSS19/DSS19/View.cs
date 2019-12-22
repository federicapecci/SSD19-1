using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Configuration;
using System.Drawing;
using System.Collections.Generic;

namespace DSS19
{

    public partial class App : Form
    {

        private int customerRandomNumber;
        private Controller C;
        TextBoxTraceListener _textBoxListener;
        string dbOrdiniPath;
        string pythonPath;
        string pythonScriptsPath;



        public App()
        {
            InitializeComponent();
            _textBoxListener = new TextBoxTraceListener(txtConsole);
            Trace.Listeners.Add(_textBoxListener);

            //btnSARIMA.Enabled = false;
            //btnLocalSearch.Enabled = false;

            btnSARIMA.Enabled = true;
            //Forecast.Enabled = true;

            dbOrdiniPath = ConfigurationManager.AppSettings["dbordiniFile"];
            pythonPath = ConfigurationManager.AppSettings["pythonPath"];
            pythonScriptsPath = ConfigurationManager.AppSettings["pyScripts"];

            C = new Controller(pythonPath, pythonScriptsPath);

        }


        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void readDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            readDb();
        }


        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
        }

        private void readDb()
        {
            txtConsole.AppendText("Read Db clicked \n");
            //C.readClientiDB(dbOrdiniPath);
        }

        //metodo richiamato dal bottone "ARIMA"
        private async void loadDb(string pyScript, int customerNumber)
        {       
            string customer = C.readClientiDB(dbOrdiniPath, customerNumber);
            
            //DA SCOMMENTARE SE VOGLIO VEDERE IL GRAFICO DI FORECAST
            //stampo la bitmap in un grafico 
            Bitmap bm = await C.readCustomerOrdersChart(dbOrdiniPath, pyScript);
            pictureBox2.Image = bm;
            
            //STAMPO LE FORECAST DI UN CUSTOMER SU TRACELINE
            await C.ForecastSpecificCustomerOrderChart(dbOrdiniPath, pyScript, customer);           
         
        }

        //metodo richiamato dal bottone "OPTIMIZE"
        private async void loadAllCustomersDb(string pyScript)
        {
            C.readAllClientiDB(dbOrdiniPath);
            await C.ForecastAllCustomersOrderChart(dbOrdiniPath, pyScript);

        }

        //SARIMA
        private async void loadLastAllCustomersForecast(string pyScript)
        {
            C.readAllClientiDB(dbOrdiniPath);           
            await C.LastForecastAllCustomersOrderChart(dbOrdiniPath, pyScript);

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            loadDb("chartOrders.py", 52);
        }

        private void btnSARIMA_Click(object sender, EventArgs e)
        {
            loadDb("arima_forecast.py", 1);
        }

        private void btnOptimize_Click(object sender, EventArgs e)
        {
           
            C.OptimizeGAP(dbOrdiniPath, "arima_forecast.py");
        }

        private void txtConsole_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void txtCustomer_TextChanged(object sender, EventArgs e)
        {

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            C.delete(txtCustomer.Text);
        }

        private void insertBtn_Click(object sender, EventArgs e)
        {
            C.insert(txtNewCustomer.Text);
        }

        private void updateBtn_Click(object sender, EventArgs e)
        {

            C.update(txtCustomer.Text, txtNewCustomer.Text);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void App_Load(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void readCustomerORMToolStripMenuItem_Click(object sender, EventArgs e)
        {      
            C.readCustomerListORM(customerRandomNumber);
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void readQuantitiesORMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            C.readQuantitiesListORM();
        }

        private void btnLocalSearch_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {

        }

        //SARIMA
        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            //loadAllCustomersDb("arima_forecast.py");
            loadLastAllCustomersForecast("arima_forecast.py");
        }
    }
}
