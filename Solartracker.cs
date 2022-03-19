using Innovative.SolarCalculator;
using Iot.Device.Nmea0183;
using Iot.Device.Nmea0183.Sentences;
using Iot.Device.Uln2003;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solartracker
{
    internal class Solartracker
    {
        //Elevation stepper motor pins        Kabel
        private const int AZIMUT_PIN_1 = 12;//grau
        private const int AZIMUT_PIN_2 = 16;//violet
        private const int AZIMUT_PIN_3 = 20;//rot
        private const int AZIMUT_PIN_4 = 21;//violet

        //Azimut stepper motor pins
        private const int ELEVATION_PIN_1 = 5;//grüen
        private const int ELEVATION_PIN_2 = 6;//brun
        private const int ELEVATION_PIN_3 = 13;//gelb
        private const int ELEVATION_PIN_4 = 19;//blau


        public int ElevationAngle { get; private set; }
        public int AzimutAngle { get; private set; }

        public int Revolution { get; private set; }
        public int MoveAngleSteps { get; private set; }
        
        public StepperMode MotorStepperMode { get; private set; }
        public short MotorRPM { get; private set; }   
        public Solartracker() 
        {
            // start angle
            ElevationAngle = 0; 
            AzimutAngle = 0;
            //Revolution for halfstep
            Revolution = 4096;
            // steps in degrees for manual move on click
            MoveAngleSteps = 10;
            MotorStepperMode = StepperMode.HalfStep;
            MotorRPM = 1;
        }

        public void RunManual()
        {
            using (Uln2003 azimutMotor = new Uln2003(AZIMUT_PIN_1, AZIMUT_PIN_2, AZIMUT_PIN_3, AZIMUT_PIN_4))
            using (Uln2003 elevationMotor = new Uln2003(ELEVATION_PIN_1, ELEVATION_PIN_2, ELEVATION_PIN_3, ELEVATION_PIN_4))
            {
                azimutMotor.Mode = MotorStepperMode;
                elevationMotor.Mode = MotorStepperMode;
                azimutMotor.RPM = MotorRPM;
                elevationMotor.RPM = MotorRPM;

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
            using(var gpsPort = new SerialPort("/dev/ttyAMA0"))
            using (Uln2003 azimutMotor = new Uln2003(AZIMUT_PIN_1, AZIMUT_PIN_2, AZIMUT_PIN_3, AZIMUT_PIN_4))
            using (Uln2003 elevationMotor = new Uln2003(ELEVATION_PIN_1, ELEVATION_PIN_2, ELEVATION_PIN_3, ELEVATION_PIN_4))
            {
                elevationMotor.RPM = MotorRPM;
                elevationMotor.Mode = MotorStepperMode;

                azimutMotor.RPM = MotorRPM;
                elevationMotor.Mode = MotorStepperMode;


                gpsPort.NewLine = "\r\n";
                gpsPort.Open();

                // Device streams continuously and therefore most of the time we would end up in the middle of the line
                // therefore ignore first line so that we align correctly
                gpsPort.ReadLine();

                DateTimeOffset lastMessageTime = DateTimeOffset.UtcNow;
                bool gotRmc = false;
                while (!gotRmc)
                {
                    string line = gpsPort.ReadLine();
                    TalkerSentence? sentence = TalkerSentence.FromSentenceString(line, out _);
                    
                    if (sentence == null)
                    {
                        continue;
                    }

                    object? typed = sentence.TryGetTypedValue(ref lastMessageTime);
                    if (typed == null)
                    {
                        Console.WriteLine($"Sentence identifier `{sentence.Id}` is not known.");
                    }
                    else if (typed is RecommendedMinimumNavigationInformation rmc)
                    {
                        gotRmc = true;

                        if (rmc.Position.ContainsValidPosition())
                        {
                            Console.WriteLine($"location: {rmc.Position}");

                            SolarTimes solarTimes = new SolarTimes(DateTime.Now, rmc.Position.Latitude, rmc.Position.Longitude);
                            AzimutAngle = SetAngle(azimutMotor, AzimutAngle, solarTimes.SolarAzimuth.Degrees);
                            ElevationAngle = SetAngle(elevationMotor, ElevationAngle, solarTimes.SolarElevation.Degrees);
                        }
                        else
                        {
                            Console.WriteLine($"Location not found");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Sentence of type `{typed.GetType().FullName}` not handled.");
                    }
                }              
            }
        }

        public void RunGPSDemo(DateTime time, double latitute, double longitude)
        {
            using (Uln2003 azimutMotor = new Uln2003(AZIMUT_PIN_1, AZIMUT_PIN_2, AZIMUT_PIN_3, AZIMUT_PIN_4))
            using (Uln2003 elevationMotor = new Uln2003(ELEVATION_PIN_1, ELEVATION_PIN_2, ELEVATION_PIN_3, ELEVATION_PIN_4))
            {
                elevationMotor.RPM = MotorRPM;
                elevationMotor.Mode = MotorStepperMode;

                azimutMotor.RPM = MotorRPM;
                elevationMotor.Mode = MotorStepperMode;

                SolarTimes solarTimes = new SolarTimes(time, latitute, longitude);
                Console.WriteLine($"Elevation: {solarTimes.SolarElevation.Degrees}°");
                Console.WriteLine($"Azimuth: {solarTimes.SolarAzimuth.Degrees}°");
                AzimutAngle = SetAngle(azimutMotor, AzimutAngle, solarTimes.SolarAzimuth.Degrees);
                ElevationAngle = SetAngle(elevationMotor, ElevationAngle, solarTimes.SolarElevation.Degrees);

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
            int currentSteps = (Revolution * currentAngle) / 360;
            int newAngleSteps = (Revolution * angle) / 360;

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
                int turnStepsBack = currentSteps - newAngleSteps;
                motor.Step(-turnStepsBack);
            }
            else if (angle > currentAngle)
            {
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
