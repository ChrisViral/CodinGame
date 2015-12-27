using System;
using System.Collections.Generic;

/// <summary>
/// Solution class
/// </summary>
public class Solution
{
    #region Classes
    /// <summary>
    /// Value representing a job with start and end times
    /// </summary>
    public struct Job
    {
        #region Fields
        /// <summary>
        /// Job start time
        /// </summary>
        public readonly int start;
        /// <summary>
        /// Job end time
        /// </summary>
        public readonly int end;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new Job
        /// </summary>
        /// <param name="start">The Job start time</param>
        /// <param name="length">The length of the job</param>
        public Job(int start, int length)
        {
            this.start = start;
            this.end = start + length - 1;
        }
        #endregion
    }
    #endregion

    #region Methods
    /// <summary>
    /// Entry point
    /// </summary>
    private static void Main()
    {
        int n = int.Parse(Console.ReadLine());  //Amount of jobs
        if (n == 0) { return; }
        if (n == 1) { Console.WriteLine("1"); return; }

        Job[] jobs = new Job[n];
        for (int i = 0; i < n; i++)
        {
            string[] inputs = Console.ReadLine().Split(' ');
            jobs[i] = new Job(int.Parse(inputs[0]), int.Parse(inputs[1]));
        }
        //Sort jobs by ending time
        Array.Sort(jobs, (j1, j2) => j1.end - j2.end);

        //Pretty easy, use a greedy algorithm since all jobs have the same weight
        int amount = 1;
        Job last = jobs[0];
        for (int i = 1; i < n; i++)
        {
            Job j = jobs[i];

            //If job ends after last, add to schedule
            if (j.start > last.end)
            {
                amount++;
                last = j;
            }
        }

        Console.WriteLine(amount);
    }
    #endregion
}