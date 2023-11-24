using Alghorithm_Silver_Polig_Hellman;
using System;
using System.Collections.Generic;
using System.Numerics;

class SilverPohligHellman
{
    static void Main()
    {
        //https://cyberleninka.ru/article/n/primer-vychisleniya-diskretnogo-logarifma/viewer
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("Алгоритм Сильвера-Полига-Хеллмана");
        Console.WriteLine("a^x ≡ b (mod p)");
        Console.WriteLine("-------------------------------");

       /* Console.Write("Введите основание (a): ");
        BigInteger a = BigInteger.Parse(Console.ReadLine());*/
        BigInteger a = new BigInteger(2);

        /*Console.Write("Введите число (b): ");
        BigInteger b = BigInteger.Parse(Console.ReadLine());*/
        BigInteger b = new BigInteger(28);

        Console.Write("Введите модуль (p): ");
        //BigInteger p = BigInteger.Parse(Console.ReadLine());
        BigInteger p = new BigInteger(37);

        BigInteger x = SilverPohligHellmanAlgorithmNew(a, b, p);

        Console.WriteLine($"Дискретный логарифм x в уравнении {a}^x ≡ {b} (mod {p}) равен {x}");
    }

    static BigInteger SilverPohligHellmanAlgorithmNew(BigInteger a, BigInteger b, BigInteger p)
    {
        List<(BigInteger prime, BigInteger pow)> q1q2qn = GetPrimeFactors(p-1)
            .GroupBy(x => x)
            .Select(x =>  (x.Key, new BigInteger(x.Count())))
            .ToList();
        Console.WriteLine(String.Join(", ", q1q2qn));
        //List<List<BigInteger>> tableR = BuildTableOfR(a, p, q1q2qn);
        //tableR.Print();

        return BigInteger.MinusOne;
    }

    static List<List<BigInteger>> BuildTableOfR(BigInteger a, BigInteger p, List<BigInteger> q1q2qn)
    {
        List<List<BigInteger>> table = new List<List<BigInteger>>();
        foreach (BigInteger qi in q1q2qn)
        {
            List<BigInteger> row = new List<BigInteger>();
            for (BigInteger j = 0; j < qi - 1; j++)
            {
                BigInteger cell = a.Pow(j * (p - 1) / qi);
                row.Add(cell);
            }
            table.Add(row);
        }
        return table;
    }

    static BigInteger SilverPohligHellmanAlgorithm(BigInteger g, BigInteger y, BigInteger p)
    {
        Console.WriteLine("Шаг 1: Факторизация порядка группы");
        List<BigInteger> primeFactors = GetPrimeFactors(p - 1);
        Console.WriteLine($"Факторизация порядка группы (p-1): {string.Join(", ", primeFactors)}");

        Console.WriteLine("Шаг 2: Решение уравнений малых порядков");
        BigInteger x = 0;

        foreach (BigInteger q in primeFactors)
        {
            Console.WriteLine($"Решение для подгруппы порядка {q}");
            BigInteger alpha = FindAlpha(g, y, p, q);
            Console.WriteLine($"alpha = {alpha}");

            BigInteger beta = ExtendedEuclideanAlgorithm(g, q);
            Console.WriteLine($"beta = {beta}");

            BigInteger xq = (alpha * beta) % q;
            Console.WriteLine($"Для подгруппы порядка {q}: x = {xq}");

            x += xq * (p - 1) / q;
        }

        return x % (p - 1);
    }

    static List<BigInteger> GetPrimeFactors(BigInteger n)
    {
        List<BigInteger> factors = new List<BigInteger>();

        for (BigInteger i = 2; i * i <= n; i++)
        {
            while (n % i == 0)
            {
                factors.Add(i);
                n /= i;
            }
        }

        if (n > BigInteger.One)
            factors.Add(n);

        return factors;
    }

    static BigInteger FindAlpha(BigInteger g, BigInteger y, BigInteger p, BigInteger q)
    {
        BigInteger alpha = 1;

        while (ModuloExponentiation(g, alpha * (p - 1) / q, p) != 1 || ModuloExponentiation(y, alpha * (p - 1) / q, p) != 1)
        {
            alpha++;
        }

        return alpha;
    }

    static BigInteger ExtendedEuclideanAlgorithm(BigInteger a, BigInteger b)
    {
        BigInteger x0 = 1, x1 = 0, y0 = 0, y1 = 1;

        while (b != 0)
        {
            BigInteger q = a / b;
            BigInteger temp = b;
            b = a % b;
            a = temp;

            temp = x0;
            x0 = x1 - q * x0;
            x1 = temp;

            temp = y0;
            y0 = y1 - q * y0;
            y1 = temp;
        }

        return x1;
    }

    static BigInteger ModuloExponentiation(BigInteger a, BigInteger b, BigInteger m)
    {
        BigInteger result = 1;

        while (b > 0)
        {
            if (b % 2 == 1)
                result = (result * a) % m;

            a = (a * a) % m;
            b /= 2;
        }

        return result;
    }
}
