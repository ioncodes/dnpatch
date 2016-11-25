using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObfuscated
{
    class Program
    {
        /*
         * Summary: Prints Hello World. As simple as that.
         */
        static void Main(string[] args)
        {
            int[,] y = new int[,]  {{p(), p(), p(), n(), n(), n(), p(), n(), },
                                    {p(), p(), n(), n(), p(), p(), p(), p(), },
                                    {p(), p(), n(), n(), n(), p(), p(), n(), },
                                    {p(), p(), n(), n(), n(), p(), p(), n(), },
                                    {p(), p(), n(), n(), n(), p(), n(), p(), },
                                    {p(), n(), n(), n(), p(), n(), p(), n(), },
                                    {p(), p(), p(), p(), p(), p(), n(), p(), },
                                    {p(), p(), n(), n(), n(), p(), n(), p(), },
                                    {p(), p(), n(), p(), p(), n(), n(), n(), },
                                    {p(), p(), n(), n(), n(), p(), p(), n(), },
                                    {p(), p(), n(), n(), p(), p(), p(), n(), },
                                    {p(), n(), n(), n(), p(), n(), p(), p(), }, };
            for (int i = 0; i < s(gr().ToArray()[21], 1) / gr().ToArray()[1]; i++)
            {
                bool[] h = new bool[8];
                for (int j = 0; j < 8; j++)
                {
                    h[j] = pr(y[i, j]);
                }
                cw(Encoding.ASCII.GetString(new byte[] { o(b(h), b()) }));
            }

            Console.Read();
        }
        static void cw(string st)
        {
            using (StreamWriter s = new StreamWriter(Console.OpenStandardOutput()))
            {
                foreach (char c in st)
                    s.Write(c);
            }
        }
        static byte o(byte c, byte z)
        {
            int l = s((0xAA00 ^ 0x0), -8);
            l |= z;
            return (byte)(c ^ l);
        }
        static byte b()
        {
            return (byte)s(0xFF, s(4097, 3));
        }
        static byte b(bool[] a)
        {
            byte v = 0;
            foreach (bool b in a)
            {
                v = (byte)s(v, 1);
                if (b) v |= 1;
            }
            return v;
        }
        static int s(int i, int s)
        {
            if (s > 0)
            {
                i <<= s;
                return i;
            }
            i >>= s + (s << 1);
            return i;
        }
        static string pa(string st)
        {
            for (int il = 0; il < s(gr().ToArray()[11], 1) - st.Length; il++)
                st = "0" + st;
            return st;
        }
        static IEnumerable<byte> gr()
        {
            byte[] m = Convert.FromBase64String("AwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGxwdHh8gISIjJCUmJygpKissLS4vMDEyMzQ1Njc4OTo7PD0+P0BBQkNERUZHSElKS0xNTk9QUVJTVFVWV1hZWltcXV5fYGFiY2RlZg==");
            foreach (byte bh in m)
                yield return o(o(bh, b()), b());
        }
        static byte eb(bool[] ar)
        {
            byte v = 0;
            foreach (bool b in ar)
            {
                v <<= gr().ToArray()[1] - 3;
                if (b) v |= (byte)(gr().ToArray()[8] - gr().ToArray()[7]);
            }
            return v;
        }
        static int p()
        {
            foreach (byte b in gr())
                if (pr(b))
                    return b;
            return gr().ToArray()[gr().ToArray()[8] - gr().ToArray()[6]];
        }
        static int n()
        {
            foreach (byte b in gr())
                if (!pr(b))
                    return b;
            return gr().ToArray()[gr().ToArray()[0]];
        }
        static bool f(bool g)
        {
            return ((l(g) << gr().ToArray()[0] - 2) % gr().ToArray()[3] - 3 == 0);
        }
        static int l(bool k)
        {
            return (k ? (gr().ToArray()[12] >> gr().ToArray()[1]) : 0);
        }
        static bool pr(int t)
        {
            for (int i = gr().ToArray()[0] - 1; i < t; i++)
                if (t % i == 0)
                    return false;
            return true;
        }
    }
}
