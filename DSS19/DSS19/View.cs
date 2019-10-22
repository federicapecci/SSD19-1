using System;
using System.Windows.Forms;
using System.Diagnostics;



namespace DSS19
{

    public partial class App : Form
    {


        private int customerRandomNumber;
        private Controller C;
        TextBoxTraceListener _textBoxListener;
        
        public App()
        {
            InitializeComponent();
            _textBoxListener = new TextBoxTraceListener(txtConsole);
            Trace.Listeners.Add(_textBoxListener);

            string dbPath = "";
            OpenFileDialog OFD = new OpenFileDialog(); //finestra per caricare il file del db
            if(OFD.ShowDialog() == DialogResult.OK) //TODO mancano i controlli sul tipo del file
            {
                dbPath = OFD.FileName;
                txtConsole.AppendText("Sqlite file name: "+dbPath+Environment.NewLine);
            }
            C = new Controller(dbPath);
            Random rnd = new Random();
            customerRandomNumber = rnd.Next(1, 100);
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
            //C.readCustomerListORM(   ,100);
        }

        private void readDb()
        {
             txtConsole.AppendText("Read Db clicked \n");
            //C.readDb(txtCustomer.Text);
        }

        private void loadDb()
        {
            txtConsole.AppendText("Load Db button clicked \n");
            C.readDb(txtCustomer.Text);
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
