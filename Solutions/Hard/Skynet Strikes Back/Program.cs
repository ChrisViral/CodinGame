using System;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Solution class
/// </summary>
public class Player
{
    #region Classes
    /// <summary>
    /// Represents a Graph node
    /// </summary>
    public class Node
    {
        #region Fields
        /// <summary>
        /// ID of the node
        /// </summary>
        public readonly int id = 0;
        /// <summary>
        /// Connected nodes
        /// </summary>
        public readonly List<Node> nodes = new List<Node>();
        /// <summary>
        /// If the node is a gateway
        /// </summary>
        public bool isGateway = false;
        /// <summary>
        /// The search pass identifier
        /// </summary>
        public int pass = 0;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new node of the given id
        /// </summary>
        /// <param name="id">Id of the new node</param>
        public Node(int id)
        {
            this.id = id;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds a linked node to this node
        /// </summary>
        /// <param name="node">Node to link</param>
        private void AddNode(Node node)
        {
            this.nodes.Add(node);
        }

        /// <summary>
        /// Removes a linked node to this node
        /// </summary>
        /// <param name="node">Node to remove</param>
        private void RemoveNode(Node node)
        {
            this.nodes.Remove(node);
        }

        /// <summary>
        /// Creates a link between two nodes
        /// </summary>
        /// <param name="a">First node to link</param>
        /// <param name="b">Second node to link</param>
        public static void CreateLink(Node a, Node b)
        {
            a.AddNode(b);
            b.AddNode(a);
        }

        /// <summary>
        /// Cuts a link between two nodes and outputs the cutted link
        /// </summary>
        /// <param name="a">First node to cut</param>
        /// <param name="b">Second node to cut</param>
        public static void CutLink(Node a, Node b)
        {
            a.RemoveNode(b);
            b.RemoveNode(a);
            Console.WriteLine(a.id + " " + b.id);
        }
        #endregion
    }

    /// <summary>
    /// A BFS search tree node
    /// </summary>
    public class TreeNode
    {
        #region Fields
        /// <summary>
        /// Parent tree node
        /// </summary>
        public readonly TreeNode parent;
        /// <summary>
        /// This tree node reference node
        /// </summary>
        public readonly Node node;
        /// <summary>
        /// The list of child nodes of this tree node
        /// </summary>
        public readonly List<Node> children;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new TreeNode from the given node and parent
        /// </summary>
        /// <param name="parent">Parent TreeNode</param>
        /// <param name="node">Node of this TreeNode</param>
        /// <param name="pass">Search pass identifier</param>
        public TreeNode(TreeNode parent, Node node, int pass)
        {
            this.parent = parent;
            this.node = node;
            //Child nodes are nodes that have not been passed yet and that are not gateways
            this.children = new List<Node>(node.nodes.Where(n => n.pass != pass && !node.isGateway));
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
        int N = int.Parse(inputs[0]);   //The total number of nodes in the level, including the gateways
        int L = int.Parse(inputs[1]);   //The number of links
        int E = int.Parse(inputs[2]);   //The number of exit gateways

        Node[] nodes = new Node[N], gateways = new Node[E];
        //Gets all nodes
        for (int i = 0; i < N; i++)
        {
            nodes[i] = new Node(i);
        }
        //Creates links between nodes
        for (int i = 0; i < L; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            Node.CreateLink(nodes[int.Parse(inputs[0])], nodes[int.Parse(inputs[1])]);
        }
        //Gets all gateway nodes
        for (int i = 0; i < E; i++)
        {
            Node g = nodes[int.Parse(Console.ReadLine())];
            g.isGateway = true;
            gateways[i] = g;
        }
        //Gets all nodes that are linked to more than one gateway
        List<Node> multi = nodes.Where(n => n.nodes.Count(m => m.isGateway) > 1).ToList();
        int pass = 1;

        //Game loop
        while (true)
        {
            Node skynet = nodes[int.Parse(Console.ReadLine())];
            //Checks neighbouring nodes
            if (AdjacentGateway(skynet)) { continue; }

            Node a, b;
            if (multi.Count > 0)
            {
                //Gets the optimal path to cut, or the path to the closest node that is connected to more than a gateway
                List<Node> path = GetAllPathes(skynet, n => multi.Contains(n), pass++).FirstOfOrFirst(p => PathConnected(p));
                a = path[0];
                b = a.nodes.First(n => n.isGateway);
                if (a.nodes.Count(n => n.isGateway) == 2) { multi.Remove(a); }
            }
            else
            {
                //Gets the shortest path to a gateway
                List<Node> path = GetShortestPath(skynet, pass++);
                a = path[0];
                b = path[1];
            }
            //Cuts appropriate link
            Node.CutLink(a, b);
        }
    }

    /// <summary>
    /// If any neighbouring nodes are a gateway, cuts the link
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static bool AdjacentGateway(Node node)
    {
        foreach (Node n in node.nodes)
        {
            if (n.isGateway)
            {
                Node.CutLink(node, n);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// If all the nodes except the last in a path are connected to a gateway
    /// </summary>
    /// <param name="path">Path to analyze</param>
    /// <returns>If all the nodes except last of a path is attached to a gateway</returns>
    public static bool PathConnected(List<Node> path)
    {
        for (int i = 0; i < path.Count - 1; i++)
        {
            if (!path[i].nodes.Any(n => n.isGateway)) { return false; }
        }
        return true;
    }

    /// <summary>
    /// Gets the shortest path from a root node to a gateway
    /// </summary>
    /// <param name="root">Root node of the graph</param>
    /// <param name="pass">Search pass identifier</param>
    /// <returns>The path to the closest gateway</returns>
    public static List<Node> GetShortestPath(Node root, int pass)
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
                TreeNode tn = new TreeNode(t, node, pass);
                if (node.isGateway)
                {
                    return GetPath(tn);
                }
                nodes.Enqueue(tn);
            }
        }
        return new List<Node>();
    }

    /// <summary>
    /// Gets all the path to the nodes that meet a certain condition
    /// </summary>
    /// <param name="root">Root of the graph</param>
    /// <param name="predicate">Conedition of the target nodes</param>
    /// <param name="pass">Search pass identifier</param>
    /// <returns>All pathes to the target nodes</returns>
    public static IEnumerable<List<Node>> GetAllPathes(Node root, Predicate<Node> predicate, int pass)
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
                TreeNode tn = new TreeNode(t, node, pass);
                if (predicate(node))
                {
                    yield return GetPath(tn);
                }
                nodes.Enqueue(tn);
            }
        }
    }

    /// <summary>
    /// Returns the path from a given tree node
    /// </summary>
    /// <param name="node">TreeNode to get the path from</param>
    /// <returns>Path from the tree node to the root of the graph</returns>
    public static List<Node> GetPath(TreeNode node)
    {
        List<Node> path = new List<Node>() { node.node };
        TreeNode current = node.parent;
        while (current != null)
        {
            path.Add(current.node);
            current = current.parent;
        }
        return path;
    }
    #endregion
}

/// <summary>
/// Some LINQ extensions. How ironic.
/// </summary>
public static class LINQExtensions
{
    #region Methods
    /// <summary>
    /// Returns the first value in a sequence that matches the predicate, or if none do, the first value of the sequence
    /// </summary>
    /// <typeparam name="T">Type of the sequence</typeparam>
    /// <param name="sequence">Sequence to find the matching value in</param>
    /// <param name="predicate">Predicate to match to</param>
    /// <returns>The first match or the first value</returns>
    public static T FirstOfOrFirst<T>(this IEnumerable<T> sequence, Predicate<T> predicate)
    {
        using (IEnumerator<T> e = sequence.GetEnumerator())
        {
            if (!e.MoveNext())
            {
                throw new InvalidOperationException("Sequence cannot be empty");
            }
            T first = e.Current;
            if (predicate(first)) { return first; }
            while(e.MoveNext())
            {
                T c = e.Current;
                if (predicate(c)) { return c; }
            }
            Console.Error.WriteLine("Returned first");
            return first;
        }
    }
    #endregion
}