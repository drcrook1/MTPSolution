using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace MTP.IoT.Devices.MotorDrivers
{
    public class GpioMotorController : IMotorController
    {
        private GpioPin PinForward;
        private GpioPin PinBack;

        public GpioMotorController(int pinForward, int pinBack)
        {
            GpioController controller = GpioController.GetDefault();
            this.PinForward = controller.OpenPin(pinForward);
            this.PinForward.SetDriveMode(GpioPinDriveMode.Output);
            this.PinBack = controller.OpenPin(pinBack);
            this.PinBack.SetDriveMode(GpioPinDriveMode.Output);
        }

        public void DriveBackFull()
        {
            PinForward.Write(GpioPinValue.Low);
            PinBack.Write(GpioPinValue.High);
        }

        public void DriveForwardFull()
        {
            PinBack.Write(GpioPinValue.Low);
            PinForward.Write(GpioPinValue.High);            
        }

        public void StopForward()
        {
            PinForward.Write(GpioPinValue.Low);
        }

        public void StopBack()
        {
            PinBack.Write(GpioPinValue.Low);
        }

        public void DriveForwardSpeed(float speed)
        {
            throw new NotImplementedException();
        }

        public void DriveBackSpeed(float speed)
        {
            throw new NotImplementedException();
        }
    }
}
