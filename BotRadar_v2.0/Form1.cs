using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace BotRadar_v2._0
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ResizeControls();
            this.Resize += Form1_Resize;
            

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            ResizeControls();
        }

        private void ResizeControls()
        {
            this.tabControl1.Width = this.Width;
            this.tabControl1.Height = this.Height;
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ( openFileDialog1.ShowDialog(this) == DialogResult.OK )
            {
                String fname = openFileDialog1.FileName;
                List<String> fs = File.ReadLines( fname ).ToList();
                foreach( String line in fs )
                {
                    listView1.Items.Add(line);
                }
                

            }
        }

        private void googleSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BotCrawler.PullSearchResults("site:mashreq.com");
            MessageBox.Show("Done!");
        }
    }
}
