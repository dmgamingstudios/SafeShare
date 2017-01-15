using FuhrerShare.Core.Nodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuhrerShare.Core.Data.Coin
{
    public class MoneyController
    {
        public string SuperVerifyPayment(SuperCoinNode me, Payment payment)
        {
            string[] ChainData = File.ReadAllLines(new CoinChain().CoinChainFile);
            string[] results = Array.FindAll(ChainData, s => s.Contains(payment.payer));
            if(results.Length != 1)
            {
                return me.name + "|CANTVERIFY|Either the payment doesnt exist or some error occured inside the network..|" + payment;
            }
            string[] PaymentData = results[0].Split('|');
            if((Int32.Parse(PaymentData[1]) - payment.Amount) <= 0)
            {
                return me.name + "|CANTVERIFY|The payer does not have enough money..|" + payment;
            }
            return me.name + "|VERIFIED|Payment verification was succesfull..|" + payment;
        }
    }
}