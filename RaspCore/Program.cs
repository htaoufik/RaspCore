using System;
using System.Drawing;
using System.Threading;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace RaspCore
{
    class Program
    {
        // Get a reference to the pin you need to use.
        // All 3 methods below are exactly equivalent
        static GpioPin ledPin = Pi.Gpio.Pin00;

        private static bool ledPinStatus = false;



        // Define the implementation of the delegate;
        static long lastISRHit = 0;
        static void ISRCallback()
        {
            long milliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            if (milliseconds - lastISRHit > 5)
            {
                lastISRHit = milliseconds;
                Console.WriteLine("Pin Activated...");
                ledPinStatus = !ledPinStatus;
                ledPin.Write(ledPinStatus);
            }
            else
            {
                Console.WriteLine($"Filtered: {milliseconds - lastISRHit}");
                lastISRHit = milliseconds;
            }
        }

        public static void TestLedBlinking()
        {
            // perform writes to the pin by toggling the isOn variable
            var isOn = false;
            for (var i = 0; i < 20; i++)
            {
                isOn = !isOn;
                ledPin.Write(isOn);
                Console.WriteLine($"Blink({i})");
                System.Threading.Thread.Sleep(500);
            }
        }

        static void Main(string[] args)
        {
            

            Engine engine = new Engine(
                    Pi.Gpio[P1.Gpio27], // A Forward
                    Pi.Gpio[P1.Gpio17], // A Backward
                    Pi.Gpio[P1.Gpio18], // A Pwm
                    Pi.Gpio[P1.Gpio23], // A Sensor
                    Pi.Gpio[P1.Gpio05], // B Forward
                    Pi.Gpio[P1.Gpio06], // B Backward
                    Pi.Gpio[P1.Gpio13], // B Pwm
                    Pi.Gpio[P1.Gpio24]  // B Sensor
                );


           /* int speed = 100;

            engine.SetSpeed(speed);
            engine.Forward();
            
            Console.ReadKey();
            Console.WriteLine($"Left Sensor count: {engine.LeftMotor.SensorCount}, Right Sensor count: {engine.RightMotor.SensorCount}");
            engine.Stop();
            return;*/

             /*int speed = 150;
             Console.WriteLine($"Start forward at {speed}");
             engine.SetSpeed(speed);
             engine.Forward();
             Console.ReadKey();
             engine.Stop();

             return;*/

            engine.MoveToPosition(50,0);
            Console.ReadKey();
            engine.Stop();
            Console.ReadKey();


            

            engine.MoveToPosition(0, 0);
            Console.ReadKey();
            engine.Stop();
            Console.ReadKey();


            return;

            engine.MoveToPosition(30, 0);
            Console.ReadKey();
            engine.Stop();
            Console.ReadKey();

            engine.MoveToPosition(0, 0);
            Console.ReadKey();
            engine.Stop();


            Console.Write("var positionList = [");
            foreach (Point p in engine.PositionList)
            {
                Console.Write($",{{ 'x':{p.X}, 'y':{p.Y} }}");
            }

            Console.WriteLine("];");
            Console.ReadKey();
        }
    }
}
