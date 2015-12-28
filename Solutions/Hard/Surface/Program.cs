using System;
using System.Collections.Generic;

public class Solution
{
    #region Constants
    /// <summary>
    /// Character identifying a lake in the map
    /// </summary>
    public const char lakeIdentifier = 'O';
    #endregion

    #region Fields
    /// <summary>
    /// Width of the map
    /// </summary>
    public static int width;
    /// <summary>
    /// Height of the map
    /// </summary>
    public static int height;
    #endregion

    #region Classes
    /// <summary>
    /// Defines a lake size
    /// </summary>
    public class Lake
    {
        #region Properties
        private int _size = 0;
        public int size
        {
            get { return this._size; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new Lake
        /// </summary>
        public Lake() { }
        #endregion

        #region Methods
        /// <summary>
        /// Increments the size of a lake in the map
        /// </summary>
        /// <param name="lakes">Map of the lakes</param>
        /// <param name="passed">Map of the passed sections of the map</param>
        /// <param name="p">Position of the lake</param>
        /// <param name="points">Search queue</param>
        public void IncrementSize(Lake[,] lakes, bool[,] passed, Point p, Queue<Point> points)
        {
            this._size++;
            //Unfortunately cannot use pointers as we can't compile with unsafe
            lakes[p.y, p.x] = this;
            passed[p.y, p.x] = true;
            points.Enqueue(p);
        }
        #endregion
    }

    /// <summary>
    /// 2D position
    /// </summary>
    public struct Point
    {
        #region Fields
        /// <summary>
        /// X axis position
        /// </summary>
        public readonly int x;
        /// <summary>
        /// Y axis position
        /// </summary>
        public readonly int y;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new Point
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
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
        width = int.Parse(Console.ReadLine());
        height = int.Parse(Console.ReadLine());
        //String map
        string[] map = new string[height];
        for (int i = 0; i < height; i++)
        {
            map[i] = Console.ReadLine();
        }
        //Passed points in the map
        bool[,] passed = new bool[height, width];
        //Map of lakes
        Lake[,] lakes = new Lake[height, width];
        for(int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                string line = map[i];
                //If the position has not been passed and the position is a lake
                if (!passed[i, j] && line[j] == lakeIdentifier)
                {
                    //Create a new lake and find all neighbouring cells
                    GetNeighbours(new Lake(), new Point(j, i), map, lakes, passed);
                }
            }
        }

        int n = int.Parse(Console.ReadLine());  //Number of points to fetch
        string[] inputs;
        int[] sizes = new int[n];
        for (int i = 0; i < n; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            //Get object in the provided position in the map
            Lake l = lakes[int.Parse(inputs[1]), int.Parse(inputs[0])];
            //If null, no lake there, else, add size of the lake
            sizes[i] = l == null ? 0 : l.size; 
        }
        for (int i = 0; i < n; i++)
        {
            Console.WriteLine(sizes[i]);
        }
    }
    
    /// <summary>
    /// Gets all the neighbouring lake cells to an original starting point
    /// </summary>
    /// <param name="lake">Lake object</param>
    /// <param name="point">Starting point</param>
    /// <param name="map">String map of the lakes</param>
    /// <param name="lakes">LAkes reference ofbject</param>
    /// <param name="passed">Reference of passed cells</param>
    public static void GetNeighbours(Lake lake, Point point, string[] map, Lake[,] lakes, bool[,] passed)
    {
        Queue<Point> points = new Queue<Point>();
        lake.IncrementSize(lakes, passed, point, points);
        int x, y;
        while (points.Count > 0)
        {
            Point p = points.Dequeue();
            x = p.x;
            y = p.y;
            x--;
            //Find lake to the left
            if (x >= 0 && !passed[y, x] && map[y][x] == lakeIdentifier)
            {
                lake.IncrementSize(lakes, passed, new Point(x, y), points);
            }
            x += 2;
            //Find lake to the right
            if (x < width && !passed[y, x] && map[y][x] == lakeIdentifier)
            {
                lake.IncrementSize(lakes, passed, new Point(x, y), points);
            }
            x--;
            y--;
            //Find lake above
            if (y >= 0 && !passed[y, x] && map[y][x] == lakeIdentifier)
            {
                lake.IncrementSize(lakes, passed, new Point(x, y), points);
            }
            y += 2;
            //Find lake underneath
            if (y < height && !passed[y, x] && map[y][x] == lakeIdentifier)
            {
                lake.IncrementSize(lakes, passed, new Point(x, y), points);
            }
        }
    }
    #endregion
}