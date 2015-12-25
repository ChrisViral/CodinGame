using System;
using System.Text;

public class Solution
{
    private static void Main()
    {
        Console.WriteLine(ToUnary(Console.ReadLine()));
    }

    public static string ToUnary(string message)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(message);
        string binary = string.Empty;
        for (int i = 0; i < bytes.Length; i++)
        {
            string bin = Convert.ToString(bytes[i], 2);
            if (bin.Length < 7) { bin = RepeatZero(7 - bin.Length) + bin; }
            binary += bin;
        }
        string result = string.Empty;
        for (int i = 0; i < binary.Length;)
        {
            char c = binary[i++];
            result += c == '1' ? "0 " : "00 ";
            int amount = 1;
            while (i < binary.Length && binary[i] == c)
            {
                i++;
                amount++;
            }
            result += RepeatZero(amount) + ' ';
        }
        return result.TrimEnd();
    }

    public static string RepeatZero(int amount)
    {
        if (amount <= 0) { return string.Empty; }

        string zeros = "0";
        for (int i = 1; i < amount; i++)
        {
            zeros += '0';
        }
        return zeros;
    }
}