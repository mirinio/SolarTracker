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
        //Elevation stepper motor pins
        private const int YELLOW_ELEVATION_PIN = 12;//grau
        private const int WHITE_ELEVATION_PIN = 16;//violet
        private const int GREEN_ELEVATION_PIN = 20;//rot
        private const int BLUE_ELEVATION_PIN = 21;//violet

        //Azimut stepper motor pins
        private const int YELLOW_AZIMUT_PIN = 5;//grüen
        private const int WHITE_AZIMUT_PIN = 6;//brun
        private const int GREEN_AZIMUT_PIN = 13;//gelb
        private const int BLUE_AZIMUT_PIN = 19;//blau


        public int ElevationAngle { get; private set; }
        public int AzimutAngle { get; private set; }

        public int Revolution { get; private set; }
        public Solartracker() 
        {
            ElevationAngle = 0;
            AzimutAngle = 0;
            Revolution = 2048;
        }

        public void RunManual()
        {
            bool start = true;

            using (Uln2003 azimutMotor = new Uln2003(YELLOW_AZIMUT_PIN, WHITE_AZIMUT_PIN, GREEN_AZIMUT_PIN, BLUE_AZIMUT_PIN))
            using (Uln2003 elevationMotor = new Uln2003(YELLOW_ELEVATION_PIN, WHITE_ELEVATION_PIN, GREEN_ELEVATION_PIN, BLUE_ELEVATION_PIN))
            {
                azimutMotor.RPM = 1;
                azimutMotor.Mode = StepperMode.FullStepDualPhase;
                elevationMotor.RPM = 1;
                elevationMotor.Mode = StepperMode.FullStepDualPhase;

                while (start)
                {
                    ConsoleKeyInfo keyinfo = Console.ReadKey(true);
                    Console.WriteLine($"Azimut: {AzimutAngle}");
                    Console.WriteLine($"Elevation: {ElevationAngle}");

                    switch (keyinfo.Key)
                    {
                        case ConsoleKey.Backspace:
                            start = false;
                            azimutMotor.Stop();
                            elevationMotor.Stop();
                            Console.WriteLine("Stop");
                            break;
                        case ConsoleKey.LeftArrow:
                            Console.WriteLine("++LEFT++");
                            ElevationAngle = MoveAngle(elevationMotor, ElevationAngle, 45);
                            break;

                        case ConsoleKey.RightArrow:
                            Console.WriteLine("--RIGHT--");
                            ElevationAngle = MoveAngle(elevationMotor, ElevationAngle, 45, isClockwise: false);
                            break;
                        case ConsoleKey.UpArrow:
                            Console.WriteLine("++UP++");
                            AzimutAngle = MoveAngle(azimutMotor, AzimutAngle, 45);
                            break;
                        case ConsoleKey.DownArrow:
                            Console.WriteLine("--DOWN--");
                            
                            AzimutAngle = MoveAngle(azimutMotor, AzimutAngle, 45, isClockwise: false);
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
            using (Uln2003 azimutMotor = new Uln2003(YELLOW_AZIMUT_PIN, WHITE_AZIMUT_PIN, GREEN_AZIMUT_PIN, BLUE_AZIMUT_PIN))
            using (Uln2003 elevationMotor = new Uln2003(YELLOW_ELEVATION_PIN, WHITE_ELEVATION_PIN, GREEN_ELEVATION_PIN, BLUE_ELEVATION_PIN))
            {
                elevationMotor.RPM = 1;
                elevationMotor.Mode = StepperMode.FullStepDualPhase;


                ElevationAngle = MoveAngle(elevationMotor, ElevationAngle, 90, isClockwise: false);
                Thread.Sleep(2000);
                ElevationAngle = MoveAngle(elevationMotor, ElevationAngle, 0, isClockwise: false);
                

                //PARAMS RT, DATUM, ZEIT, Längengrad, Breitengrad                
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
                int turnStep = (Revolution * angle) / 360;
                motor.Step(turnStep);

                return angle;
            }
            else if (angle == 0)
            {
                int turnStep = (Revolution * currentAngle) / 360;
                motor.Step(-turnStep);

                return 0;
            }
            else if (isClockwise)
            {
                Console.WriteLine("is clockwise");
                Console.WriteLine($"Angle: {angle} CurrentAngle: {currentAngle}");
                int turnSteps = (Revolution * angle) / 360;
                motor.Step(turnSteps);
                return angle + currentAngle;
            }
            else
            {
                Console.WriteLine("is counter clockwise");
                Console.WriteLine($"Angle: {angle} CurrentAngle: {currentAngle}");
                int turnSteps = (Revolution * angle) / 360;
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
            return (angle < 0 || angle> 360);
        }
    }
}
