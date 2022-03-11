using System;
using System.Collections.Generic;
using System.Device.Pwm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solartracker
{
    internal class BrokenServo
    {
        internal string Id { get; private set; }
        internal int CurrentAngle { get; private set; }
        internal PwmChannel Pwm { get; private set; }

        //360° ~= 1050ms workaround for broken servo
        //mindest winkel 1050 * 1°/360° ~= 20° und nicht 1° leider
        //Bewegung ist über zeit nicht linear 

        internal int PeriodMs = 1038;

        public BrokenServo(string id, PwmChannel pwm)
        {
            Pwm = pwm;
            Id = id;
        }

        internal void SetAngle(int angle)
        {
            if (angle < 0 || angle > 360)
            {
                return;
            }

            if (angle < CurrentAngle)
            {
                Pwm.DutyCycle = 0.03;
            }
            else if (angle > CurrentAngle)
            {
                Pwm.DutyCycle = 0.1;
            }


            //45
            //Turn(55);

            //90
            Turn(155);

            //135
            //Turn(310);

            //180
            //Turn(462);

            //225
            //Turn(585);

            //270
            //Turn(752);

            //315
            //Turn(885);

            //360
            //Turn(PeriodMs);

            //int sleepAngle = (1050 * angle) / 360;
            CurrentAngle = angle;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Servo {Id}");
            sb.AppendLine($"Angle {CurrentAngle}");
            sb.AppendLine();
            return sb.ToString();
        }


        private void Turn(int sleepMs)
        {
            Pwm.Start();
            Thread.Sleep(sleepMs);
            Pwm.Stop();
        }
    }
}
