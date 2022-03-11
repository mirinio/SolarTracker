using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solartracker
{
    internal class Solartracker
    {
        internal BrokenServo Top { get; private set; }
        //internal BrokenServo Bottom { get; private set; }

        public Solartracker(BrokenServo top)
        {
            Top = top;

        }

        public void RunManual()
        {
            bool start = true;
            while (start)
            {
                Console.WriteLine(Top.ToString());

                ConsoleKeyInfo keyinfo = Console.ReadKey(true);

                switch (keyinfo.Key)
                {
                    case ConsoleKey.Backspace:
                        start = false;
                        Console.WriteLine("Stop");
                        break;
                    case ConsoleKey.LeftArrow:
                        //Console.WriteLine("++LEFT++");
                        //CurrentAngle = Angle(pwm, currentAngle++);
                        break;

                    case ConsoleKey.RightArrow:
                        //Console.WriteLine("--RIGHT--");
                        //CurrentAngle = Angle(pwm, currentAngle--);
                        break;
                    case ConsoleKey.UpArrow:
                        Console.WriteLine("++UP++");
                        Top.SetAngle(Top.CurrentAngle + 45);
                        break;
                    case ConsoleKey.DownArrow:
                        Console.WriteLine("--DOWN--");
                        Top.SetAngle(Top.CurrentAngle - 45);
                        break;
                    default:
                        Console.WriteLine("Use Arrow keys, hit Backspace to stop");
                        start = false;
                        break;
                }
            }
        }
    }
}
