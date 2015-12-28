using System;
using System.Linq;
using System.Collections.Generic;

/* Most of this works great, however the program works by setting the "forced" pathes down and using the new information to set more down.
 * Unfortunately, in some cases this is not possible, especially when multiple solutions are possible, or when no forced pathes can
 * be identified. The solution would be to set a test path, try to solve, and if in error, use the new information that there is no path
 * as set in the test where it was set to set new pathes down. This will require some tweaking, I'll get to it later. */

public class Player
{
    [Flags]
    public enum Direction
    {
        UP = 1,
        DOWN = 2,
        LEFT = 4,
        RIGHT = 8,
        VERTICAL = UP | DOWN,
        HORIZONTAL = LEFT | RIGHT
    }

    public enum LinkType
    {
        SINGLE = 1,
        DOUBLE = 2
    }

    public struct Point
    {
        public readonly int x;
        public readonly int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Point MoveTowards(Direction direction)
        {
            switch(direction)
            {
                case Direction.UP:
                    return new Point(this.x, this.y - 1);
                case Direction.DOWN:
                    return new Point(this.x, this.y + 1);
                case Direction.LEFT:
                    return new Point(this.x - 1, this.y);
                case Direction.RIGHT:
                    return new Point(this.x + 1, this.y);
            }
            return new Point(this.x, this.y);
        }

        public override string ToString()
        {
            return this.x + " " + this.y;
        }
    }

    public class Element { }

    public class Node : Element
    {
        public readonly int original;
        public int links;
        public readonly Point pos;
        public readonly List<Link> connections = new List<Link>();

        public Node(int links, Point pos)
        {
            this.original = links;
            this.links = links;
            this.pos = pos;
        }

        public static void SetLink(Node a, Node b, LinkType type, Graph graph)
        {
            int p = (int)type;
            Link li = a.connections.Find(l => l.Match(a, b));
            if (li != null)
            {
                a.connections.Remove(li);
                b.connections.Remove(li);
                if (type == LinkType.DOUBLE) { p--; }
                else { type = LinkType.DOUBLE; }
            }
            Direction d = a.pos.x == b.pos.x ? Direction.VERTICAL : Direction.HORIZONTAL;
            Link link = new Link(a, b, type, d);
            a.connections.Add(link);
            b.connections.Add(link);
            if (d == Direction.VERTICAL)
            {
                int min = Math.Min(a.pos.y, b.pos.y) + 1, max = Math.Max(a.pos.y, b.pos.y) ;
                for (int i = min; i < max; i++)
                {
                    graph[a.pos.x, i] = link;
                }
            }
            else
            {
                int min = Math.Min(a.pos.x, b.pos.x) + 1, max = Math.Max(a.pos.x, b.pos.x);
                for (int i = min; i < max; i++)
                {
                    graph[i, a.pos.y] = link;
                }
            }
            a.links -= p;
            b.links -= p;
            if (a.links == 0) { graph.nodes.Remove(a); }
            if (b.links == 0) { graph.nodes.Remove(b); }
            Console.WriteLine(a.pos.ToString() + ' ' + b.pos.ToString() + ' ' + p);
        }
    }

    public class Link : Element
    {
        public readonly Node a;
        public readonly Node b;
        public readonly Direction direction;
        public readonly LinkType type;

        public Link(Node a, Node b, LinkType type, Direction direction)
        {
            this.a = a;
            this.b = b;
            this.type = type;
            this.direction = direction;
        }

        public bool Match(Node a, Node b)
        {
            return (this.a == a && this.b == b) || (this.a == b && this.b == a);
        }
    }

    public class Graph
    {
        public readonly int width;
        public readonly int height;
        public readonly Element[,] graph;
        public readonly List<Node> nodes = new List<Node>();

        public Element this[int x, int y]
        {
            set { this.graph[y, x] = value; }
        }
        private Element this[Point pos]
        {
            get { return this.graph[pos.y, pos.x]; }
        }

        public Graph(int width, int height)
        {
            this.width = width;
            this.height = height;
            graph = new Element[height, width];
            for (int i = 0; i < height; i++)
            {
                string line = Console.ReadLine();
                for (int j = 0; j < width; j++)
                {
                    char c = line[j];
                    if (c != '.')
                    {
                        Node n = new Node((int)char.GetNumericValue(c), new Point(j, i));
                        graph[i, j] = n;
                        this.nodes.Add(n);
                    }
                }
            }
        }

        public void Solve()
        {
            int i = 0;
            while (nodes.Count > 0)
            {
                if (this.nodes.Count == 2) { Console.WriteLine(nodes[0].pos.ToString() + ' ' + nodes[1].pos.ToString() + ' ' + nodes[0].links); break; }
                if (i >= this.nodes.Count) { i = 0; }
                Node n = this.nodes[i++];
                List<Node> neighbours = GetNeighbours(n).ToList();
                if (neighbours.Count * 2 == n.links)
                {
                    foreach(Node m in neighbours)
                    {
                        Node.SetLink(n, m, LinkType.DOUBLE, this);
                    }
                }
                else if (((neighbours.Count * 2) - 1 == n.links) || (neighbours.All(m => m.links == 1) && neighbours.Count == n.links))
                {
                    foreach (Node m in neighbours)
                    {
                        Node.SetLink(n, m, LinkType.SINGLE, this);
                    }
                }
                else if (n.links == 1 && neighbours.Count == 1)
                {
                    Node.SetLink(n, neighbours[0], LinkType.SINGLE, this);
                }
                else
                {
                    List<Node> nonLinked = neighbours.Where(m => !n.connections.Any(l => l.Match(n, m))).ToList();
                    if (n.links + (neighbours.Count - nonLinked.Count) == (2 * neighbours.Count) -1)
                    {
                        foreach(Node m in nonLinked)
                        {
                            Node.SetLink(n, m, LinkType.SINGLE, this);
                        }
                    }
                    else if (nonLinked.Count == 0 && neighbours.Count == n.links)
                    {
                        foreach(Node m in neighbours)
                        {
                            Node.SetLink(n, m, LinkType.DOUBLE, this);
                        }
                    }
                }
            }
        }

        public IEnumerable<Node> GetNeighbours(Node n)
        {
            Node m;
            if (TryGetNodeIn(n, Direction.UP, out m)) { yield return m; }
            if (TryGetNodeIn(n, Direction.DOWN, out m)) { yield return m; }
            if (TryGetNodeIn(n, Direction.LEFT, out m)) { yield return m; }
            if (TryGetNodeIn(n, Direction.RIGHT, out m)) { yield return m; }
        }

        private bool TryGetNodeIn(Node initial, Direction dir, out Node n)
        {
            Point p = initial.pos.MoveTowards(dir);
            while(p.x >= 0 && p.x < this.width && p.y >= 0 && p.y < this.height)
            {
                Element e = this[p];
                if (e is Node)
                {
                    Node m = (Node)e;
                    Link link = m.connections.Find(l => l.Match(initial, m));
                    if ((link != null && link.type == LinkType.DOUBLE) || m.links == 0 || (initial.original == 1 && m.original == 1)) { break; }
                    n = m;
                    return true;
                }
                if (e is Link)
                {
                    Link l = (Link)e;
                    if (l.type == LinkType.DOUBLE || (l.direction & dir) == 0) { break; }
                }
                p = p.MoveTowards(dir);
            }
            n = null;
            return false;
        }
    }

    private static void Main()
    {
        new Graph(int.Parse(Console.ReadLine()), int.Parse(Console.ReadLine())).Solve();
    }
}