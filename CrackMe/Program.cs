using System;

namespace CrackMe
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(UI.UI.GetCredits());
            if(Security.Security.IsLicensed())
            {
                Console.WriteLine("NSA Hacked");
            }
            else
            {
                Console.WriteLine("You're are not Elliot Alderson!");
            }

            Console.Read();
        }
    }
}
