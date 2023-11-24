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
        public static BigInteger Pow(this BigInteger a, BigInteger b)
        {
            BigInteger result = BigInteger.One;
            for (BigInteger i = 0; i < b; i++)
            {
                result = result * a;
            }
            return result;
        }
        public static void Print(this List<List<BigInteger>> tableR)
        {
            foreach (var row in tableR) 
            {
                Console.WriteLine(String.Join(" ", row));
            }
        }
    }
}
