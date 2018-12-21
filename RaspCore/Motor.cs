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
            public const int SensorFilterThresholdInMs = -1;
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

            _pwmPin.StartSoftPwm(0, 255);
            SetSpeed(0);

            _sensorPin.PinMode = GpioPinDriveMode.Input;
            _sensorPin.RegisterInterruptCallback(EdgeDetection.RisingEdge, this.SensorInterupt);



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
            CurrentSpeed = value;
            _pwmPin.SoftPwmValue = value;
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
