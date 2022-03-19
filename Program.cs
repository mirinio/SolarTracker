using System.Text;

namespace Solartracker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Solartracker ---");
            Solartracker tracker = new Solartracker();

            if (args.Length == 1)
            {
                string argument = args[0].Trim().ToLower();

                if (argument.Equals("gps"))
                {
                    Console.WriteLine("Starting GPS tracking....");
                    tracker.RunGPS();
                }
                else if (argument.Equals("manual"))
                {
                    Console.WriteLine("Starting manual tracking....");
                    tracker.RunManual();
                }
                else if (argument.Equals("gpsdemo"))
                {
                    Console.WriteLine("Starting GPS demo tracking....");
                    //Demo zürich
                    double latitude = 47.368650;
                    double longitude = 8.539183;
                    string dateTime = DateTime.Now.ToString("dd-MM-YYYY:HH:mm:ss");
                    Console.WriteLine($"Date and Time: {dateTime}");
                    Console.WriteLine($"--Zurich Demo Koordinaten--");
                    Console.WriteLine($"Latitude: {latitude}");
                    Console.WriteLine($"Longitude: {longitude}");

                    tracker.RunGPSDemo(DateTime.Now, latitude, longitude);
                }
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("Start Default");
                builder.AppendLine("starting GPS tracking...");

                builder.AppendLine("--Solartracker Parameters--");
                builder.AppendLine("dotnet Solartracker.dll [manual] => start Manual tracking");
                builder.AppendLine("dotnet Solartracker.dll [gps] => start GPS tracking");
                builder.AppendLine("dotnet Solartracker.dll [gpsdemo] => start GPS tracking");

                Console.WriteLine(builder.ToString());

                tracker.RunGPS();
            }
        }
    }
}