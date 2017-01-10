using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecuSite_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SecuSite Server");
            if(!File.Exists("nodes.dat"))
            {
                Console.WriteLine("Please place a valid nodes.dat file in the same directory as this executable and restart");
                Environment.Exit(0);
            }

        }
    }
}