using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuhrerShare.Core.Setup
{
    public class PrMethod_Settings
    {
        public void SetLow()
        {
            Config.ProtectMethod = Enums.ProtectionMethod.PrMethod.low;

        }
        public void SetMedium()
        {
            Config.ProtectMethod = Enums.ProtectionMethod.PrMethod.medium;

        }
        public void SetHigh()
        {
            Config.ProtectMethod = Enums.ProtectionMethod.PrMethod.high;

        }
        public void SetParanoid()
        {
            Config.ProtectMethod = Enums.ProtectionMethod.PrMethod.paranoid;
            Config.UseOTP = true;
            Config.EnforceSSL = true;
        }
    }
}