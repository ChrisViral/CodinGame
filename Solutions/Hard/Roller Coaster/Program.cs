using System;
using System.Collections.Generic;

/// <summary>
/// Solution class
/// </summary>
public class Solution
{
    #region Classes
    /// <summary>
    /// Wrapper struct for groups
    /// </summary>
    public struct Group : IEquatable<Group>
    {
        #region Fields
        /// <summary>
        /// ID of the group
        /// </summary>
        public readonly int id;
        /// <summary>
        /// Members in the group
        /// </summary>
        public readonly int members;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new group of ther given ID and amount of members
        /// </summary>
        /// <param name="id">ID of the group</param>
        /// <param name="members">Members in the group</param>
        public Group(int id, int members)
        {
            this.id = id;
            this.members = members;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Compares the given object to this one
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns>If the objects are equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is Group)
            {
                return Equals((Group)obj);
            }
            return false;
        }

        /// <summary>
        /// Compares the given Group to this one
        /// </summary>
        /// <param name="group">Group to compare to</param>
        /// <returns>If the two Groups are equal</returns>
        public bool Equals(Group group)
        {
            return this.id.Equals(group.id);
        }

        /// <summary>
        /// Gets the Hashcode of this Group
        /// </summary>
        /// <returns>Hashcode of the group</returns>
        public override int GetHashCode()
        {
            //Groups are identified by ID and the hashcode of an int is itself anyway
            return this.id;
        }
        #endregion

        #region Operators
        /// <summary>
        /// Identifies if the two groups are equal
        /// </summary>
        /// <param name="a">Left operand</param>
        /// <param name="b">Right operand</param>
        /// <returns>If the groups are equal</returns>
        public static bool operator ==(Group a, Group b)
        {
            return a.id == b.id;
        }

        /// <summary>
        /// Identifies if the two groups are inequal
        /// </summary>
        /// <param name="a">Left operand</param>
        /// <param name="b">Right operand</param>
        /// <returns>If the groups are inequal</returns>
        public static bool operator !=(Group a, Group b)
        {
            return a.id != b.id;
        }
        #endregion
    }

    /// <summary>
    /// Info on the target index of a group and the profit made
    /// </summary>
    public struct QueueInfo
    {
        #region Fields
        /// <summary>
        /// Next index in the queue
        /// </summary>
        public readonly int next;
        /// <summary>
        /// Profit made by this group
        /// </summary>
        public readonly int profit;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new QueueInfo 
        /// </summary>
        /// <param name="next">Target index</param>
        /// <param name="profit">Profit made</param>
        public QueueInfo(int next, int profit)
        {
            this.next = next;
            this.profit = profit;
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
        string[] inputs = Console.ReadLine().Split(' ');
        int size = int.Parse(inputs[0]);    //Size of the attraction
        int count = int.Parse(inputs[1]);   //Amount of rides in a day
        int n = int.Parse(inputs[2]);       //Number of groups in queue
        Group[] queue = new Group[n];
        //Gets all groups
        for (int i = 0; i < n; i++)
        {
            queue[i] = new Group(i, int.Parse(Console.ReadLine()));
        }
        //Group->info dictionary
        Dictionary<Group, QueueInfo> groupInfo = new Dictionary<Group, QueueInfo>(n);
        long profits = 0L;  //Total profits
        int index = 0;      //Queue index
        for (int rides = 0; rides < count; rides++)
        {
            Group group = queue[index];
            QueueInfo info;
            //If group in memeory
            if (groupInfo.TryGetValue(group, out info))
            {
                index = info.next;
                profits += info.profit;
            }
            else
            {
                int total = group.members;
                int j = 0;
                Group g;
                //Identify group info
                for (int i = index + 1; i < index + n; i++)
                {
                    j = i % n;
                    g = queue[j];
                    if (total + g.members > size) { break; }
                    total += g.members;
                }

                index = j;
                profits += total;
                //Add info to dictionary
                groupInfo.Add(group, new QueueInfo(j, total));
            }
        }

        Console.WriteLine(profits);
    }
    #endregion
}