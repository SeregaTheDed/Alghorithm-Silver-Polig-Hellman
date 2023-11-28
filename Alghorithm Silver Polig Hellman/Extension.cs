using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Alghorithm_Silver_Polig_Hellman
{
    public static class Extension
    {
        public static BigInteger BigIntegerModPowWithMinusOne(this BigInteger a, BigInteger b, BigInteger Modulus)
        {
            if (b <= -1)
                return BigInteger.ModPow(FindObratniiElement(a, Modulus), -b, Modulus);
            return BigInteger.ModPow(a, b, Modulus);
        }
        public static void Print(this List<List<BigInteger>> tableR)
        {
            foreach (var row in tableR) 
            {
                Console.WriteLine(String.Join(" ", row));
            }
        }




        public static BigInteger FindObratniiElement(BigInteger a, BigInteger m)
        {
            BigInteger x, y;
            BigInteger g = GCD(a, m, out x, out y);
            if (g != 1)
                throw new ArgumentException();
            return (x % m + m) % m;
        }

        private static BigInteger GCD(BigInteger a, BigInteger b, out BigInteger x, out BigInteger y)
        {
            if (a == 0)
            {
                x = 0;
                y = 1;
                return b;
            }
            BigInteger x1, y1;
            BigInteger d = GCD(b % a, a, out x1, out y1);
            x = y1 - (b / a) * x1;
            y = x1;
            return d;
        }
    }
}
