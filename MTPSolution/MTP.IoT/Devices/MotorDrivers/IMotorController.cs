using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTP.IoT.Devices.MotorDrivers
{
    public interface IMotorController
    {
        void DriveForwardFull();
        void DriveBackFull();
        void DriveForwardSpeed(float speed);
        void DriveBackSpeed(float speed);
        void StopForward();
        void StopBack();
    }
}
