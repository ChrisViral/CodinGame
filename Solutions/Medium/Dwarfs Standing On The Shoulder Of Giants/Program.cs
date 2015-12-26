using System;
using System.Linq;
using System.Collections.Generic;

public class Solution
{
    public class Node
    {
        #region Fields
        public readonly int id = 0;
        public readonly List<Node> children = new List<Node>();
        #endregion

        #region Constructor
        public Node(int id)
        {
            this.id = id;
        }
        #endregion

        #region Methods
        public void AddChild(Node child)
        {
            this.children.Add(child);
        }

        public static Node AddNew(int id, List<Node> nodes)
        {
            Node node = new Node(id);
            nodes.Add(node);
            return node;
        }
        #endregion
    }

    private static void Main()
    {
        int N = int.Parse(Console.ReadLine());  //The number of relationships of influence
        List<Node> nodes = new List<Node>(N + 1);

        for (int i = 0; i < N; i++)
        {
            string[] inputs = Console.ReadLine().Split(' ');
            int x = int.Parse(inputs[0]);
            int y = int.Parse(inputs[1]);
            (nodes.Find(n => n.id == x) ?? Node.AddNew(x, nodes)).AddChild(nodes.Find(n => n.id == y) ?? Node.AddNew(y, nodes));
        }
        

        Console.WriteLine(nodes.Max(n => FindDeepest(n)));
    }

    public static int FindDeepest(Node node)
    {
        int max = 0;
        foreach(Node n in node.children)
        {
            int c = FindDeepest(n);
            if (c > max) { max = c; }
        }
        return max + 1;
    }
}