using System;

public class Solution
{
    private static void Main()
    {
        string line = Console.ReadLine();
        int L = int.Parse(Console.ReadLine());

        if (L > 1)
        {
            for (int i = 1; i < L; i++)
            {
                line = GetNextLine(line);
            }
        }

        Console.WriteLine(line);
    }

    public static string GetNextLine(string line)
    {
        string[] nums = line.Split(' ');
        if (nums.Length == 1) { return "1 " + nums[0]; }

        string result = string.Empty;
        string prev = nums[0];
        int amount = 1, i = 1;
        while(true)
        {
            string c = nums[i++];
            if (c == prev)
            {
                amount++;
                if (i == nums.Length)
                {
                    result += amount + " " + prev;
                    break;
                }
            }
            else
            {
                result += amount + " " + prev + " ";
                amount = 1;
                prev = c;

                if (i == nums.Length)
                {
                    result += "1 " + c;
                    break;
                }
            }
        }

        return result;
    }
}