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
                    //listView1.Items.Add(line);
                }
                

            }
        }
        Bing_ResultsCrawler bingCrawler;
        private void googleSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void startToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void continueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fName = "url_list.csv";
            if(bingCrawler != null)
                bingCrawler.PullSearchResults(fName);
            MessageBox.Show("Done!");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string fName = "url_list.csv";
            bingCrawler = new Bing_ResultsCrawler(textBox1.Text, textBox2.Text);
            bingCrawler.PullSearchResults(fName);
            MessageBox.Show("Done!");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string fName = "url_list.csv";
            bingCrawler.PullSearchResults(fName);
            MessageBox.Show("Done!");
        }

        private void button3_Click(object sender, EventArgs e)
        {
           if (openFileDialog1.ShowDialog() == DialogResult.OK )
            {
                FileStream fs = File.OpenRead( openFileDialog1.FileName );
                StreamReader sr = File.OpenText(openFileDialog1.FileName);
                string str = string.Empty;
                int ctr = 0;
                while (!sr.EndOfStream)
                {
                    str = sr.ReadLine();
                    string []strs = str.Split('#');
                    listView1.Items.Insert(ctr++, str);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            String fName = "combined_urls.csv";
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                string str = listView1.Items[i].Text;
                string[] strs = str.Split('#');
                // MessageBox.Show(strs[0] + " " + strs[1]);
                if (i == 0)
                {
                    bingCrawler = new Bing_ResultsCrawler(strs[0].Trim().ToLower(), strs[1].Trim().ToLower());
                    bingCrawler.PullSearchResults(fName);
                }else
                {
                    bingCrawler.searchString = strs[0].Trim();
                    bingCrawler.searchUrlFilter = strs[1].Trim();
                    bingCrawler.searchUrl = bingCrawler.searchStartUrl + bingCrawler.searchString + bingCrawler.searchEndUrl;
                    bingCrawler.PullSearchResults(fName);
                }
            }
            MessageBox.Show("Done!!");
        }
    }
}
