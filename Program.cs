using Innovative.SolarCalculator;
using Iot.Device.Uln2003;
using System.Globalization;

namespace Solartracker
{
    internal class Program
    {
        static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }


        static void Main(string[] args)
        {
            Console.WriteLine("--- Solartracker ---");

            Solartracker tracker = new Solartracker();
            tracker.RunGPS();
            //tracker.RunManual();
        }
    }
}