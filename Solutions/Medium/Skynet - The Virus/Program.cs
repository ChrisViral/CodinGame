using System;
using System.Linq;
using System.Collections.Generic;

public class Player
{
    public class Node
    {
        #region Fields
        public readonly int index = 0;
        public readonly List<Node> nodes = new List<Node>();
        public bool isGateway = false;
        public int pass = -1;
        #endregion

        #region Constructor
        public Node(int index)
        {
            this.index = index;
        }
        #endregion

        #region Methods
        public void AddNode(Node node)
        {
            this.nodes.Add(node);
        }

        public void RemoveNode(Node node)
        {
            this.nodes.Remove(node);
        }
        #endregion
    }

    public class TreeNode
    {
        #region Fields
        public TreeNode parent = null;
        public Node node = null;
        public List<Node> children = new List<Node>();
        #endregion

        #region Constructors
        public TreeNode(TreeNode parent, Node node, int pass)
        {
            this.parent = parent;
            this.node = node;
            this.children = new List<Node>(node.nodes.Where(n => n.pass != pass));
        }
        #endregion
    }

    private static void Main()
    {
        string[] inputs = Console.ReadLine().Split(' ');
        int N = int.Parse(inputs[0]);   //The total number of nodes in the level, including the gateways
        int L = int.Parse(inputs[1]);   //The number of links
        int E = int.Parse(inputs[2]);   //The number of exit gateways

        Node[] nodes = new Node[N];
        for (int i = 0; i < N; i++)
        {
            nodes[i] = new Node(i);
        }
        for (int i = 0; i < L; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            Node a = nodes[int.Parse(inputs[0])], b = nodes[int.Parse(inputs[1])];
            a.AddNode(b);
            b.AddNode(a);
        }
        for (int i = 0; i < E; i++)
        {
            nodes[int.Parse(Console.ReadLine())].isGateway = true;
        }
        int pass = 0;

        //Game loop
        while (true)
        {
            Node skynet = nodes[int.Parse(Console.ReadLine())];
            Stack<Node> path = new Stack<Node>(GetShortestPath(nodes, skynet, pass++));
            Node a = path.Pop(), b = path.Pop();
            a.RemoveNode(b);
            b.RemoveNode(a);
            Console.WriteLine(a.index + " " + b.index);
        }
    }

    public static IEnumerable<Node> GetShortestPath(Node[] graph, Node root, int pass)
    {
        root.pass = pass;
        Queue<TreeNode> nodes = new Queue<TreeNode>();
        nodes.Enqueue(new TreeNode(null, root, pass));

        while (nodes.Count > 0)
        {
            TreeNode t = nodes.Dequeue();
            foreach (Node node in t.children)
            {
                node.pass = pass;
                if (node.isGateway)
                {
                    return GetListFromTree(t, node);
                }
                nodes.Enqueue(new TreeNode(t, node, pass));
            }
        }
        return GetListFromTree(null, null);
    }

    public static IEnumerable<Node> GetListFromTree(TreeNode parent, Node node)
    {
        if (node == null) { yield break; }

        yield return node;
        TreeNode c = parent;
        while (c != null)
        {
            yield return c.node;
            c = c.parent;
        }
    }
}