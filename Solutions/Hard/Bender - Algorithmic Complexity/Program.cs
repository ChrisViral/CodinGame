using System;
using System.Linq;

/// <summary>
/// Solution class
/// </summary>
public class Solution
{
    #region Constants
    //All complexities (except for O(1) as it's a special case)
    public static readonly string[] complexities = { "O(log n)", "O(n)", "O(n log n)", "O(n^2)", "O(n^2 log n)", "O(n^3)", "O(2^n)" };
    #endregion

    #region Classes
    /// <summary>
    /// Data point structure
    /// </summary>
    public struct DataPoint
    {
        #region Fields
        /// <summary>
        /// Number of passed elements (x coordinate)
        /// </summary>
        public readonly int n;
        /// <summary>
        /// Time taken by the algorithm with the given amount of elements (y coordinate)
        /// </summary>
        public readonly double time;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new DataPoint
        /// </summary>
        /// <param name="n">Amount of elements</param>
        /// <param name="time">Time taken</param>
        public DataPoint(int n, double time)
        {
            this.n = n;
            this.time = time;
        }
        #endregion
    }

    /// <summary>
    /// Linear regression result structure
    /// </summary>
    public struct RegressionResult
    {
        #region Fields
        /// <summary>
        /// Slope of the regression
        /// </summary>
        public readonly double slope;
        /// <summary>
        /// Value of the intercept on the Y axis
        /// </summary>
        public readonly double intercept;
        /// <summary>
        /// Correlation coefficient
        /// </summary>
        public readonly double r2;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new RegressionResult
        /// </summary>
        /// <param name="slope">Slope of the regression</param>
        /// <param name="intercept">Y intercept of the regression</param>
        /// <param name="r2">Correlation coefficient of the regression</param>
        public RegressionResult(double slope, double intercept, double r2)
        {
            this.slope = slope;
            this.intercept = intercept;
            this.r2 = r2;
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
        int N = int.Parse(Console.ReadLine());
        string[] inputs;
        DataPoint[] points = new DataPoint[N];
        for (int i = 0; i < N; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            points[i] = new DataPoint(int.Parse(inputs[0]), int.Parse(inputs[1]));
        }
        double average = points.Average(p => p.time);
        //This is a safety preventing Math.Exp(x) to throw Inifinity values
        double d = Math.Pow(10, Math.Floor(Math.Log10(average)));
        RegressionResult[] results = new RegressionResult[7];
        //By applying the inverse function, we make the function linear and can do a linear regression on the data
        results[0] = LinearRegression(points, x => Math.Exp(x / d));        //Inverse of Ln(x) is e^x
        results[1] = LinearRegression(points, x => x);                      //Already linear
        results[2] = LinearRegression(points, x => InverseNLogN(x));        //Inverse of x*Ln(x) not calculable classically, approximation 
        results[3] = LinearRegression(points, x => Math.Sqrt(x));           //Inverse of x^2 is Sqrt(x)
        results[4] = LinearRegression(points, x => InverseN2LogN(x));       //Inverse of x^2*Ln(x) not calculable classically, approximation 
        results[5] = LinearRegression(points, x => Math.Pow(x, 1 / 3d));    //Inverse of x^3 is x^(1/3)
        results[6] = LinearRegression(points, x => Math.Log(2, x));         //Inverse of 2^x is Log_2(x)

        int index = GetBest(results);
        RegressionResult r = results[index];
        //If it's linear, or if the resulting R^2 is very small (the regression is forced through 0), it might be constant
        if (index == 1 || r.r2 < 0.25)
        {
            //If all points are within 10% of the average, that's probably constant
            if (points.All(p => Math.Abs(p.time - average) <= average * 0.1))
            {
                Console.WriteLine("O(1)");
                return;
            }
        }

        Console.WriteLine(complexities[index]);
    }

    /// <summary>
    /// Does a linear regression on the set of data points applying the given transformation
    /// </summary>
    /// <param name="points">Points to regress on</param>
    /// <param name="transform">Transformation to apply to each point</param>
    /// <returns>The result of the linear regression</returns>
    public static RegressionResult LinearRegression(DataPoint[] points, Func<double, double> transform)
    {
        //Values
        double sx = 0, sy = 0;          //sum(x), sum(y)
        double sxx = 0, syy = 0;        //sum(x�), sum(y�)
        double sxy = 0;                 //sum(x*y)
        double count = points.Length;   //Size of the set
        for (int i = 0; i < count; i++)
        {
            DataPoint p = points[i];
            double x = p.n;
            double y = transform(p.time);   //Inverts original y
            sx += x;
            sy += y;
            sxx += x * x;
            syy += y * y;
            sxy += x * y;
        }
        double sxsx = sx * sx, sysy = sy * sy, sxsy = sx * sy;  //sum(x)�, sum(y)�, sum(x)*sum(y)
        double ssx = sxx - ((sx * sx) / count);                 //sum(x�)-sum(x)�
        double ssy = syy - ((sy * sy) / count);                 //sum(y�)-sum(y)�
        double ssxy = sxy - ((sx * sy) / count);                //sum(x*y)-sum(x)sum(y)

        double avgx = sx / count;   //x average
        double avgy = sy / count;   //y average

        //R coefficient
        double r = ((count * sxy) - (sx * sy)) / Math.Sqrt((count * sxx - (sx * sx)) * (count * syy - (sy * sy)));
        //R� coefficient
        double r2 = r * r;

        //y = a*x + b
        double a = ssxy / ssx;          //a
        double b = avgy - (a * avgx);   //b
        return new RegressionResult(a, b, r2);
    }

    /// <summary>
    /// Finds the value of n for the equation n*Ln(n) = x
    /// </summary>
    /// <param name="x">Value of x</param>
    /// <param name="loops">Number of loops to the approximation</param>
    /// <returns>The value of n</returns>
    public static double InverseNLogN(double x, int loops = 10)
    {
        //n*Ln(n) = x => n = x/Ln(n)
        //By doing this division multiple time, n will converge to it's actual value
        double n = x / Math.Log(x);
        for (int i = 1; i < loops; i++)
        {
            n = x / Math.Log(n);
        }
        return n;
    }

    /// <summary>
    /// Finds the value of n for the equation n^2*Ln(n) = x
    /// </summary>
    /// <param name="x">Value of x</param>
    /// <param name="loops">Number of loops to the approximation</param>
    /// <returns>The value of n</returns>
    public static double InverseN2LogN(double x, int loops = 10)
    {
        //n^2*Ln(n) = x => n^2 = x/Ln(n) => n = Sqrt(x/Ln(n))
        //By doing this division multiple time, n will converge to it's actual value
        double n = Math.Sqrt(x / Math.Log(x));
        for (int i = 1; i < loops; i++)
        {
            n = Math.Sqrt(x / Math.Log(n));
        }
        return n;
    }

    /// <summary>
    /// Gets the index of the best matching complexity
    /// </summary>
    /// <param name="values">All the done regressions regressions</param>
    /// <returns>The index of the best regression</returns>
    public static int GetBest(RegressionResult[] values)
    {
        if (values.Length == 0) { return -1; }
        //Gets index of max R^2 value
        double value = values[0].r2;
        int index = 0;
        for (int i = 1; i < values.Length; i++)
        {
            double d = values[i].r2;
            if (d > value)
            {
                value = d;
                index = i;
            }
        }
        return index;
    }
    #endregion
}