using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Alghorithm_Silver_Polig_Hellman
{
    internal class PrimeAndPow
    {
        BigInteger Prime {  get; set; }
        BigInteger Pow { get; set; }
        public PrimeAndPow(BigInteger Prime, BigInteger Pow) 
        {
            this.Pow = Pow;
            this.Prime = Prime;
        }
    }
}
