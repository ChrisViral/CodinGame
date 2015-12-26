using System;
using System.Linq;
using System.Collections.Generic;

public class Solution
{
    public class Number
    {
        #region Fields
        public readonly int value = 0;
        public readonly int xSize = 0, ySize = 0;
        private readonly string[] lines = new string[0];
        private readonly int start = 0;
        #endregion

        #region Constructor
        public Number(int xSize, int ySize, int value)
        {
            this.value = value;
            this.xSize = xSize;
            this.ySize = ySize;
            this.lines = new string[ySize];
            this.start = value * xSize;
        }
        #endregion

        #region Methods
        public void AddLine(int i, string line)
        {
            lines[i] = line.Substring(start, xSize);
        }

        public bool IsCharacter(string[] lines)
        {
            return this.lines.SequenceEqual(lines);
        }

        public override string ToString()
        {
            return String.Join("\n", lines);
        }
        #endregion
    }

    private static void Main()
    {
        string[] inputs = Console.ReadLine().Split(' ');
        int width = int.Parse(inputs[0]);
        int height = int.Parse(inputs[1]);

        List<Number> numbers = new List<Number>(20);
        string line = Console.ReadLine();
        for (int i = 0; i < 20; i++)
        {
            Number n = new Number(width, height, i);
            n.AddLine(0, line);
            numbers.Add(n);
        }
        for (int i = 1; i < height; i++)
        {
            line = Console.ReadLine();
            for (int j = 0; j < 20; j++)
            {
                numbers[j].AddLine(i, line);
            }
        }

        string[] current = new string[height];
        int s1 = int.Parse(Console.ReadLine());
        List<Number> n1 = new List<Number>(s1 / height);
        for (int i = 0; i < s1; i++)
        {
            int c = i % height;
            current[c] = Console.ReadLine();
            if (c == height - 1)
            {
                n1.Add(numbers.Find(n => n.IsCharacter(current)));
            }
        }

        int s2 = int.Parse(Console.ReadLine());
        List<Number> n2 = new List<Number>(s2 / height);
        for (int i = 0; i < s2; i++)
        {
            int c = i % height;
            current[c] = Console.ReadLine();
            if (c == height - 1)
            {
                n2.Add(numbers.Find(n => n.IsCharacter(current)));
            }
        }

        long result = ToInt64(n1), a = ToInt64(n2);
        switch (Console.ReadLine())
        {
            case "+":
                result += a; break;

            case "-":
                result -= a; break;

            case "*":
                result *= a; break;

            case "/":
                result /= a; break;
        }

        List<int> values = new List<int>();
        if (result == 0) { values.Add(0); }
        else
        {
            while (result >= 1)
            {
                long rem;
                result = Math.DivRem(result, 20L, out rem);
                values.Add((int)rem);
            }
        }

        for (int i = values.Count - 1; i >= 0; i--)
        {
            Console.WriteLine(numbers[values[i]].ToString());
        }
    }

    public static long ToInt64(List<Number> number)
    {
        long amount = 0L;
        for (int i = 0; i < number.Count; i++)
        {
            amount += number[i].value * Pow20(number.Count - i - 1L);
        }
        return amount;
    }

    public static long Pow20(long x)
    {
        switch (x)
        {
            case 0:
                return 1L;

            case 1:
                return 20L;

            case 2:
                return 400L;

            case 3:
                return 8000L;

            case 4:
                return 160000L;

            case 5:
                return 3200000L;

            case 6:
                return 64000000L;

            default:
                return (long)Math.Pow(20, x);
        }
    }
}