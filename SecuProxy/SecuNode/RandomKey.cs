using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuhrerShare.Core.Security
{
    internal class RandomKey
    {
        internal void GenerateOTP()
        {
            Config.OTP = System.Web.Security.Membership.GeneratePassword(200, 10);
        }
    }
}