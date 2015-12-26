using System;
using System.Collections.Generic;

public enum Direction
{
    SOUTH,
    EAST,
    NORTH,
    WEST
}

public class Solution
{
    public struct BenderState
    {
        #region Fields
        public readonly Direction direction;
        public readonly bool inverted;
        public readonly bool breaker;
        #endregion

        #region Constructor
        public BenderState(Direction direction, bool inverted, bool breaker)
        {
            this.direction = direction;
            this.inverted = inverted;
            this.breaker = breaker;
        }
        #endregion
    }

    public struct Point : IEquatable<Point>
    {
        #region Fields
        public static readonly Point zero = new Point(0, 0);
        public readonly int x;
        public readonly int y;
        #endregion

        #region Constructor
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        #endregion

        #region Methods
        public Point MoveTowards(Direction direction)
        {
            switch(direction)
            {
                case Direction.SOUTH:
                    return new Point(this.x, this.y + 1);
                case Direction.EAST:
                    return new Point(this.x + 1, this.y);
                case Direction.NORTH:
                    return new Point(this.x, this.y - 1);
                case Direction.WEST:
                    return new Point(this.x - 1, this.y);
            }

            return this;
        }

        public override bool Equals(object obj)
        {
            if (obj is Point)
            {
                return Equals((Point)obj);
            }
            return false;
        }

        public bool Equals(Point point)
        {
            return this.x.Equals(point.x) && this.y.Equals(point.y);
        }

        public override int GetHashCode()
        {
            return (this.x << 5) ^ this.y + this.x;
        }
        #endregion

        #region Operators
        public static bool operator ==(Point p1, Point p2)
        {
            return p1.x == p2.x && p1.y == p2.y;
        }

        public static bool operator !=(Point p1, Point p2)
        {
            return p1.x != p2.x || p1.y != p2.y;
        }
        #endregion
    }

    public class Grid
    {
        #region Fields
        public readonly int height;
        public readonly int width;
        private readonly char[,] grid;
        private Point t1, t2;
        #endregion

        #region Indexers
        public char this[int x, int y]
        {
            get { return this.grid[y, x]; }
            set { this.grid[y, x] = value; }
        }
        public char this[Point point]
        {
            get { return this.grid[point.y, point.x]; }
            set { this.grid[point.y, point.x] = value; }
        }
        #endregion

        #region Constructors
        public Grid(int height, int width)
        {
            this.height = height;
            this.width = width;
            this.grid = new char[height, width];
        }
        #endregion

        #region Methods
        public Point Teleport(Point position)
        {
            if (position == t1) { return t2; }
            if (position == t2) { return t1; }
            return position;
        }

        public void SetTeleporters(Point t1, Point t2)
        {
            this.t1 = t1;
            this.t2 = t2;
        }
        #endregion
    }

    public static void Main()
    {
        string[] inputs = Console.ReadLine().Split(' ');
        int height = int.Parse(inputs[0]);
        int width = int.Parse(inputs[1]);
        Grid grid = new Grid(height, width);
        List<Point> teleporters = new List<Point>(2);
        Point startPos = Point.zero;
        for (int i = 0; i < height; i++)
        {
            string row = Console.ReadLine();
            for (int j = 0; j < width; j++)
            {
                char c = row[j];
                grid[j, i] = c;
                if (c == '@') { startPos = new Point(j, i); }
                else if (c == 'T') { teleporters.Add(new Point(j, i)); }
            }
        }
        if (teleporters.Count == 2) { grid.SetTeleporters(teleporters[0], teleporters[1]); }

        Console.WriteLine(String.Join("\n", Simulate(grid, startPos)));
    }

    public static List<string> Simulate(Grid grid, Point startPos)
    {
        Direction[] directions = DirectionsUtil.normal;
        List<string> output = new List<string>();
        Point pos = startPos;
        Direction direction = Direction.SOUTH;
        bool inverted = false, breaker = false;
        Dictionary<Point, Dictionary<BenderState, int>> path = new Dictionary<Point, Dictionary<BenderState, int>>()
        {
            {
                startPos,
                new Dictionary<BenderState, int>() { { new BenderState(Direction.SOUTH, false, false), 1 } }
            }
        };

        while (grid[pos] != '$')
        {
            Point to = pos.MoveTowards(direction);
            char c = grid[to];
            int i = 0;
            //Find next possible direction
            while (c == '#' || (c == 'X' && !breaker))
            {
                direction = directions[i++];
                to = pos.MoveTowards(direction);
                c = grid[to];
            }
            //If in breaker mode, replace wall
            if (c == 'X' && breaker) { grid[to] = ' '; }
            
            output.Add(DirectionsUtil.GetName(direction));
            pos = to;
            switch (c)
            {
                case 'S':
                case 'E':
                case 'N':
                case 'W':
                    direction = DirectionsUtil.GetValue(c); break;

                case 'I':
                    {
                        inverted = !inverted;
                        directions = directions.Reverse();
                        break;
                    }

                case 'B':
                    breaker = !breaker; break;  //Hah... Break.

                case 'T':
                    pos = grid.Teleport(pos); break;
            }

            //Add state to the dictionary
            BenderState state = new BenderState(direction, inverted, breaker);
            Dictionary<BenderState, int> states;
            if (path.TryGetValue(pos, out states))
            {
                int passes;
                if (states.TryGetValue(state, out passes))
                {
                    //If passed more than ten times at the same place in the same settings, consider it a loop
                    if (passes >= 10)
                    {
                        return new List<string>(1) { "LOOP" };
                    }
                    states[state]++;
                }
                else { states.Add(state, 1); }
            }
            else
            {
                path.Add(pos, new Dictionary<BenderState, int>() { { state, 1 } });
            }
            
        }
        
        return output;
    }
}

public static class DirectionsUtil
{
    #region Fields
    public static readonly Direction[] normal;
    private static readonly Direction[] reversed;
    private static readonly Dictionary<char, Direction> toValue;
    private static readonly Dictionary<Direction, string> toString;
    #endregion

    #region Constructor
    static DirectionsUtil()
    {
        normal = new Direction[4] { Direction.SOUTH, Direction.EAST, Direction.NORTH, Direction.WEST };
        reversed = new Direction[4] { Direction.WEST, Direction.NORTH, Direction.EAST, Direction.SOUTH };
        toValue = new Dictionary<char, Direction>(4)
        {
            { 'S', Direction.SOUTH },
            { 'E', Direction.EAST  },
            { 'N', Direction.NORTH },
            { 'W', Direction.WEST  }
        };
        toString = new Dictionary<Direction, string>(4)
        {
            { Direction.SOUTH, "SOUTH" },
            { Direction.EAST,  "EAST"  },
            { Direction.NORTH, "NORTH" },
            { Direction.WEST,  "WEST"  }
        };
    }
    #endregion

    #region Methods
    /* The following methods are careless because I know they'll
     * only bew used in this setting and won't cause exceptions. */

    public static Direction[] Reverse(this Direction[] directions)
    {
        switch(directions[0])
        {
            case Direction.SOUTH:
                return reversed;

            default:
                return normal;
        }
    }

    public static Direction GetValue(char character)
    {
        return toValue[character];
    }

    public static string GetName(Direction value)
    {
        return toString[value];
    }
    #endregion
}