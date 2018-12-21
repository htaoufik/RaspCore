using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace RaspCore
{
    class Engine
    {

        class Constants
        {
            // http://rco.fr.nf/index.php/2016/07/03/deplacement-dun-robot/
            public const double coeffLDist = 0.52 ;
            public const double coeffRDist = 0.52;
            public const double coeffLAngl = 2.23;
            public const double coeffRAngl = 2;
        }

        private Motor _leftMotor;
        private Motor _rightMotor;

        public List<Point> PositionList;

        public void PrintSensorCount()
        {
            Console.WriteLine($"Sensor L: {_leftMotor.SensorCount} Sensor R:{_rightMotor.SensorCount}");
        }


        public Engine(
            GpioPin motorRMoveForward,
            GpioPin motorRMoveBackward,
            GpioPin motorRPwm,
            GpioPin wheelRSensor,
            GpioPin motorLMoveForward,
            GpioPin motorLMoveBackward,
            GpioPin motorLPwm,   
            GpioPin wheelLSensor
            )
        {
            _leftMotor = new Motor(motorLMoveForward, motorLMoveBackward,motorLPwm,wheelLSensor);
            _rightMotor = new Motor(motorRMoveForward, motorRMoveBackward, motorRPwm, wheelRSensor);
        }


        public void InitSensorCount()
        {
            _leftMotor.InitSensorCount();
            _rightMotor.InitSensorCount();
        }

        public void SetSpeed(int value)
        {
            _rightMotor.SetSpeed(value);
            _leftMotor.SetSpeed(value);
        }


        public void Init()
        {
            _leftMotor.Init();
            _rightMotor.Init();

            InitSensorCount();

            PositionList = new List<Point>();
        }

        public void TurnLeft()
        {
            _rightMotor.MoveForward();
            /*_leftMotor.MoveForward();
            _rightMotor.MoveBackward();*/
        }

        public void TurnRight()
        {
            _leftMotor.MoveForward();
            /*_rightMotor.MoveForward();
            _leftMotor.MoveBackward();*/
        }

        public void MoveForward()
        {

            _leftMotor.MoveForward();
            _rightMotor.MoveForward();
        }

        public void MoveBackward()
        {
            _leftMotor.MoveBackward();
            _rightMotor.MoveBackward();
        }

        public void Stop()
        {
            _leftMotor.Stop();
            _rightMotor.Stop();
        }


        //Variables allant contenir les positions du robots
        double xR = 0;
        double yR = 0;


        public void SetGoal(int x, int y)
        {
            xC = x;
            yC = y;
        }


        //Variables permettant de stocker la position de la cible du robot
        double xC = 0;
        double yC = 0;

        //Variables allant contenir les delta position et angle
        double dDist = 0;


        //Variable contenant le cap du robot
        double orientationInDeg = 0;

        //Variable contenant l'angle entre le robot et la cible
        int consigneOrientation = 0;

        //Variable permettant de savoir si la cible est à gauche ou à droite du robot.
        int signe = 1;

        //Variable contenant la distance entre le robot et la cible
        public double distanceCible = 0;

        //Variables parametrant l'asservissement en angle du robot
        double coeffP = 1;
        double coeffD = 1.0;

        //Variables utilisées pour asservir le robot
        double erreurAngle = 0;
        double erreurPre = 0;
        double deltaErreur = 0;

        //Variables représentant les commandes à envoyer aux moteurs
        int cmdG = 0;//Commande gauche
        int cmdD = 0; //commande droite

        private double dAnglInDeg;

        void Log(string log)
        {
            Console.WriteLine(log);
        }



        public void Loop()
        {
            Log("-------- LOOP -------------");
            Log($"SensorCount L: {_leftMotor.SensorCount } R: {_rightMotor.SensorCount} ");

            // Chemin parcouru pendant le dernier intervalle de temps
            dDist = (Constants.coeffLDist * _leftMotor.SensorCount + Constants.coeffRDist * _rightMotor.SensorCount) / 2.0;

            // L'angle augmente quand on tourne à gauche ( on tourne à gauche quand la roue droite avance )
            //dAngl = Constants.coeffRAngl * _rightMotor.SensorCount - Constants.coeffLAngl * _leftMotor.SensorCount;
            //dAngl = dAngl % 360;


            double deltaDifn = (Constants.coeffLDist * _leftMotor.SensorCount) -
                               (Constants.coeffRDist * _rightMotor.SensorCount);

            int L = 15; // Largeur en cm entre les deux roues

            const double RAD_TO_DEG = 180 / Math.PI;
            const double DEG_TO_RAD =  Math.PI /180;

            double dAnglInRad = -(deltaDifn / L);
            dAnglInDeg = dAnglInRad * RAD_TO_DEG;

            Log($"Delta since last loop, Distance: {dDist} Angle: {dAnglInDeg} ");

            double deltaXn = dDist * Math.Cos(orientationInDeg * DEG_TO_RAD);
            double deltaYn = dDist * Math.Sin(orientationInDeg * DEG_TO_RAD);

            // Math.PI / 180 degres vers rad
            //Actualisation de la position du robot en xy et en orientation
            xR += deltaXn;
            yR += deltaYn;

            orientationInDeg += dAnglInDeg;
            orientationInDeg = orientationInDeg % 360;

            Log($"Current position: xR: {xR} yR: {yR}");

            PositionList.Add(new Point((int)xR, (int)yR));


            double distanceCibleY = yC - yR;
            double distanceCibleX = xC - xR;

            //On calcule la distance séparant le robot de sa cible
            // As we use Pow2 we don't care if negative
            distanceCible = Math.Sqrt( Math.Pow((distanceCibleX),2) + Math.Pow((distanceCibleY), 2) );

            


            double alpha = Math.Acos(Math.Abs(distanceCibleX) / distanceCible) * (180 / Math.PI);
            // We must check if C is left or right from the robot
            bool isRight = distanceCibleY < 0;
            bool isBehind = distanceCibleX < 0;

            //On calcule l'angle de la cible en considérant que le robot est à angle 0
            double angleVersCibleInDeg = 0;
            if (isBehind)
            {
                if (isRight)
                {
                    angleVersCibleInDeg = 180 + alpha;
                }
                else
                {
                    angleVersCibleInDeg = 180 - alpha;
                }  
            }
            else
            {
                angleVersCibleInDeg = alpha;
            }

            erreurAngle = angleVersCibleInDeg - (orientationInDeg );

            Log($"ErreurAngle: {erreurAngle}");

            Log($"From target: Distance: {distanceCible} Angle:{erreurAngle}");



            // On calcule la consigne pour l'orientation
            // Coeff proportionel pour l'orientation
            int pAngle = 4;

            // si l'angle est supérieur à 180 degrés ça vaut le coup de tourner à droite 
            if (erreurAngle > 180)
            {
                erreurAngle = erreurAngle - 360;
            }

            // on ramène la consigne à 255 (vitesse max ) : Règle de trois * 255/360
            consigneOrientation = Math.Abs((int) (255 * erreurAngle* pAngle) / 360) ;


            // Et maintenant on calcule la consigne pour la distance
            // on applique un coeff pour qu'à 1 cm on soit à la vitesse 100 
            // Si la cible est à droite il faut ralentir la vitesse de la roue droite

            int pDirection = 10;
            int maxSpeedFromDistance = (int)Math.Min(distanceCible* pDirection, 255);

            // On arrête quand on se raproche de 10 !
            if (distanceCible <= 10)
            {
                maxSpeedFromDistance = 0;
            }

            Log($"Consigne distance: {maxSpeedFromDistance} direction: {consigneOrientation} " + (isRight ? "R" : "L"));

            int consigneRight = maxSpeedFromDistance - (isRight ? consigneOrientation : 0);
            int consigneLeft =  maxSpeedFromDistance - (isRight ? 0  : consigneOrientation);

            consigneRight = Math.Max(consigneRight, 0);
            consigneLeft = Math.Max(consigneLeft, 0);

            //Calcul de la différence entre l'erreur au coup précédent et l'erreur actuelle.
            deltaErreur = erreurAngle - erreurPre;

            //Mise en mémoire de l'erreur actuelle
            erreurPre = erreurAngle;

            // We must invert it as to turn right we set speed on left etc.
            _leftMotor.SetSpeed(consigneLeft);
            _rightMotor.SetSpeed(consigneRight);

            Log($"Speed L: {_leftMotor.CurrentSpeed} Speed R:{_rightMotor.CurrentSpeed}");


            InitSensorCount();
        }
    }
}
