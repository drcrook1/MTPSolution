using MTP.IoT.Devices.MotorDrivers;
using MTP.IoT.Devices.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTP.CarLab.CarLogic
{
    public class CarBrain
    {
        public enum CarState
        {
            Unknown,
            Stopped,
            DrivingForward,
            DrivingBackword,
            TurningRight,
            TurningLeft
        }
        public CarState CurrentState;
        public GpioMotorController motorA;
        public GpioMotorController motorB;
        //MotorA Pin Definitions
        private const int motorAForwardPin = 24;
        private const int motorABackPin = 23;
        //MotorB Pin Definitions
        private const int motorBForwardPin = 12;
        private const int motorBBackPin = 16;
        //Distance Sensor Pin Definitions
        private const int triggerPin = 22;
        private const int echoPin = 27;

        private HCSR04 distanceSensor;
        /// <summary>
        /// Initializes the Car Brain
        /// </summary>
        public CarBrain()
        {
            CurrentState = CarState.Unknown;
            //initialize our motors.
            motorA = new GpioMotorController(motorAForwardPin, motorABackPin);
            motorB = new GpioMotorController(motorBForwardPin, motorBBackPin);
            distanceSensor = new HCSR04(triggerPin, echoPin);
            //initialize first state.
            //this.InitializeFirstState();
            while(true)
            {
                this.Drive();
            }
        }

        private void Drive()
        {
            var dist = distanceSensor.GetDistance();
            if (dist > 50 && this.CurrentState != CarState.DrivingForward)
            {
                this.DriveForward();
            }
            else if (dist < 50 && this.CurrentState == CarState.DrivingForward)
            {
                this.Stop();
            }
            else
            {
                this.TurnLeft();
            }
            
        }

        private void DriveForward()
        {
            motorA.DriveForwardFull();
            motorB.DriveForwardFull();
            this.CurrentState = CarState.DrivingForward;            
        }

        private void Stop()
        {
            motorA.StopBack();
            motorA.StopForward();
            motorB.StopForward();
            motorB.StopBack();
            this.CurrentState = CarState.Stopped;
        }

        private void TurnRight()
        {
            motorA.StopForward();
            motorB.StopForward();
            motorA.DriveForwardFull();
            this.CurrentState = CarState.TurningRight;
            System.Threading.SpinWait.SpinUntil(() => { return false; }, 50);
            motorA.StopForward();
            this.CurrentState = CarState.Stopped;
        }

        private void TurnLeft()
        {
            motorA.StopForward();
            motorB.StopForward();
            motorB.DriveForwardFull();
            this.CurrentState = CarState.TurningLeft;
            System.Threading.SpinWait.SpinUntil(() => { return false; }, 50);
            motorB.StopForward();
            this.CurrentState = CarState.Stopped;
        }

    }
}
