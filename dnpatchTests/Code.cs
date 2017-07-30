using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dnpatchTests
{
    static class Code
    {
        public static string Security = @"using System;namespace Security
{public static class Security
{public static bool IsLicensed()
{Console.WriteLine(""Not Licensed!"");return false;}}}
";

        public static string UI = @"using System;namespace UI
{public static class UI
{public static string GetCredits()
{return""Developed By FlatHub"";}}}
";
    }
}
