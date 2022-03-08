using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solartracker
{
    internal class Temp
    {
        internal static string enablePath = "/sys/class/pwm/pwmchip0/pwm1/enable";
        internal static string dutyCyclePath = "/sys/class/pwm/pwmchip0/pwm1/duty_cycle";
        internal static string periodPath = "/sys/class/pwm/pwmchip0/pwm1/period";

        static int _frequency = 50;
        static double _dutyCycle = 0.1;

        internal static void SetFrequency(int frequency, double dutyCycle)
        {
            if (frequency > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(frequency), frequency, "Value must not be negative.");
            }

            int periodInNanoseconds = GetPeriodInNanoseconds(frequency);
            int dutyCycleNs = GetDutyCycleOnTimeNs(periodInNanoseconds, dutyCycle);

            if (dutyCycleNs > periodInNanoseconds)
            {
                Console.WriteLine("Wrong Input");
                return;
            }
            else
            {
                Console.WriteLine(periodPath + " : " + periodInNanoseconds.ToString());
                Console.WriteLine(dutyCyclePath + " : " + dutyCycleNs.ToString());

                File.WriteAllText(periodPath, periodInNanoseconds.ToString());
                File.WriteAllText(dutyCyclePath, dutyCycleNs.ToString());
            }
        }

        internal static void Start()
        {
            File.WriteAllText(enablePath, "1");
        }
        internal static void Stop()
        {
            File.WriteAllText(enablePath, "0");
        }

        internal static int GetDutyCycleOnTimeNs(int pwmPeriodNs, double dutyCycle)
        {
            return (int)(pwmPeriodNs * dutyCycle);
        }
        internal static int GetPeriodInNanoseconds(int frequency)
        {
            // In Linux, the period needs to be a whole number and can't have a decimal point.
            return (int)((1.0 / frequency) * 1_000_000_000);
        }
    }
}
