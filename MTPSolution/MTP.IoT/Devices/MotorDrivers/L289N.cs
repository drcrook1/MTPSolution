using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTP.IoT.Devices.MotorDrivers
{
    public class L289N
    {

        private IMotorController[] _motors;
        public IMotorController[] Motors { get; set; }

        public L289N ()
        {

        }

        public void DriveMotor(int index, TimeSpan time)
        {

        }
    }
}
