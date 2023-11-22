using System;
using System.Collections.Generic;

class SilverPohligHellman
{
    static void Main()
    {
        Console.WriteLine("Алгоритм Сильвера-Полига-Хеллмана");
        Console.WriteLine("-------------------------------");

        Console.Write("Введите основание (g): ");
        int g = int.Parse(Console.ReadLine());

        Console.Write("Введите число (y): ");
        int y = int.Parse(Console.ReadLine());

        Console.Write("Введите модуль (p): ");
        int p = int.Parse(Console.ReadLine());

        int x = SilverPohligHellmanAlgorithm(g, y, p);

        Console.WriteLine($"Дискретный логарифм x в уравнении {g}^x ≡ {y} (mod {p}) равен {x}");
    }

    static int SilverPohligHellmanAlgorithm(int g, int y, int p)
    {
        Console.WriteLine("Шаг 1: Факторизация порядка группы");
        List<int> primeFactors = FactorizeGroupOrder(p - 1);
        Console.WriteLine($"Факторизация порядка группы (p-1): {string.Join(", ", primeFactors)}");

        Console.WriteLine("Шаг 2: Решение уравнений малых порядков");
        int x = 0;

        foreach (int q in primeFactors)
        {
            Console.WriteLine($"Решение для подгруппы порядка {q}");
            int alpha = FindAlpha(g, y, p, q);
            Console.WriteLine($"alpha = {alpha}");

            int beta = ExtendedEuclideanAlgorithm(g, q);
            Console.WriteLine($"beta = {beta}");

            int xq = (alpha * beta) % q;
            Console.WriteLine($"Для подгруппы порядка {q}: x = {xq}");

            x += xq * (p - 1) / q;
        }

        return x % (p - 1);
    }

    static List<int> FactorizeGroupOrder(int n)
    {
        List<int> factors = new List<int>();

        for (int i = 2; i * i <= n; i++)
        {
            while (n % i == 0)
            {
                factors.Add(i);
                n /= i;
            }
        }

        if (n > 1)
            factors.Add(n);

        return factors;
    }

    static int FindAlpha(int g, int y, int p, int q)
    {
        int alpha = 1;

        while (ModuloExponentiation(g, alpha * (p - 1) / q, p) != 1 || ModuloExponentiation(y, alpha * (p - 1) / q, p) != 1)
        {
            alpha++;
        }

        return alpha;
    }

    static int ExtendedEuclideanAlgorithm(int a, int b)
    {
        int x0 = 1, x1 = 0, y0 = 0, y1 = 1;

        while (b != 0)
        {
            int q = a / b;
            int temp = b;
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

    static int ModuloExponentiation(int a, int b, int m)
    {
        int result = 1;

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
