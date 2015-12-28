using System;
using System.Collections.Generic;

/* This algorithm explores the map by approaching the closest unknown reachable region of the maze. Once it finds the control room, it tries to trace
 * a route back to the teleporter. If it can do so within the limited amount of time before the alarm rings, it reaches for the control then follows
 * the fastest path back to the teleporter. If it cannot reach the teleporter in time, it keeps searching the maze until a route short enough can
 * be found. It then proceeds to reach the control and then go back to the teleporter. */

/// <summary>
/// Possible values of the grid elements of the maze
/// </summary>
[Flags]
public enum MapValue
{
    UNKOWN = 0,
    EMPTY = 1,
    WALL = 2,
    TELEPORTER = 4,
    CONTROL = 8,
    HOLLOW = EMPTY | TELEPORTER    //Indicates Kirk can pass through theses
}

/// <summary>
/// One of the four directions Kirk can move to
/// </summary>
public enum Direction
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}

public class Player
{
    #region Classes
    /// <summary>
    /// Represents a 2D coordinate
    /// </summary>
    public struct Point
    {
        #region Fields
        /// <summary>
        /// X coordinate
        /// </summary>
        public readonly int x;
        /// <summary>
        /// Y coordinate
        /// </summary>
        public readonly int y;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new point at the given coordinates
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns the position of the point moved in the given direction
        /// </summary>
        /// <param name="direction">Direction to move into</param>
        /// <returns>The new point moved in the given direction</returns>
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
        #endregion
    }

    /// <summary>
    /// Represents the maze Kirk is in
    /// </summary>
    public class Map
    {
        #region Classes
        /// <summary>
        /// Represents a position in the map
        /// </summary>
        private class MapItem
        {
            #region Fields
            /// <summary>
            /// Search pass index (visited flag)
            /// </summary>
            public int pass;
            /// <summary>
            /// Type of object this map item is
            /// </summary>
            public MapValue value;
            #endregion

            #region Constructors
            /// <summary>
            /// Creates a new empty MapItem
            /// </summary>
            public MapItem()
            {
                this.pass = 0;
                this.value = MapValue.UNKOWN;
            }
            #endregion

            #region Methods
            /// <summary>
            /// Identify if this MapItem can be in the current path to the specified target and takes the appropriate actions
            /// </summary>
            /// <param name="p">MapPath the path is coming from</param>
            /// <param name="pos">Position of the MapItem</param>
            /// <param name="dir">Direction the path is coming from</param>
            /// <param name="target">MapValue to find</param>
            /// <param name="pass">Current search pass index</param>
            /// <param name="queue">Current search queue</param>
            /// <param name="path">Result path if target found</param>
            /// <returns>If the search found the target</returns>
            public bool SetPath(MapPath p, Point pos, Direction dir, MapValue target, int pass, Queue<MapPath> queue, ref Stack<Direction> path)
            {
                if (this.pass != pass)
                {
                    //Set search pass index
                    this.pass = pass;
                    //If target return path
                    if (this.value == target)
                    {
                        path = GetPath(new MapPath(p, pos, dir));
                        return true;
                    }
                    //If hollow add to search queue
                    else if ((this.value & MapValue.HOLLOW) != 0)
                    {
                        queue.Enqueue(new MapPath(p, pos, dir));
                    }
                }
                //Path not found
                return false;
            }

            /// <summary>
            /// Gets the successive directions to get from the target MapPath to the origin of the search
            /// </summary>
            /// <param name="target">Target position</param>
            /// <returns>Successive directions to get to the target</returns>
            private Stack<Direction> GetPath(MapPath target)
            {
                Stack<Direction> directions = new Stack<Direction>();
                MapPath p = target;
                //While the parent isn't null (at origin), add the direction
                while (p.parent != null)
                {
                    directions.Push(p.dir);
                    p = p.parent;
                }
                return directions;
            }
            #endregion
        }

        /// <summary>
        /// Represents a path node in the maze
        /// </summary>
        private class MapPath
        {
            #region Fields
            /// <summary>
            /// Parent path node
            /// </summary>
            public readonly MapPath parent;
            /// <summary>
            /// Position of the node
            /// </summary>
            public readonly Point pos;
            /// <summary>
            /// Direction coming from onto the node
            /// </summary>
            public readonly Direction dir;
            #endregion

            #region Constructor
            /// <summary>
            /// Creates a new MapPath from the given components
            /// </summary>
            /// <param name="parent">Parent node</param>
            /// <param name="pos">Position of the node</param>
            /// <param name="dir">Direction coming from onto the node</param>
            public MapPath(MapPath parent, Point pos, Direction dir)
            {
                this.parent = parent;
                this.pos = pos;
                this.dir = dir;
            }
            #endregion
        }
        #endregion

        #region Fields
        /// <summary>
        /// Width of the map
        /// </summary>
        public readonly int width;
        /// <summary>
        /// Height of the map
        /// </summary>
        public readonly int height;
        /// <summary>
        /// Internal memory array of the map
        /// </summary>
        private readonly MapItem[,] map;
        #endregion

        #region Search fields
        /// <summary>
        /// If the Teleport has been found
        /// </summary>
        private bool teleportFound = false;
        /// <summary>
        /// Position of the Teleport
        /// </summary>
        private Point teleportPos;
        /// <summary>
        /// Position of the Control
        /// </summary>
        private Point controlPos;
        /// <summary>
        /// Current search pass index
        /// </summary>
        private int pass = 1;
        /// <summary>
        /// Current optimal path from the teleporter
        /// </summary>
        private Stack<Direction> toTeleport;
        #endregion

        #region Properties
        private bool controlFound = false;
        /// <summary>
        /// If the Control has been found
        /// </summary>
        public bool ControlFound
        {
            get { return this.controlFound; }
        }

        private bool pathCorrect = false;
        /// <summary>
        /// If the path from the control to the teleporter exists
        /// </summary>
        public bool PathCorrect
        {
            get { return this.pathCorrect; }
        }

        private bool controlReached = false;
        /// <summary>
        /// If the control room has been reached
        /// </summary>
        public bool ControlReached
        {
            get { return this.controlReached; }
            set { this.controlReached = value; }
        }

        /// <summary>
        /// The length of the path from the Control to the Teleporter
        /// </summary>
        public int PathLength
        {
            get { return this.toTeleport.Count; }
        }
        #endregion

        #region Indexers
        /// <summary>
        /// Gets the MApValue at the given position in the maze
        /// </summary>
        /// <param name="pos">2D position in the maze</param>
        /// <returns>The MapValue at the given position</returns>
        public MapValue this[Point pos]
        {
            get { return this.map[pos.y, pos.x].value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new empty map of the given width and height
        /// </summary>
        /// <param name="width">Width of the map</param>
        /// <param name="height">Height of the map</param>
        public Map(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.map = new MapItem[height, width];
            //Initiates the values in the map
            for (int i = 0; i < this.height; i++)
            {
                for (int j = 0; j < this.width; j++)
                {
                    this.map[i, j] = new MapItem();
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Updates the map with the provided input
        /// </summary>
        public void UpdateMap()
        {
            for (int i = 0; i < this.height; i++)
            {
                string row = Console.ReadLine();
                for (int j = 0; j < this.width; j++)
                {
                    //Sets values only for unknown cases
                    if (this.map[i, j].value == MapValue.UNKOWN)
                    {
                        MapValue m = EnumUtils.GetItem(row[j]);
                        this.map[i, j].value = m;
                        if (!controlFound && m == MapValue.CONTROL)
                        {
                            controlPos = new Point(j, i);
                            controlFound = true;
                            SetTeleportPath();
                        }
                        else if (!teleportFound && m == MapValue.TELEPORTER)
                        {
                            teleportPos = new Point(j, i);
                            teleportFound = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tries to find the direction to the closest reachable unknown case in the map
        /// </summary>
        /// <param name="pos">Starting position</param>
        /// <param name="direction">Variable to store the resulting direction into</param>
        /// <returns>If a reachable unknown space in the map could be found</returns>
        public bool ToClosestUnknown(Point pos, ref Direction direction)
        {
            Stack<Direction> dirs;
            if (GetPathTo(pos, MapValue.UNKOWN, out dirs))
            {
                //Return first value
                direction = dirs.Peek();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the direction to the fastest route to the Control
        /// </summary>
        /// <param name="pos">Starting position</param>
        /// <returns>Direction to the Control</returns>
        public Direction ToControl(Point pos)
        {
            Stack<Direction> path;
            GetPathTo(pos, MapValue.CONTROL, out path);
            return path.Peek();
        }

        /// <summary>
        /// Searches the map and tries to set the shortest path from the Control to the Teleporter, if possible
        /// </summary>
        public void SetTeleportPath()
        {
            this.pathCorrect = GetPathTo(this.controlPos, MapValue.TELEPORTER, out this.toTeleport);
        }

        /// <summary>
        /// Gets the shortest path from a starting position to a target map element
        /// </summary>
        /// <param name="start">Starting position</param>
        /// <param name="target">Target element</param>
        /// <param name="path">Variable to store the resulting path into</param>
        /// <returns>If a path could be found in the map to the target from the starting position</returns>
        private bool GetPathTo(Point start, MapValue target, out Stack<Direction> path)
        {
            //Increment search pass index
            this.pass++;
            this.map[start.y, start.x].pass = this.pass;
            Queue<MapPath> queue = new Queue<MapPath>();
            queue.Enqueue(new MapPath(null, start, Direction.UP));
            path = new Stack<Direction>();
            while(queue.Count > 0)
            {
                //While there are possible directions
                MapPath p = queue.Dequeue();
                //Test all four directions
                Point pos = p.pos.MoveTowards(Direction.UP);
                if (pos.y >= 0 && this.map[pos.y, pos.x].SetPath(p, pos, Direction.UP, target, this.pass, queue, ref path))
                {
                    return true;
                }
                pos = p.pos.MoveTowards(Direction.DOWN);
                if (pos.y < this.height && this.map[pos.y, pos.x].SetPath(p, pos, Direction.DOWN, target, this.pass, queue, ref path))
                {
                    return true;
                }
                pos = p.pos.MoveTowards(Direction.LEFT);
                if (pos.x >= 0 && this.map[pos.y, pos.x].SetPath(p, pos, Direction.LEFT, target, this.pass, queue, ref path))
                {
                    return true;
                }
                pos = p.pos.MoveTowards(Direction.RIGHT);
                if (pos.x < this.width && this.map[pos.y, pos.x].SetPath(p, pos, Direction.RIGHT, target, this.pass, queue, ref path))
                {
                    return true;
                }
            }
            //No path found
            return false;
        }

        /// <summary>
        /// Returns the next direction to the teleporter once the Control has been reached
        /// </summary>
        /// <returns>Next direction to move into to reach the Teleporter</returns>
        public Direction NextDirectionToTeleporter()
        {
            return this.toTeleport.Pop();
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
        int height = int.Parse(inputs[0]);  //Number of rows.
        int width = int.Parse(inputs[1]);   //Number of columns.
        int alarm = int.Parse(inputs[2]);   //Number of rounds between the time the alarm countdown is activated and the time the alarm goes off.
        Map map = new Map(width, height);   //Initiates the map

        //Game loop
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            //Current position of Kirk
            Point pos = new Point(int.Parse(inputs[1]), int.Parse(inputs[0]));
            //Updates the map
            map.UpdateMap();
            //Mark the control room as reached if currently over
            if (map[pos] == MapValue.CONTROL) { map.ControlReached = true; }
            //If control room not reached
            if (!map.ControlReached)
            {
                //And return path short enough, move towards control room
                if (map.ControlFound && map.PathCorrect && map.PathLength <= alarm) { Console.WriteLine(EnumUtils.GetName(map.ToControl(pos))); }
                else
                {
                    Direction dir = Direction.UP;
                    //Else move towards closest unknown region
                    map.ToClosestUnknown(pos, ref dir);
                    Console.WriteLine(EnumUtils.GetName(dir));
                    //If control found, set new shortest path
                    if (map.ControlFound) { map.SetTeleportPath(); }
                }
            }
            else
            {
                //Mlse move towards teleporter
                Console.WriteLine(EnumUtils.GetName(map.NextDirectionToTeleporter()));
            }
        }
    }
    #endregion
}

/// <summary>
/// Some enum parsing utilities
/// </summary>
public static class EnumUtils
{
    #region Fields
    /// <summary>
    /// Stores character->map value conversions
    /// </summary>
    private static readonly Dictionary<char, MapValue> toItem;
    /// <summary>
    /// Stores direction->string conversions
    /// </summary>
    private static readonly Dictionary<Direction, string> toString;
    #endregion

    #region Constructors
    /// <summary>
    /// Initiates the conversion dictionaries
    /// </summary>
    static EnumUtils()
    {
        toItem = new Dictionary<char, MapValue>(5)
        {
            { '?', MapValue.UNKOWN },
            { '.', MapValue.EMPTY },
            { '#', MapValue.WALL },
            { 'T', MapValue.TELEPORTER },
            { 'C', MapValue.CONTROL }
        };
        toString = new Dictionary<Direction, string>(4)
        {
            { Direction.UP, "UP" },
            { Direction.DOWN, "DOWN" },
            { Direction.LEFT, "LEFT" },
            { Direction.RIGHT, "RIGHT" }
        };
    }
    #endregion

    #region Methods
    //Both methods are used carelessly, but it's because the only setting for their usage is this and no errors should arise

    /// <summary>
    /// Finds the matching MapValue for the given character of the input grid
    /// </summary>
    /// <param name="value">Character to match</param>
    /// <returns>MapValue of the passed char</returns>
    public static MapValue GetItem(char value)
    {
        return toItem[value];
    }

    /// <summary>
    /// Obtains the string name of the given direction
    /// </summary>
    /// <param name="value">Direction to get the name from</param>
    /// <returns>Name of the direction</returns>
    public static string GetName(Direction value)
    {
        return toString[value];
    }
    #endregion
}