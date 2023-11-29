using Alghorithm_Silver_Polig_Hellman;
using System.Numerics;

class SilverPohligHellman
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("Алгоритм Сильвера-Полига-Хеллмана");
        Console.WriteLine("a^x ≡ b (mod p)");
        Console.WriteLine("-------------------------------");

        Console.Write("Введите основание (a): ");
        BigInteger a = BigInteger.Parse(Console.ReadLine());

        Console.Write("Введите число (b): ");
        BigInteger b = BigInteger.Parse(Console.ReadLine());

        Console.Write("Введите модуль (p): ");
        BigInteger p = BigInteger.Parse(Console.ReadLine());

        try
        {
            BigInteger x = SilverPohligHellmanAlgorithmNew(a, b, p);
            Console.WriteLine($"Дискретный логарифм x в уравнении {a}^x ≡ {b} (mod {p}) равен {x}");
        }
        catch (Exception)
        {
            Console.WriteLine("Ошибка во входных данных!");
        }
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


        return Step3_KTO(system, p);
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
            List<BigInteger> xis = new List<BigInteger>();
            BigInteger skobochki = b;
            BigInteger skobochki_qi_stepen = 1;
            for (int j = 0; j < qi_stepen; j++)
            {
                BigInteger stepen = (p - 1) / BigInteger.Pow(qi, j + 1);


                BigInteger a_stepen_result = 0;
                for (int to_j = 0; to_j < j; to_j++)
                {
                    a_stepen_result += -(xis[to_j] * BigInteger.Pow(qi, to_j));
                }
                skobochki = b * Extension.BigIntegerModPowWithMinusOne(a, a_stepen_result, p);

                BigInteger currentX = Step2_GetXi(currentRow, skobochki, stepen, p);

                xis.Add(currentX);
            }
            BigInteger currentResult = 0;
            for (int j = 0; j < xis.Count; j++)
            {
                currentResult += xis[j] * BigInteger.Pow(qi, j);
            }
            result.Add((currentResult, BigInteger.Pow(q1q2qn[i].prime, (int)q1q2qn[i].pow)));
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

    static BigInteger Step3_KTO(List<(BigInteger b, BigInteger p)> system, BigInteger p)
    {
        //tex:
        //$${\displaystyle x=\sum _{i= 1}^{k} a_{i}M_{i} N_{i}.}$$
        //$${\displaystyle {\begin{cases}x\equiv r_{1}{\pmod {a_{1}}},\\x\equiv r_{2}{\pmod {a_{2}}},\\\cdots \cdots \cdots \cdots \cdots \cdots \\x\equiv r_{n}{\pmod {a_{n}}}.\\\end{cases}}}$$

        BigInteger M = 1;//ai=pi;
        BigInteger result = 0;

        for (int i = 0; i < system.Count; i++)
            M *= system[i].p;

        for (int i = 0; i < system.Count; i++)
        {
            BigInteger Mi = M / system[i].p;
            result += system[i].b * Extension.FindObratniiElement(Mi, system[i].p) * Mi;
        }

        return result % M;
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
