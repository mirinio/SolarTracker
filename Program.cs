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
            Angle(pwm, 45);

            bool start = true;
            int currentAngle = 0;

            while (start)
            {
                ConsoleKeyInfo keyinfo = Console.ReadKey(true);

                switch (keyinfo.Key)
                {
                    case ConsoleKey.Backspace:
                        start = false;
                        break;
                    case ConsoleKey.LeftArrow:
                        Console.WriteLine("++LEFT++");
                        Angle(pwm, currentAngle++);
                        break;

                    case ConsoleKey.RightArrow:
                        Console.WriteLine("--RIGHT--");
                        Angle(pwm, currentAngle--);
                        break;
                    case ConsoleKey.UpArrow:
                        Console.WriteLine("++UP++");
                        Angle(pwm, currentAngle++);
                        break;
                    case ConsoleKey.DownArrow:
                        Console.WriteLine("--DOWN--");
                        Angle(pwm, currentAngle--);
                        break;
                    default:
                        start = false;
                        break;
                }
            }
            
        }

        internal static void Angle(PwmChannel pwm, int angle)
        {
            //360° ~= 1050ms workaround for broken servo
            //mindest winkel 1050 * 1°/360° ~= 20° und nicht 1° leider
            pwm.Start();
            Thread.Sleep(100);
            pwm.Stop();
        }
    }
}