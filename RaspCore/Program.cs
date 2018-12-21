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

        public static void MoveMotor()
        {
            var pin = Pi.Gpio[P1.Gpio18];
            pin.PinMode = GpioPinDriveMode.PwmOutput;
            pin.PwmMode = PwmMode.MarkSign;
            pin.PwmClockDivisor = 1;
            pin.PwmRegister = 850;

            var in1pin = Pi.Gpio.Pin03;
            in1pin.PinMode = GpioPinDriveMode.Output;
            var in2pin = Pi.Gpio.Pin04;
            in2pin.PinMode = GpioPinDriveMode.Output;

            // perform writes to the pin by toggling the isOn variable
            var isOn = false;
            for (var i = 0; i < 20; i++)
            {
                isOn = !isOn;
                in1pin.Write(isOn);
                
                Console.WriteLine($"Motor ({i})");
                System.Threading.Thread.Sleep(500);
            }

            in1pin.Write(false);
            in2pin.Write(false);
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Gpio Interrupts");

            Engine engine = new Engine(
                    Pi.Gpio[P1.Gpio17], // A Forward
                    Pi.Gpio[P1.Gpio27], // A Backward
                    Pi.Gpio[P1.Gpio18], // A Pwm
                    Pi.Gpio[P1.Gpio22], // A Sensor
                    Pi.Gpio[P1.Gpio05], // B Forward
                    Pi.Gpio[P1.Gpio06], // B Backward
                    Pi.Gpio[P1.Gpio13], // B Pwm
                    Pi.Gpio[P1.Gpio12]  // B Sensor
                );

            engine.Init();

            /* engine.MoveForward();
             System.Threading.Thread.Sleep(2000);
             engine.Stop();
             engine.PrintSensorCount();
             engine.InitSensorCount();
             System.Threading.Thread.Sleep(1000);

             engine.MoveBackward();
             System.Threading.Thread.Sleep(1000);
             engine.Stop();
             engine.PrintSensorCount();
             engine.InitSensorCount();
             System.Threading.Thread.Sleep(1000);
             */

            /*  engine.SetSpeed(255);
              engine.TurnLeft();
              System.Threading.Thread.Sleep(2600);
              engine.Stop();
              engine.PrintSensorCount();
              engine.InitSensorCount();
              */


            // Create an AutoResetEvent to signal the timeout threshold in the
            // timer callback has been reached.
            var autoEvent = new AutoResetEvent(false);

            engine.MoveForward();


            System.Threading.Timer timer = null;
            timer = new System.Threading.Timer((obj) =>
                {
                    engine.Loop();
                },
                null, 20, 100);

            engine.SetGoal(30,0);

            
            Console.ReadKey();

            engine.SetGoal(30, 30);

            Console.ReadKey();

            engine.Stop();


            Console.Write("var positionList = [");
            foreach (Point p in engine.PositionList)
            {
                Console.Write($",{{ 'x':{p.X}, 'y':{p.Y} }}");
            }

            Console.WriteLine("];");

        }
    }
}
