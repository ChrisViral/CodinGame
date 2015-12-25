using System;

public class Solution
{
    public struct Coordinate
    {
        #region Constants
        public const double degToRad = Math.PI / 180;
        public const double earthRadii = 6371;
        #endregion

        #region Fields
        public readonly double longitude;
        public readonly double latitude;
        #endregion

        #region Constructor
        public Coordinate(string longitude, string latitude)
        {
            this.longitude = double.Parse(longitude.Replace(',', '.')) * degToRad;
            this.latitude = double.Parse(latitude.Replace(',', '.')) * degToRad;
        }
        #endregion

        #region Methods
        public static double Distance(Coordinate a, Coordinate b)
        {
            double x = (b.longitude - a.longitude) * Math.Cos((a.latitude + b.latitude) / 2);
            double y = b.latitude - a.latitude;
            return Math.Sqrt((x * x) + (y * y)) * earthRadii;
        }
        #endregion
    }
    
    public struct Defibrillator
    {
        #region Fields
        //public readonly int id;           Unused
        public readonly string name;
        //public readonly string address;   Unused
        //public readonly string phone;     Unused
        public readonly Coordinate position;
        #endregion

        #region Constructor
        public Defibrillator(string data)
        {
            string[] values = data.Split(';');
            //this.id = int.Parse(values[0]);
            this.name = values[1];
            //this.address = values[2];
            //this.phone = values[3];
            this.position = new Coordinate(values[4], values[5]);
        }
        #endregion
    }
    
    private static void Main()
    {
        Coordinate pos = new Coordinate(Console.ReadLine(), Console.ReadLine());
        int n = int.Parse(Console.ReadLine());       
        Defibrillator[] defs = new Defibrillator[n];
        for (int i = 0; i < n; i++)
        {
            defs[i] = new Defibrillator(Console.ReadLine());
        }
        
        Defibrillator closest = defs[0];
        double distance = Coordinate.Distance(closest.position, pos);
        for (int i = 0; i < n; i++)
        {
            Defibrillator def = defs[i];
            double d = Coordinate.Distance(def.position, pos);
            if (d < distance)
            {
                closest = def;
                distance = d;
            }
        }
        
        Console.WriteLine(closest.name);
    }
}