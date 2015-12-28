using System;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Solution class
/// </summary>
public class Solution
{
    #region Methods
    /// <summary>
    /// Entry point
    /// </summary>
    private static void Main()
    {
        int N = int.Parse(Console.ReadLine());
        //If empty
        if (N == 0) { return; }
        //If only one input
        else if (N == 1) { Console.WriteLine(Console.ReadLine().Length); }
        //First input setting
        List<string> possibilities = new List<string>() { Console.ReadLine() };
        //Reading for other inputs
        for (int i = 1; i < N; i++)
        {
            //Adding possibilities progressibely
            possibilities = GetNextPossibilities(Console.ReadLine(), possibilities).ToList();
        }

        //Return largest length
        Console.WriteLine(possibilities.Min(s => s.Length));
    }

    /// <summary>
    /// Gets all the new possibilities from a new sequence and all the existing sequences
    /// </summary>
    /// <param name="name">New sequences</param>
    /// <param name="possibilities">Existing sequences</param>
    /// <returns>New enumerable of all the possible sequences</returns>
    public static IEnumerable<string> GetNextPossibilities(string name, List<string> possibilities)
    {
        foreach(string poss in possibilities)
        {
            //Find appends/insertions of current into previous
            foreach (string s in FindPossibilities(name, poss))
            {
                yield return s;
            }
            //Find append/insertions of previous into current
            foreach (string s in FindPossibilities(poss, name))
            {
                yield return s;
            }
        }
    }

    /// <summary>
    /// This is kind of complex, but this finds all the appending and insertion indexes
    /// </summary>
    /// <param name="a">First sequence</param>
    /// <param name="b">Second sequence</param>
    /// <returns>All the possible appends and insertions for the two sequences</returns>
    public static IEnumerable<string> FindPossibilities(string a, string b)
    {
        //Return both appended
        yield return b + a;
        //If contained, return container
        if (b.Contains(a)) { yield return b; }
        List<int> indexes = new List<int>();
        char f = a[0];
        for (int i = 0; i < b.Length; i++)
        {
            //Indices in b where we could insert/append a
            if (b[i] == f) { indexes.Add(i); }
        }
        foreach (int index in indexes)
        {
            //Potentially appended/inserted string
            string s = b.Substring(0, index) + a;
            //Check for appending
            for (int i = 1; i < a.Length; i++)
            {
                //If at end of b, successfully appended
                if (index + i == b.Length)
                {
                    yield return s;
                    break;
                }
                //If characters different, failed
                else if (a[i] != b[index + i]) { break; }
            }
            //Insertion index
            int ins = index + 1;
            //Check for insertion
            if (ins != b.Length)
            {
                List<int> ind = new List<int>();
                f = b[ins];
                for (int i = 2; i < a.Length; i++)
                {
                    //All indexes in a that are the next character of b, where we could start insertion
                    if (a[i] == f) { ind.Add(i); }
                }
                foreach (int i in ind)
                {
                    for (int j = ins + 1; j < b.Length; j++)
                    {
                        //If at end of a, successfully inserted
                        if (i + j - ins == a.Length)
                        {
                            yield return s + b.Substring(j, b.Length - j);
                            break;
                        }
                        //If characters different, failed
                        else if (a[i] != b[j]) { break; }
                    }
                }
            }
        }
    }
    #endregion
}