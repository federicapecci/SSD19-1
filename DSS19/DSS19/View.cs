using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Configuration;
using System.Drawing;

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

            btnSARIMA.Enabled = false;
            btnLocalSearch.Enabled = false;

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
            C.readClientiDB(dbOrdiniPath);
        }

        private async void loadDb()
        {
            txtConsole.AppendText("Load Db button clicked \n");
            C.readClientiDB(dbOrdiniPath);
            Bitmap bm = await C.readCustomerOrdersChart(dbOrdiniPath);
            pictureBox2.Image = bm;
            btnSARIMA.Enabled = true;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            loadDb();
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

    }
}
