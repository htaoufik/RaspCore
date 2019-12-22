using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Unosquare.RaspberryIO.Gpio;

namespace RaspCore
{
    class Motor
    {

        class Constants
        {
            public const int SensorFilterThresholdInMs = -1; // -1 means deactivated, no filtering
        }


        public enum Direction
        {
            None,
            Forward,
            Backward
        }

        public Direction CurrentDirection { get; set; }
        public int CurrentSpeed { get; set; }
        public int SensorCount { get; set; }
        private long _lastSensorHitInMilliseconds;
        private GpioPin _pwmPin;
        private GpioPin _forwardPin;
        private GpioPin _backwardPin;
        private GpioPin _sensorPin;

        private const int _maxSpeed = 255;
        private const int _minSpeed = 100;



        public Motor(GpioPin forwardPin, GpioPin backwardPin, GpioPin pwmPin, GpioPin sensorPin)
        {
            _forwardPin = forwardPin;
            _backwardPin = backwardPin;
            _sensorPin = sensorPin;
            _pwmPin = pwmPin;

        }



        public void Init()
        {
            _forwardPin.PinMode = GpioPinDriveMode.Output;
            _backwardPin.PinMode = GpioPinDriveMode.Output;


            // As we have a bit too much power for our motors, we set the pwm to 500 max but limit the range to 255 max
            _pwmPin.StartSoftPwm(0, 500); 
           

            //_pwmPin.PinMode = GpioPinDriveMode.Output;

            //_pwmPin.PinMode = GpioPinDriveMode.PwmOutput;
            //_pwmPin.PwmMode = PwmMode.Balanced;


            SetSpeed(0);

            _sensorPin.PinMode = GpioPinDriveMode.Input;
            _sensorPin.InputPullMode = GpioPinResistorPullMode.PullUp;
            _sensorPin.RegisterInterruptCallback(EdgeDetection.RisingAndFallingEdges, this.SensorInterupt);

        }

        public void InitSensorCount()
        {
            SensorCount = 0;
        }


        public void MoveForward()
        {
            _backwardPin.Write(false);
            _forwardPin.Write(true);
            CurrentDirection = Direction.Forward;
        }

        public void MoveBackward()
        {
            _backwardPin.Write(true);
            _forwardPin.Write(false);
            CurrentDirection = Direction.Backward;
        }

        public void Stop()
        {
            _backwardPin.Write(false);
            _forwardPin.Write(false);
            CurrentDirection = Direction.None;
        }

        public void SetSpeed(int value)
        {

           /* if( value == 0 )
            {
                _pwmPin.Write(false);
            }
            else
            {
                _pwmPin.Write(true);
            }*/


            CurrentSpeed = value;
            if (CurrentSpeed > 0)
            { 
                CurrentSpeed = Math.Max(CurrentSpeed, _minSpeed);
            }

            // Limit to max allowed speed
            if (CurrentSpeed > _maxSpeed)
            {
                CurrentSpeed = _maxSpeed;
            }


            _pwmPin.SoftPwmValue = CurrentSpeed;
            //_pwmPin.PwmRegister = value;
        }

        /// <summary>
        /// Function triggered on sensor  hit
        /// </summary>
        private void SensorInterupt()
        {
            long milliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            if (milliseconds - _lastSensorHitInMilliseconds > Constants.SensorFilterThresholdInMs)
            {
                _lastSensorHitInMilliseconds = milliseconds;

                switch (CurrentDirection)
                {
                    case Direction.Forward:
                        SensorCount++;
                        break;
                    case Direction.Backward:
                        SensorCount--;
                        break;
                }
            }
            else
            {
                Console.WriteLine($"Filtered: {milliseconds - _lastSensorHitInMilliseconds}");
                _lastSensorHitInMilliseconds = milliseconds;
            }
        }
    }
}
