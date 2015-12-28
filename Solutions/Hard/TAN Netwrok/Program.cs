using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/* Uses the A* algorithm to find the shortest point between two nodes of a directed
 * directed, weighted graph */

/// <summary>
/// A generic Priority Queue implementation using a binary heap.
/// </summary>
/// <typeparam name="T">Type of the queue</typeparam>
public class PriorityQueue<T> : ICollection<T>
{
    #region Constants
    /// <summary>
    /// Base capacity of the memory List
    /// </summary>
    public const int baseCapacity = 4;
    #endregion

    #region Properties
    /// <summary>
    /// Amount of Stops stored in the queue
    /// </summary>
    public int Count
    {
        get { return this.heap.Count; }
    }

    /// <summary>
    /// Current capacity of the queue
    /// </summary>
    public int Capacity
    {
        get { return this.heap.Capacity; }
    }

    /// <summary>
    /// Internal memory List is never read only
    /// </summary>
    bool ICollection<T>.IsReadOnly
    {
        get { return false; }
    }

    /// <summary>
    /// Returns the index of the last member of the queue
    /// </summary>
    private int Last
    {
        get { return this.heap.Count - 1; }
    }
    #endregion

    #region Fields
    /// <summary>
    /// Internal Queue memory
    /// </summary>
    private readonly List<T> heap;
    /// <summary>
    /// Dictionary of the values and their index
    /// </summary>
    private readonly Dictionary<T, int> indexes;
    /// <summary>
    /// Comparer to sort the values of this instance
    /// </summary>
    private readonly IComparer<T> comparer;
    #endregion

    #region Constructors
    /// <summary>
    /// Creates an empty PriorityQueue using the default IComparer of <typeparamref name="T"/>
    /// </summary>
    public PriorityQueue() : this(baseCapacity, Comparer<T>.Default) { }

    /// <summary>
    /// Creates a PriorityQueue of the given capacity with the default IComparer of <typeparamref name="T"/>
    /// </summary>
    /// <param name="capacity">Capacity of the PriorityQueue</param>
    public PriorityQueue(int capacity) : this(capacity, Comparer<T>.Default) { }

    /// <summary>
    /// Creates an empty PriorityQueue with the provided IComparer of <typeparamref name="T"/>
    /// </summary>
    /// <param name="comparer">Comparer to sort the Queue</param>
    public PriorityQueue(IComparer<T> comparer) : this(baseCapacity, comparer) { }

    /// <summary>
    /// Creates an PriorityQueue of the given size with the IComparer of <typeparamref name="T"/> provided
    /// </summary>
    /// <param name="capacity">Capacity of the queue</param>
    /// <param name="comparer">Comparer to sort the Queue</param>
    public PriorityQueue(int capacity, IComparer<T> comparer)
    {
        if (comparer == null) { throw new ArgumentNullException("comparer", "Default IComparer cannot be null"); }
        this.heap = new List<T>(capacity);
        this.indexes = new Dictionary<T, int>(capacity);
        this.comparer = comparer;
    }

    /// <summary>
    /// Creates the PriorityQueue from the IEnumerable provided with the default IComparer of <typeparamref name="T"/>
    /// </summary>
    /// <param name="enumerable">Enumerable to make the StopQueue from</param>
    public PriorityQueue(IEnumerable<T> enumerable) : this(enumerable, Comparer<T>.Default) { }

    /// <summary>
    /// Creates the PriorityQueue from the IEnumerable provided with the IComparer of <typeparamref name="T"/> provided
    /// </summary>
    /// <param name="enumerable">Enumerable to make the PriorityQueue from</param>
    /// <param name="comparer">Comparer to sort the values</param>
    public PriorityQueue(IEnumerable<T> enumerable, IComparer<T> comparer)
    {
        if (comparer == null) { throw new ArgumentNullException("comparer", "Default IComparer cannot be null"); }
        this.heap = new List<T>(enumerable);
        this.comparer = comparer;
        for (int i = this.heap.Count / 2; i >= 1; i--)
        {
            HeapDown(i);
        }
        for (int i = 0; i < this.Count; i++)
        {
            this.indexes.Add(this.heap[i], i);
        }
    }

    /// <summary>
    /// Cloning constructor, creates a new PriorityQueue of <typeparamref name="T"/> from an existing one
    /// </summary>
    /// <param name="queue"></param>
    public PriorityQueue(PriorityQueue<T> queue)
    {
        this.heap = new List<T>(queue.heap);
        this.indexes = new Dictionary<T, int>(queue.indexes);
        this.comparer = queue.comparer;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Returns the index of the parent node
    /// </summary>
    /// <param name="i">Index of the child node</param>
    /// <returns>Index of the parent</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Parent(int i)
    {
        return (i - 1) / 2; //Floor is done by integer division
    }

    /// <summary>
    /// Returns the index of the left child node
    /// </summary>
    /// <param name="i">Index of the parent node</param>
    /// <returns>Index of the left child node</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int LeftChild(int i)
    {
        return (2 * i) + 1;
    }

    /// <summary>
    /// Returns the index of the right child node
    /// </summary>
    /// <param name="i">Index of the parent node</param>
    /// <returns>Index of the right child node</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int RightChild(int i)
    {
        return (2 * i) + 2;
    }

    /// <summary>
    /// Compares the values at indexes i and j and finds out if i must be moved upwards
    /// </summary>
    /// <param name="i">Index of the bottom node</param>
    /// <param name="j">Index of the top node</param>
    /// <returns>If i must be moved to j</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool CompareUp(int i, int j)
    {
        return i > 0 && i < this.heap.Count && CompareAt(i, j) < 0;
    }

    /// <summary>
    /// Compares the values at indexes i and j and finds out if i must be moved downwards
    /// </summary>
    /// <param name="i">Index of top value</param>
    /// <param name="j">Index of bottom value</param>
    /// <returns>If i must be moved to j</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool CompareDown(int i, int j)
    {
        return j < this.heap.Count && CompareAt(i, j) > 0;
    }

    /// <summary>
    /// Meaves the target <typeparamref name="T"/> up until it satisfies heap priority
    /// </summary>
    /// <param name="i">Index of the value to move</param>
    private void HeapUp(int i)
    {
        //If index is zero it's already at the top
        if (i == 0) { return; }

        //Store value to move
        T value = this.heap[i];
        int j = Parent(i);
        while (CompareUp(i, j))
        {
            //Swap values downwards
            this.heap[i] = this.heap[j];
            this.heap[j] = value;
            i = j;
            j = Parent(i);
        }
        //Set value to final index
        this.heap[i] = value;
        this.indexes[value] = i;
    }

    /// <summary>
    /// Moves the <typeparamref name="T"/> down until it satisfies heap priority
    /// </summary>
    /// <param name="i">Index of the value to move</param>
    private void HeapDown(int i)
    {
        //Index of the left, right, and current nodes
        int l = LeftChild(i), r = RightChild(i), largest = i;
        if (CompareDown(i, l))
        {
            //If left smaller, possibly move this way
            largest = l;
        }
        if (CompareDown(largest, r))
        {
            //If right smaller, move this way
            largest = r;
        }
        if (largest > i)
        {
            T temp = this.heap[i];
            this.heap[i] = this.heap[largest];
            this.heap[largest] = temp;
            HeapDown(largest);
        }
        else if (i >= 0 && i < this.heap.Count)
        {
            this.indexes[this.heap[i]] = i;
        }
    }

    /// <summary>
    /// Adds a value in the queue by priority
    /// </summary>
    /// <param name="value"><typeparamref name="T"/> to add. Cannot be null as it isn't sortable</param>
    public void Add(T value)
    {
        if (value == null) { throw new ArgumentNullException("value", "Cannot add or sort null item in PriorityQueue"); }
        this.heap.Add(value);
        this.indexes.Add(value, 0);
        HeapUp(this.Last);
    }

    /// <summary>
    /// Removes and returns the first element of the queue by priority
    /// </summary>
    /// <returns>The first element of the queue</returns>
    public T Pop()
    {
        if (this.heap.Count == 0) { throw new InvalidOperationException("Queue empty, operation invalid"); }
        T value = this.heap[0];
        RemoveAt(0, value);
        if (this.Count > 0) { HeapDown(0); }
        return value;
    }

    /// <summary>
    /// Returns without removing the first <typeparamref name="T"/> in the queue
    /// </summary>
    /// <returns>First Item in the queue</returns>
    public T Peek()
    {
        return this.heap[0];
    }

    /// <summary>
    /// Removes the given value at the given index
    /// </summary>
    /// <param name="i">Index of the value to remove</param>
    /// <param name="value">Value to remove</param>
    private void RemoveAt(int i, T value)
    {
        this.heap[i] = this.heap[Last];
        this.heap.RemoveAt(Last);
        this.indexes.Remove(value);
    }

    /// <summary>
    /// Removes the provided <typeparamref name="T"/> from the queue
    /// </summary>
    /// <param name="value">Value to remove</param>
    /// <returns>If the value was succesfully removed</returns>
    public bool Remove(T value)
    {
        if (value == null) { return false; }
        int i;
        if (this.indexes.TryGetValue(value, out i))
        {
            RemoveAt(i, value);
            Update(i);
            return true;
        }
        return false;
    }

    /// <summary>
    /// If the queue contains the given value
    /// </summary>
    /// <param name="value">Value to find</param>
    /// <returns>True when the queue contains the value, false otherwise</returns>
    public bool Contains(T value)
    {
        if (value == null) { return false; }
        return this.indexes.ContainsKey(value);
    }

    /// <summary>
    /// Clears the memory of the queue
    /// </summary>
    public void Clear()
    {
        this.heap.Clear();
        this.indexes.Clear();
    }

    /// <summary>
    /// Updates the <typeparamref name="T"/> at the index i until it satisfies heap priority
    /// </summary>
    /// <param name="i">Index of the element to update</param>
    private void Update(int i)
    {
        int j = Parent(i);
        if (j >= 0 && j < this.heap.Count && CompareUp(i, j))
        {
            T val = this.heap[i];
            HeapUp(i);
        }
        else
        {
            T val = this.heap[i];
            HeapDown(i);
        }
    }

    /// <summary>
    /// Updates the position of an element that has been modified
    /// </summary>
    /// <param name="value">Element that has been modified to move</param>
    /// <returns>If the element was found and updated correctly</returns>
    public bool Update(T value)
    {
        if (value == null) { throw new ArgumentNullException("value", "Cannot update the position of a null item"); }
        int i;
        //Makes sure object is within 
        if (this.indexes.TryGetValue(value, out i))
        {
            Update(i);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Compares two <typeparamref name="T"/> using the provided or default comparer
    /// </summary>
    /// <param name="a">First value to compare</param>
    /// <param name="b">Second value to compare</param>
    /// <returns>-1 if a comes before b, 1 if it comes after b, and 0 if they are equal</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Compare(T a, T b)
    {
        return this.comparer.Compare(a, b);
    }

    /// <summary>
    /// Compares the two <typeparamref name="T"/> at both indexes in the memory using the provided or default comparer
    /// </summary>
    /// <param name="i">Index of the first value to compare</param>
    /// <param name="j">Index of the second value to compare</param>
    /// <returns>-1 if i comes before j, 1 if it comes after j, and 0 if they are equal</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int CompareAt(int i, int j)
    {
        return Compare(this.heap[i], this.heap[j]);
    }

    /// <summary>
    /// Copies the PriorityQueue to a generic array. This is an O(n Log(n)) operation.
    /// </summary>
    /// <param name="array">Array to copy to</param>
    public void CopyTo(T[] array)
    {
        CopyTo(array, 0);
    }

    /// <summary>
    /// Copies the PriorityQueue to a generic array. This is an O(n Log(n)) operation.
    /// </summary>
    /// <param name="array">Array to copy to</param>
    /// <param name="arrayIndex">Starting index in the target array to put the objects in</param>
    public void CopyTo(T[] array, int index)
    {
        this.heap.CopyTo(array, index);
        Array.Sort(array, this.comparer);
    }

    /// <summary>
    /// Returns this queue in a sorted array. This is an O(n Log(n)) operation.
    /// </summary>
    /// <returns>Array of the queue</returns>
    public T[] ToArray()
    {
        T[] a = this.heap.ToArray();
        Array.Sort(a, this.comparer);
        return a;
    }

    /// <summary>
    /// Returns this queue in a sorted List of <typeparamref name="T"/>. This is an O(n Log(n)) operation.
    /// </summary>
    /// <returns>List of this queue</returns>
    public List<T> ToList()
    {
        List<T> l = new List<T>(this.heap);
        l.Sort(this.comparer);
        return l;
    }

    /// <summary>
    /// Trims the memory of the PriorityQueue to it's size if more than 10% of the memory is unused
    /// </summary>
    public void TrimExcess()
    {
        this.heap.TrimExcess();
    }

    /// <summary>
    /// Returns an IEnumerator of <typeparamref name="T"/> from this PriorityQueue. Warning: Obtening the first element of the iterator is O(n).
    /// </summary>
    /// <returns>Iterator going through this sequence</returns>
    public IEnumerator<T> GetEnumerator()
    {
        PriorityQueue<T> s = new PriorityQueue<T>(this);
        while (s.heap.Count > 0)
        {
            yield return s.Pop();
        }
    }

    /// <summary>
    /// Returns an IEnumerator from this PriorityQueue. Warning: Obtening the first element of the iterator is O(n).
    /// </summary>
    /// <returns>Iterator going through this sequence</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    #endregion
}

/// <summary>
/// Solution class
/// </summary>
public class Solution
{
    #region Classes
    /// <summary>
    /// Unidirectional route struct between two stops
    /// </summary>
    public struct Route
    {
        #region Fields
        /// <summary>
        /// Destination stop
        /// </summary>
        public readonly Stop stop;
        /// <summary>
        /// Distance of the route
        /// </summary>
        public readonly double distance;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new Route to the given Stop of the given length
        /// </summary>
        /// <param name="stop">Stop the route goes to</param>
        /// <param name="distance">Length of the route</param>
        public Route(Stop stop, double distance)
        {
            this.stop = stop;
            this.distance = distance;
        }
        #endregion
    }

    /// <summary>
    /// Latitude/Longitude coordinates struct
    /// </summary>
    public struct Coordinate
    {
        #region Constants
        public const double degToRad = Math.PI / 180;   //Degree->radians converting constant
        public const int earthRadii = 6371;             //Radius of the earth in km
        #endregion

        #region Fields
        /// <summary>
        /// Latitude in radians
        /// </summary>
        public readonly double latitude;
        /// <summary>
        /// Longitude in radians
        /// </summary>
        public readonly double longitude;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new Coordinates binding from the given latitude/longitude
        /// </summary>
        /// <param name="latitude">Latitude of the coordinates in degree</param>
        /// <param name="longitude">Latitude of the coordiante (in degrees)</param>
        public Coordinate(double latitude, double longitude)
        {
            this.latitude = latitude * degToRad;
            this.longitude = longitude * degToRad;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Calculates the distance in km between two coordinates
        /// </summary>
        /// <param name="a">First coordinate</param>
        /// <param name="b">Second coordinate</param>
        /// <returns>The distance in km between a and b</returns>
        public static double GetDistance(Coordinate a, Coordinate b)
        {
            double x = (b.longitude - a.longitude) * Math.Cos((a.latitude + b.latitude) / 2);
            double y = b.latitude - a.latitude;
            return Math.Sqrt((x * x) + (y * y)) * earthRadii;
        }
        #endregion
    }

    /// <summary>
    /// Class representing a public transport Stop station
    /// </summary>
    public class Stop : IEquatable<Stop>
    {
        #region Constants
        /// <summary>
        /// Character splitting the info lines
        /// </summary>
        private static readonly char[] splitter = { ',' };
        #endregion

        #region Fields
        /// <summary>
        /// String ID of this Stop
        /// </summary>
        public readonly string id;
        /// <summary>
        /// String name of this Stop
        /// </summary>
        public readonly string name;
        /// <summary>
        /// Coordinates of this stop
        /// </summary>
        public readonly Coordinate coordinate;
        //public readonly int type;     Unused
        /// <summary>
        /// Routes leaving from this Stop
        /// </summary>
        public readonly List<Route> routes = new List<Route>();
        #endregion

        #region Search
        /// <summary>
        /// Minimum distance from the search start node
        /// </summary>
        public double distance = double.PositiveInfinity;
        /// <summary>
        /// Distance from the target station
        /// </summary>
        public double targetDistance = 0;
        /// <summary>
        /// If this node has been visited
        /// </summary>
        public bool visited = false;
        /// <summary>
        /// If the Stop has been added to the search list
        /// </summary>
        public bool added = false;
        /// <summary>
        /// Node coming from that gives the smallest distance from the starting node
        /// </summary>
        public Stop from = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new stop from the info string
        /// </summary>
        /// <param name="info">Info string</param>
        public Stop(string info)
        {
            //Remove unused fields
            string[] stats = info.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
            this.id = stats[0];                 //String id: first fields
            this.name = stats[1].Trim('"');     //Name: second field
            //Latitude: third field - Longitude: fourth field
            this.coordinate = new Coordinate(double.Parse(stats[2]), double.Parse(stats[3]));
            //this.type = int.Parse(stats[4]);      Unused (always 1?)
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds a route from this stop to the given stop
        /// </summary>
        /// <param name="stop">Stop the route leads to</param>
        public void AddRoute(Stop stop)
        {
            this.routes.Add(new Route(stop, Coordinate.GetDistance(this.coordinate, stop.coordinate)));
        }

        /// <summary>
        /// If the given object is equal to this intance
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns>If the objects are equal</returns>
        public override bool Equals(object obj)
        {
            //Can only be true when object is a stop
            if (obj != null && obj is Stop)
            {
                return Equals((Stop)obj);
            }
            return false;
        }

        /// <summary>
        /// IF the given stop is equal to this instance
        /// </summary>
        /// <param name="stop">Stop to compare</param>
        /// <returns>If the Stops are equal</returns>
        public virtual bool Equals(Stop stop)
        {
            //Stops are equal when their ID is
            return this.id.Equals(stop.id);
        }

        /// <summary>
        /// Gets the hascode of this Stop instance
        /// </summary>
        /// <returns>Hashcode of the instance</returns>
        public override int GetHashCode()
        {
            //Stops are their IDs, so return ID hashcode
            return this.id.GetHashCode();
        }

        /// <summary>
        /// Name of this Stop
        /// </summary>
        /// <returns>Name of the stop</returns>
        public override string ToString()
        {
            return this.name;
        }
        #endregion

        #region Operators
        /// <summary>
        /// If both Stops are equal
        /// </summary>
        /// <param name="a">First stop</param>
        /// <param name="b">Second stop</param>
        /// <returns>True when boths stops are null or both are equal</returns>
        public static bool operator ==(Stop a, Stop b)
        {
            bool oa = (object)a == null, ob = (object)b == null;  //For nullchecking
            return (oa || ob) ? oa == ob : a.id == b.id;
        }

        /// <summary>
        /// If boths stops are inequal
        /// </summary>
        /// <param name="a">First stop</param>
        /// <param name="b">Second stop</param>
        /// <returns>True if one stop is null and the other isnt, or both are different</returns>
        public static bool operator !=(Stop a, Stop b)
        {
            bool oa = (object)a == null, ob = (object)b == null;    //For nullchecking
            return (oa || ob) ? oa != ob : a.id != b.id;
        }
        #endregion
    }

    /// <summary>
    /// Compares two Stops by their distance from the starting stop, using a heuristic on the distance from the target stop
    /// </summary>
    public class StopComparer : IComparer<Stop>
    {
        #region Properties
        private static readonly StopComparer comparer;
        /// <summary>
        /// The default instance of StopComparer
        /// </summary>
        public static StopComparer Comparer
        {
            get { return comparer; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initiates the default StopComparer instance
        /// </summary>
        static StopComparer()
        {
            comparer = new StopComparer();
        }

        /// <summary>
        /// Creates a default StopComparer
        /// </summary>
        private StopComparer() { }
        #endregion

        #region Methods
        /// <summary>
        /// Compares two Stops by distance from the starting and target node
        /// </summary>
        /// <param name="a">First stop</param>
        /// <param name="b">Second stop</param>
        /// <returns>1 if the Stop a is closer than b, -1 if it's smaller, and 0 if they are at equal distance</returns>
        public int Compare(Stop a, Stop b)
        {
            double d1 = a.distance + a.targetDistance, d2 = b.distance + b.targetDistance;
            if (d1 == d2) { return 0; }
            return d1 > d2 ? 1 : -1;
        }
        #endregion
    }
    #endregion

    #region Methods
    /// <summary>
    /// Program
    /// </summary>
    private static void Main()
    {
        string startID = Console.ReadLine();    //Starting station
        string endID = Console.ReadLine();      //Ending station
        int n = int.Parse(Console.ReadLine());  //Number of stations

        //Stops array to set target distance
        Stop[] stopsArr = new Stop[n];
        //Starting/ending stops
        Stop start = null, end = null;
        //ID/Stop dictionary
        Dictionary<string, Stop> stops = new Dictionary<string, Stop>(n);
        for (int i = 0; i < n; i++)
        {
            Stop s = new Stop(Console.ReadLine());   //Create new stop
            stopsArr[i] = s;                        //Add to array
            stops.Add(s.id, s);                     //Add to dictionary
            if (s.id == startID) { start = s; }     //Set starting Stop
            else if (s.id == endID) { end = s; }    //Or ending stop
        }

        string[] inputs;
        //Number of routes
        int m = int.Parse(Console.ReadLine());
        for (int i = 0; i < m; i++)
        {
            inputs = Console.ReadLine().Split(' '); //Stations A B (from to)
            Stop s = stops[inputs[0]];              //Station a (from)
            s.AddRoute(stops[inputs[1]]);           //Add route to station B (to)
        }

        //If starting station is ending station, no need to go anywhere
        if (startID == endID) { Console.WriteLine(start.ToString()); return; }
        //Sets A* heuristic (distance from target stop)
        Array.ForEach(stopsArr, s => s.targetDistance = Coordinate.GetDistance(s.coordinate, end.coordinate));

        //Analyze graph and print shortest path
        PrintShortestPath(start, end);
    }

    /// <summary>
    /// Finds the shortest path from a starting node to ending node and prints it. If impossible, prints "IMPOSSIBLE"
    /// </summary>
    /// <param name="start">Starting node</param>
    /// <param name="end">Ending node</param>
    public static void PrintShortestPath(Stop start, Stop end)
    {
        //Try to find shortest path
        if (SetShortestPath(start, end))
        {
            //Path
            Stack<Stop> route = new Stack<Stop>();
            route.Push(end);
            Stop to = end.from;
            //While Stop coming front isn't null
            while (to != null)
            {
                //Add stop to path
                route.Push(to);
                to = to.from;
            }
            //While in path
            while (route.Count > 0)
            {
                //Print stop name
                Console.WriteLine(route.Pop().ToString());
            }
        }
        //If fails, print IMPOSSIBLE
        else { Console.WriteLine("IMPOSSIBLE"); }
    }

    /// <summary>
    /// Finds the shortest path from a starting node to ending node and sets the path within the graph
    /// </summary>
    /// <param name="start">Starting node</param>
    /// <param name="end">Ending node</param>
    /// <returns>If the search succeeded</returns>
    private static bool SetShortestPath(Stop start, Stop end)
    {
        start.distance = 0;
        start.added = true;
        start.visited = true;
        PriorityQueue<Stop> path = new PriorityQueue<Stop>(StopComparer.Comparer) { start };
        while(path.Count > 0)
        {
            Stop current = path.Pop();
            current.visited = true;
            foreach(Route route in current.routes)
            {
                Stop next = route.stop;
                if (next == end)
                {
                    end.from = current;
                    return true;
                }
                if (!next.visited)
                {
                    double dist = current.distance + route.distance;
                    if (dist < next.distance)
                    {
                        next.distance = dist;
                        next.from = current;
                        path.Update(next);
                    }
                    if (!next.added)
                    {
                        next.added = true;
                        path.Add(next);
                    }
                }
            }
        }
        return false;
    }
    #endregion
}