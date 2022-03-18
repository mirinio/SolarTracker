using Innovative.SolarCalculator;
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
        private const int YELLOW_AZIMUT_PIN = 12;//grau
        private const int WHITE_AZIMUT_PIN = 16;//violet
        private const int GREEN_AZIMUT_PIN = 20;//rot
        private const int BLUE_AZIMUT_PIN = 21;//violet

        //Azimut stepper motor pins
        private const int YELLOW_ELEVATION_PIN = 5;//grüen
        private const int WHITE_ELEVATION_PIN = 6;//brun
        private const int GREEN_ELEVATION_PIN = 13;//gelb
        private const int BLUE_ELEVATION_PIN = 19;//blau


        public int ElevationAngle { get; private set; }
        public int AzimutAngle { get; private set; }

        public int Revolution { get; private set; }
        public int MoveAngleSteps { get; private set; }

        public Solartracker() 
        {
            ElevationAngle = 0;
            AzimutAngle = 0;
            Revolution = 4096;
            MoveAngleSteps = 10;
        }

        public void RunManual()
        {
            using (Uln2003 azimutMotor = new Uln2003(YELLOW_AZIMUT_PIN, WHITE_AZIMUT_PIN, GREEN_AZIMUT_PIN, BLUE_AZIMUT_PIN))
            using (Uln2003 elevationMotor = new Uln2003(YELLOW_ELEVATION_PIN, WHITE_ELEVATION_PIN, GREEN_ELEVATION_PIN, BLUE_ELEVATION_PIN))
            {
                azimutMotor.Mode = StepperMode.HalfStep;
                elevationMotor.Mode = StepperMode.HalfStep;
                azimutMotor.RPM = 1;
                elevationMotor.RPM = 1;

                bool start = true;

                Console.WriteLine($"Move Steps in Angle: {MoveAngleSteps}°");

                while (start)
                {
                    Console.WriteLine($"Cururent Azimut Angle: {AzimutAngle}°");
                    Console.WriteLine($"Current Elevation Angle: {ElevationAngle}°");

                    ConsoleKeyInfo keyinfo = Console.ReadKey(true);

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
                            AzimutAngle = MoveAngle(azimutMotor, AzimutAngle, MoveAngleSteps);
                            break;

                        case ConsoleKey.RightArrow:
                            Console.WriteLine("--RIGHT--");
                            AzimutAngle = MoveAngle(azimutMotor, AzimutAngle, MoveAngleSteps, isClockwise: false);
                            break;
                        case ConsoleKey.UpArrow:
                            Console.WriteLine("++UP++");
                            ElevationAngle = MoveAngle(elevationMotor, ElevationAngle, MoveAngleSteps);
                            
                            break;
                        case ConsoleKey.DownArrow:
                            Console.WriteLine("--DOWN--");
                            ElevationAngle = MoveAngle(elevationMotor, ElevationAngle, MoveAngleSteps, isClockwise: false);
                            break;
                        default:
                            Console.WriteLine("Use Arrow keys, hit Backspace to stop");
                            start = false;
                            azimutMotor.Stop();
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
                elevationMotor.Mode = StepperMode.HalfStep;

                azimutMotor.RPM = 1;
                elevationMotor.Mode = StepperMode.HalfStep;

                //Breitengrad Rho dezimal Züri
                double latitude = 47.376887;
                //Längengrad lambda dezimal Züri
                double longitude = 8.541694;

                SolarTimes solarTimes = new SolarTimes(DateTime.Now, latitude, longitude);
                ElevationAngle = SetAngle(elevationMotor,ElevationAngle, solarTimes.SolarElevation.Degrees);
                AzimutAngle = SetAngle(azimutMotor, AzimutAngle, solarTimes.SolarAzimuth.Degrees);
                //PARAMS RT, DATUM, ZEIT, Längengrad, Breitengrad                
            }
        }

        private int MoveAngle(Uln2003 motor, int currentAngle, int angle, bool isClockwise = true)
        {
            if ((isClockwise && IsOutOfRadius(currentAngle + angle)) || (!isClockwise && IsOutOfRadius(currentAngle - angle)))
            {
                return currentAngle;
            }
            if (currentAngle == 0)
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
                int turnSteps = (Revolution * angle) / 360;
                motor.Step(turnSteps);
                return angle + currentAngle;
            }
            else
            {
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
                int turnStep = (Revolution * angle) / 360;
                motor.Step(turnStep);
            }
            else if (angle == 0)
            {
                int turnStep = (Revolution * currentAngle) / 360;
                motor.Step(-turnStep);
            }
            else if (angle < currentAngle)
            {
                int currentSteps = (Revolution * currentAngle) / 360;
                int newAngleSteps = (Revolution * angle) / 360;
                int turnStepsBack = currentSteps - newAngleSteps;
                motor.Step(-turnStepsBack);
            }
            else if (angle > currentAngle)
            {
                int currentSteps = (Revolution * currentAngle) / 360;
                int newAngleSteps = (Revolution * angle) / 360;
                int turnStepsForward = newAngleSteps - currentSteps;

                motor.Step(turnStepsForward);
            }

            return angle;
        }

        private bool IsOutOfRadius(int angle)
        {
            if (angle < 0 || angle > 360)
            {
                Console.WriteLine($"angle: {angle}° is out of range 0 < x < 360");
                return true;
            }
            return false;
        }
    }
}
