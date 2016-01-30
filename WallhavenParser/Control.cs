using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using Nito.AsyncEx;
using System.Threading;

namespace WallhavenParser
{
    public partial class Control : Form
    {
        private bool isWorking = false;
        public Control()
        {
            InitializeComponent();
            notifyIcon1.Visible = false;
            this.notifyIcon1.MouseDoubleClick += new MouseEventHandler(notifyIcon1_MouseDoubleClick);
            this.Resize += new System.EventHandler(this.Form1_Resize);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {

            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;

            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            notifyIcon1.Visible = false;
            WindowState = FormWindowState.Normal;


        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (!isWorking)
            {
                if (textBox1.Text.Length > 0)
                {
                    Program.Query = textBox1.Text;
                }
                if (textBox2.Text.Length > 0)
                {
                    Program.Path = textBox2.Text;
                }
                if (textBox3.Text.Length > 0)
                {
                    Program.Timeout = int.Parse(textBox3.Text);
                }
                this.WindowState = FormWindowState.Minimized;
                new Program().MainAsync().ConfigureAwait(false);
                isWorking = true;
                button1.Text = "YOU CANNOT STOP THIS MADNESS NOW";
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                button1.Enabled = false;
            }
            else
            { 
            }
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
