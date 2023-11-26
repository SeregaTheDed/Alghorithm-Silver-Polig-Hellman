using Alghorithm_Silver_Polig_Hellman;
using System;
using System.Collections.Generic;
using System.Numerics;

class SilverPohligHellman
{
    static void Main()
    {
        Console.WriteLine(BigInteger.ModPow(28, 18, 37));
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
        List<List<BigInteger>> tableR = Step1_BuildTableOfR(a, p, q1q2qn);
        Console.WriteLine("R Table:");
        tableR.Print();
        Console.WriteLine("System:");
        var system = Step2(q1q2qn, tableR, a, b, p);
        foreach (var item in system)
        {
            Console.WriteLine($"x ≡ {item.b} (mod {item.p})");
        }


        return BigInteger.MinusOne;
    }

    static List<(BigInteger b, BigInteger p)> Step2(
        List<(BigInteger prime, BigInteger pow)> q1q2qn, 
        List<List<BigInteger>> tableR, BigInteger a, BigInteger b, BigInteger p)
    {
        //tex:
        //$$x\equiv\log_a{b}\equiv x_0+x_1q_i+...+x_{\alpha_{i}-1}q_i^{\alpha_{i}-1}\pmod{q_i^{\alpha_i}},$$
        //$$a^{x_{j}\cdot\frac{p-1}{q_i}} \equiv (ba^{-x_0-x_1q_i...-x_{j-1}q_i^{j-1}})^{\frac{p-1}{q_i^{j+1}}} \pmod{p}$$
        int k = tableR.Count;
        List<(BigInteger b, BigInteger p)> result = new();
        for (int i = 0; i < k; i++)
        {
            var currentRow = tableR[i];
            var qi = q1q2qn[i].prime;
            var qi_stepen = q1q2qn[i].pow;
            BigInteger xi = 0;
            BigInteger skobochki = b;
            for (int j = 0; j < qi_stepen; j++)
            {
                BigInteger stepen = (p - 1) / BigInteger.Pow(qi, j+1);
                BigInteger currentX = Step2_GetXi(currentRow, skobochki, stepen, p);
                if (j != qi_stepen - 1)
                    skobochki = skobochki * Extension.BigIntegerModPowWithMinusOne(a, -currentX, p);
                //??? Нужно ли каждый раз -currentX или только в первый раз (потом без -)
                xi += currentX * BigInteger.ModPow(qi, j, p);
            }
            result.Add((xi, BigInteger.ModPow(q1q2qn[i].prime, q1q2qn[i].pow, 99999999)));//??? Магическое число...
        }
        return result;
    }
    static BigInteger Step2_GetXi(List<BigInteger> tableRRow, 
        BigInteger skobochki, BigInteger stepen, BigInteger p)
    {
        var temp = BigInteger.ModPow(skobochki, stepen, p);
        for (int i = 0; i < tableRRow.Count;i++)
        {
            if (tableRRow[i] == temp)
                return i;
        }
        throw new ArgumentException($"Не найдено решение в R таблице!");
    }

    static List<List<BigInteger>> Step1_BuildTableOfR(BigInteger a, BigInteger p, List<(BigInteger prime, BigInteger pow)> q1q2qn)
    {
        //tex:
        //$$r_{i,j}=a^{j\cdot\frac{p-1}{q_i}}, i\in\{1,\dots,k\}, j\in\{0,\dots,q_i-1\}.$$
        List<List<BigInteger>> table = new List<List<BigInteger>>();
        foreach (var elem in q1q2qn)
        {
            var prime = elem.prime; 
            var pow = elem.pow;
            List<BigInteger> row = new List<BigInteger>();
            for (BigInteger j = 0; j <= prime - 1; j++)
            {
                BigInteger cell = BigInteger.ModPow(a, j * (p - 1) / prime, p);
                row.Add(cell);
            }
            table.Add(row);
        }
        return table;
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
}
