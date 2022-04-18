using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FastReport.Messaging
{
    internal static class TelegramUtils
    {
        internal static bool IsPhoneNumber(string number)
        {
            return Regex.Match(number, @"^([0-9]{11})$").Success;
        }
    }
}
