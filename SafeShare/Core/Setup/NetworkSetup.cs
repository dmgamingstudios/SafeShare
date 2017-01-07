using FuhrerShare.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FuhrerShare.Core.Setup
{
    public partial class NetworkSetup : Form
    {
        ConnectionMethod.ConnMethod CM;
        internal NetworkSetup(ConnectionMethod.ConnMethod CM)
        {
            InitializeComponent();
            this.CM = CM;
        }

        private void NetworkSetup_Load(object sender, EventArgs e)
        {

        }
    }
}
