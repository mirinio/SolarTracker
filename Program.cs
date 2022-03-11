using Iot.Device.ServoMotor;
using Iot.Device.Uln2003;
using System;
using System.Device.Pwm;


namespace Solartracker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Solartracker ---");

            Solartracker tracker = new Solartracker();
            tracker.RunGPS();
        }
    }
}