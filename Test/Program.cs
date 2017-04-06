using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        private bool _isPremium = false;

        public bool IsPremium
        {
            get { return _isPremium; }
        }

        static void Main(string[] args)
        {
            Print();
            Check(10);
            PrintAlot();
            Foo.Bar foobar = new Foo.Bar();
            foobar.NestedPrint();
            I.Am.A.Burger burger = new I.Am.A.Burger();
            burger.Eat();
            FindMe();
            ReplaceMe();
            RemoveMe();
            VerifyMe();
            VerifyMeNot();
            WriteLog();
            BigMethodBootySorryIMeanBody();
            SameName();
            SameName("SameName: 1 string param");
            SameName(1337);
            SameName("SameName: 2 param; string, int ", 1337);
            FindSomeILByRegex();
            Console.Read();
        }

        static void Print()
        {
            Console.WriteLine("Hello");
        }

        static void Check(int i)
        {
            Console.WriteLine(i > 0 ? "Error" : "Secret Key Here");
        }

        static void PrintAlot()
        {
            Console.WriteLine("Hello");
            Console.WriteLine("Hello");
            Console.WriteLine("Hello");
            Console.WriteLine("Hello");
            Console.WriteLine("Hello");
            Console.WriteLine("Hello");
            Console.WriteLine("Hello");
            Console.WriteLine("Hello");
        }

        static void FindMe()
        {
            Console.WriteLine("You");
            Console.WriteLine("Wont");
            Console.WriteLine("Find");
            Console.WriteLine("TheWord");
            Console.WriteLine("The");
            Console.WriteLine("Word");
        }

        static void ReplaceMe()
        {
            Console.WriteLine("I love dogs");
        }

        static void RemoveMe()
        {
            Console.WriteLine("The next sentence is a lie");
            Console.WriteLine("ion is best");
        }

        static bool VerifyMe()
        {
            Console.WriteLine("Verification failed");
            return false;
        }

        static bool VerifyMeNot()
        {
            Console.WriteLine("Verification worked, but that's not good");
            return true;
        }

        static void WriteLog()
        {
            Console.WriteLine("harmful log 1");
            Console.WriteLine("harmful log 2");
            Console.WriteLine("harmful log 3");
            Console.WriteLine("harmful log 4");
        }

        static void BigMethodBootySorryIMeanBody()
        {
            Console.WriteLine("need to get some ideas");
            if (VerifyMe())
            {
                Console.WriteLine("for my console prints...");
            }
            int imUseless = 1337;
            for (int i = 0; i < imUseless; i++)
            {
                Console.WriteLine(i);
            }
        }

        static void SameName()
        {
            Console.WriteLine("SameName: No params");
        }

        static void SameName(string t)
        {
            Console.WriteLine(t);
        }

        static void SameName(int i)
        {
            Console.WriteLine("SameName: 1 int param " + i);
        }

        static void SameName(string t, int i)
        {
            Console.WriteLine(t + i);
        }

        static void FindSomeILByRegex()
        {
            int imUseless = 1337;
            for (int i = 0; i < imUseless; i++)
            {
                Console.WriteLine(i);
            }
            imUseless = 1233;
            for (int i = 0; i < imUseless; i++)
            {
                Console.WriteLine(i);
            }
            imUseless = 3123;
            for (int i = 0; i < imUseless; i++)
            {
                Console.WriteLine(i);
            }
            imUseless = 1231;
            for (int i = 0; i < imUseless; i++)
            {
                Console.WriteLine(i);
            }
            imUseless = 1123;
            for (int i = 0; i < imUseless; i++)
            {
                for(int j = 0; j < 9; j++) Console.WriteLine("Damn");
                Console.WriteLine(i);
            }
        }
    }
}
