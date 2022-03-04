using System;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServoMotor servoMotor = new ServoMotor(
            PwmChannel.Create(0, 0, 50),
            180,
            900,
            2100);

            servoMotor.Start();  // Enable control signal.

            // Move position.
            servoMotor.WriteAngle(0); // ~0.9ms; Approximately 0 degrees.
            servoMotor.WritePulseWidth(90); // ~1.5ms; Approximately 90 degrees.
            servoMotor.WritePulseWidth(180); // ~2.1ms; Approximately 180 degrees.

            servoMotor.Stop(); // Disable control signal.
        }
    }
}