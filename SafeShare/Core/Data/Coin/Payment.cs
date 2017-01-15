using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuhrerShare.Core.Data.Coin
{
    public class Payment
    {
        public string payer = null;
        public string receiver = null;
        public string paymentID = null;
        public Int32 Amount = 0;
        public Payment()
        {

        }
    }
}