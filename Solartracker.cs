using Iot.Device.Uln2003;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solartracker
{
    internal class Solartracker
    {
        private const int yellowPin = 14;
        private const int whitePin = 15;
        private const int greenPin = 18;
        private const int bluePin = 23;

        public int ElevationAngle { get; private set; }
        public int AzimutAngle { get; private set; }


        public Solartracker() 
        {
            ElevationAngle = 0;
            AzimutAngle = 0;
        }

        public void RunManual()
        {
            bool start = true;

            using (Uln2003 elevationMotor = new Uln2003(yellowPin, whitePin, greenPin, bluePin))
            {
                elevationMotor.RPM = 15;
                elevationMotor.Mode = StepperMode.FullStepDualPhase;

                while (start)
                {
                    ConsoleKeyInfo keyinfo = Console.ReadKey(true);

                    switch (keyinfo.Key)
                    {
                        case ConsoleKey.Backspace:
                            start = false;
                            elevationMotor.Stop();
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
                            ElevationAngle = MoveAngle(elevationMotor, ElevationAngle, ElevationAngle + 1);

                            break;
                        case ConsoleKey.DownArrow:
                            Console.WriteLine("--DOWN--");
                            ElevationAngle = MoveAngle(elevationMotor, ElevationAngle, ElevationAngle - 1, isClockwise: false);
                            break;
                        default:
                            Console.WriteLine("Use Arrow keys, hit Backspace to stop");
                            start = false;
                            elevationMotor.Stop();
                            break;
                    }
                }
            }
        }

        public void RunGPS()
        {
            bool start = true;

            using (Uln2003 elevationMotor = new Uln2003(yellowPin, whitePin, greenPin, bluePin))
            {
                elevationMotor.RPM = 15;
                elevationMotor.Mode = StepperMode.FullStepDualPhase;

                while (start)
                {
                    ConsoleKeyInfo keyinfo = Console.ReadKey(true);

                    if (keyinfo.Key == ConsoleKey.Backspace)
                    {
                        start = false;
                        elevationMotor.Stop();
                    }

                    ElevationAngle = SetAngle(elevationMotor, ElevationAngle, 90);
                    Thread.Sleep(2000);
                    ElevationAngle = SetAngle(elevationMotor, ElevationAngle, 180);
                    //Thread.Sleep(2000);
                    //ElevationAngle = SetAngle(elevationMotor, 90, 180);
                    //Thread.Sleep(2000);
                    //ElevationAngle = SetAngle(elevationMotor, 180, 270);
                    //Thread.Sleep(2000);
                    //ElevationAngle = SetAngle(elevationMotor, 270, 360);
                    //Thread.Sleep(2000);
                    //ElevationAngle = SetAngle(elevationMotor, 360, 180);
                    //Thread.Sleep(2000);
                    //ElevationAngle = SetAngle(elevationMotor, 180, 135);
                    //Thread.Sleep(2000);
                    //ElevationAngle = SetAngle(elevationMotor, 135, 0);

                }
            }
        }


        private int MoveAngle(Uln2003 motor, int currentAngle, int angle, bool isClockwise = true)
        {
            if (IsOutOfRadius(angle))
            {
                return currentAngle;
            }
            else if (currentAngle == 0)
            {
                int turnStep = (2048 * angle) / 360;
                motor.Step(turnStep);

                return angle;
            }
            else if (angle == 0)
            {
                int turnStep = (2048 * currentAngle) / 360;
                motor.Step(-turnStep);

                return 0;
            }
            else if (isClockwise)
            {
                int turnSteps = (2048 * angle) / 360;
                motor.Step(turnSteps);
                return angle + currentAngle;
            }
            else
            {
                int turnSteps = (2048 * angle) / 360;
                motor.Step(-turnSteps);
                return currentAngle - angle;
            }
        }


        private int SetAngle(Uln2003 motor, int currentAngle, int angle)
        {
            if (IsOutOfRadius(angle) || currentAngle == angle)
            {
                return currentAngle;
            }
            else if (currentAngle == 0)
            {
                int turnStep = (2048 * angle) / 360;
                motor.Step(turnStep);
            }
            else if (angle == 0)
            {
                int turnStep = (2048 * currentAngle) / 360;
                motor.Step(-turnStep);
            }
            else if (angle < currentAngle)
            {
                int currentSteps = (2048 * currentAngle) / 360;
                int newAngleSteps = (2048 * angle) / 360;
                int turnStepsBack = currentSteps - newAngleSteps;
                motor.Step(-turnStepsBack);
            }
            else if (angle > currentAngle)
            {
                int currentSteps = (2048 * currentAngle) / 360;
                int newAngleSteps = (2048 * angle) / 360;
                int turnStepsForward = newAngleSteps - currentSteps;

                motor.Step(turnStepsForward);
            }

            return angle;
        }

        private bool IsOutOfRadius(int angle)
        {
            return (angle < 0 || angle > 360);
        }
    }
}
