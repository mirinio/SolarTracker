using Iot.Device.ServoMotor;
using System;
using System.Device.Pwm;

namespace Solartracker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- TEST 1 ---");

            var pwm = PwmChannel.Create(0, 1, 50);

            pwm.DutyCycle = 0.01;

            Angle(pwm, 50);

        }

        internal static void Angle(PwmChannel pwm, int angle)
        {
            //360° = 1050ms
            pwm.Start();
            Thread.Sleep();
            pwm.Stop();
        }
    }
}