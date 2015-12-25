using System;
using System.Collections.Generic;


public class Solution
{
    public class Node
    {
        #region Fields
        public readonly int index = 0;
        public readonly List<Node> nodes = new List<Node>();
        public int connections = 0;
        #endregion

        #region Constructor
        public Node(int index)
        {
            this.index = index;
        }
        #endregion

        #region Methods
        public void Remove()
        {
            foreach (Node node in nodes)
            {
                node.connections--;
            }
        }

        public static Node AddNew(int index, Dictionary<int, Node> nodes)
        {
            Node node = new Node(index);
            nodes.Add(index, node);
            return node;
        }

        public static void CreateLink(Node a, Node b)
        {
            a.nodes.Add(b);
            b.nodes.Add(a);
        }
        #endregion
    }

    private static void Main()
    {
        int N = int.Parse(Console.ReadLine()); //The number of adjacency relations
        Dictionary<int, Node> nodes = new Dictionary<int, Node>(N + 1);
        string[] inputs;
        for (int i = 0; i < N; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            int xi = int.Parse(inputs[0]);  //The ID of a person which is adjacent to yi
            int yi = int.Parse(inputs[1]);  //The ID of a person which is adjacent to xi
            Node a, b;
            if (!nodes.TryGetValue(xi, out a)) { a = Node.AddNew(xi, nodes); }
            if (!nodes.TryGetValue(yi, out b)) { b = Node.AddNew(yi, nodes); }
            Node.CreateLink(a, b);
        }

        List<Node> tree = new List<Node>();
        foreach(Node n in nodes.Values)
        {
            n.connections = n.nodes.Count;
            tree.Add(n); 
        }
        int count = 0;
        for (; tree.Count > 1; count++)
        {
            List<Node> newTree = new List<Node>();
            List<Node> remove = new List<Node>();
            foreach (Node n in tree)
            {
                if (n.connections == 1) { remove.Add(n); }
                else { newTree.Add(n); }
            }
            remove.ForEach(n => n.Remove());
            tree = newTree;
        }
        Console.WriteLine(count);
    }
}