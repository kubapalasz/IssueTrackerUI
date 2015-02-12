using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Extentions
{
    public static class Extentions
    {
        public static string FormattedDate(this DateTime dateTime)
        {
            return dateTime.ToString("u").Substring(0, 10);
        }
    }
}
