using MTP.IoT.Devices.MotorDrivers;
using MTP.IoT.Devices.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTP.CarLab.CarLogic
{
    /// <summary>
    /// This is a class.  A class can be turned into an object of this type.
    /// In this example, we can create an object of the type "CarBrain".
    /// The program creates a car brain.
    /// When a car brain is created it starts driving.
    /// </summary>
    public class CarBrain
    {
        //possible states the car can be in.
        public enum CarState
        {
            Unknown,
            Stopped,
            DrivingForward,
            DrivingBackword,
            TurningRight,
            TurningLeft
        }
        //representation of the current state it is in.
        public CarState CurrentState;
        //representation of motorA (this could be left or right)
        public GpioMotorController motorA;
        //representation of motorB (this could be left or right)
        public GpioMotorController motorB;
        //MotorA Pin Definitions
        //switching these will cause the motor to spin the opposite direction
        //when calling forward or back.
        private const int motorAForwardPin = 24;
        private const int motorABackPin = 23;
        //MotorB Pin Definitions
        private const int motorBForwardPin = 12;
        private const int motorBBackPin = 16;
        //Distance Sensor Pin Definitions
        private const int triggerPin = 22;
        private const int echoPin = 27;
        //representation of the distance sensor.
        private HCSR04 distanceSensor;
        /// <summary>
        /// Initializes the Car Brain
        /// This code runs when a new brain is created.
        /// </summary>
        public CarBrain()
        {
            //sets the first state of the car to unknown.
            CurrentState = CarState.Unknown;
            //initialize our motors.
            this.Initialize();
                        
            while(true) //do stuff forever.
            {
                //drive, this is where main logic happens.
                this.Drive();
            }
        }

        private void Initialize()
        {
            //initialize the motors.
            motorA = new GpioMotorController(motorAForwardPin, motorABackPin);
            motorB = new GpioMotorController(motorBForwardPin, motorBBackPin);
            //initialize our senses.
            distanceSensor = new HCSR04(triggerPin, echoPin);
            this.Stop();
            this.CurrentState = CarState.Stopped;
        }
        //This is the main logic for the brain.  This is where the thinking and acting and control happens.
        private void Drive()
        {
            motorA.DriveForwardFull();

            ////get distance with the sensor for how far we are away from things.
            //var dist = distanceSensor.GetDistance();
            ////if we are more than 50 cm and not currently driving forward, lets drive forward.
            //if (dist > 50 && this.CurrentState != CarState.DrivingForward)
            //{
            //    this.DriveForward();
            //}            
            ////otherwise, if we are close to something and driving forward, lets stop.
            //else if (dist < 50 && this.CurrentState == CarState.DrivingForward)
            //{
            //    this.Stop();
            //}//in all other instances, turn left.
            //else
            //{
            //    this.TurnLeft();
            //}

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
            System.Threading.SpinWait.SpinUntil(() => { return false; }, 100);
            motorA.StopForward();
            this.CurrentState = CarState.Stopped;
        }

        private void TurnLeft()
        {
            motorA.StopForward();
            motorB.StopForward();
            motorB.DriveForwardFull();
            this.CurrentState = CarState.TurningLeft;
            System.Threading.SpinWait.SpinUntil(() => { return false; }, 100);
            motorB.StopForward();
            this.CurrentState = CarState.Stopped;
        }

    }
}
