using System;
namespace Security
{
    public static class Security
    {
        public static bool IsLicensed()
        {
            Console.WriteLine("Not Licensed!");
            return false;
        }
    }
}
