using FuhrerShare.Core.Nodes;
using FuhrerShare.Core.Setup;
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
        LocalSafeNode _node;
        public Form1(LocalSafeNode node)
        {
            InitializeComponent();
            _node = node;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!File.Exists(Application.StartupPath + "\\nodes.dat"))
               BeginDownloadNodeFile();
        }
        public void BeginDownloadNodeFile()
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