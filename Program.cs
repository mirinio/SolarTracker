using Iot.Device.Uln2003;

namespace Solartracker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Solartracker ---");

            Solartracker tracker = new Solartracker();
            //tracker.RunGPS();
            tracker.RunManual();
        }
    }
}