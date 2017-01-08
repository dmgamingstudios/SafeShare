using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FuhrerShare
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!File.Exists(Application.StartupPath + "\\nodes.dat"))
                BeginDownloadNodeFile();
        }
        internal void BeginDownloadNodeFile()
        {
            if(Config.UseClear)
            {

            }
            else if(Config.UseI2P)
            {

            }
            else if(Config.UseTor)
            {

            }
        }
    }
}