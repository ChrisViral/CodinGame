using System;
using System.Collections.Generic;

public class Solution
{
    private static void Main()
    {
        int N = int.Parse(Console.ReadLine());  //Number of elements which make up the association table.
        int Q = int.Parse(Console.ReadLine());  //Number Q of file names to be analyzed.
        Dictionary<string, string> types = new Dictionary<string, string>();

        for (int i = 0; i < N; i++)
        {
            string[] inputs = Console.ReadLine().Split(' ');
            types.Add(inputs[0].ToLower(), inputs[1]);
        }

        for (int i = 0; i < Q; i++)
        {
            string[] file = Console.ReadLine().Split('.'); // One file name per line.
            string mime;
            if (file.Length <= 1 || !types.TryGetValue(file[file.Length - 1].ToLower(), out mime)) { Console.WriteLine("UNKNOWN"); }
            else { Console.WriteLine(mime); }
        }
    }
}