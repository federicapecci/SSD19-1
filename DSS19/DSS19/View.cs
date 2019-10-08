using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;



namespace DSS19
{
    public partial class App : Form
    {

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
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
          
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void readDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            readDb();
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
    }
}
