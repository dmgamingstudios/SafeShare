using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuhrerShare.Core.Security
{
    public class RandomKey
    {
        public void GenerateOTP()
        {
            Config.OTP = System.Web.Security.Membership.GeneratePassword(200, 10);
        }
    }
}