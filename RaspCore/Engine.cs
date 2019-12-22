using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using Unosquare.RaspberryIO.Gpio;
using Unosquare.Swan;

namespace RaspCore
{
    class Engine
    {
        private enum State
        {
            Unknown, 
            MovingToGoal, 
            Stopped
        };

        // Pour le moteur du moment:
        // Taille de roue : 65 mm,  Ratiode reduction = Ratio 34.02, nombre de tick par tour 6 soit 6*34.02 = 204.12
        // nombre de cm par tick = 6.5 / 204.12 = 0.032


        class Constants
        {
            // http://rco.fr.nf/index.php/2016/07/03/deplacement-dun-robot/
            public const double CoeffLDist = 0.032; // cm par tick de capteur rouge gauche
            public const double CoeffRDist = 0.032; // cm par tick de capteur roue droite
            public const double RAD_TO_DEG = 180 / Math.PI;
            public const double DEG_TO_RAD = Math.PI / 180;
        }




        private State _currentState;

        private readonly Motor _leftMotor;
        private readonly Motor _rightMotor;

        public Motor LeftMotor { get { return _leftMotor; } }
        public Motor RightMotor { get { return _rightMotor; } }


        private readonly Timer _goalTimer;
        private readonly Stopwatch _goalWatch = new Stopwatch();


        public List<Point> PositionList;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="motorRMoveForward"></param>
        /// <param name="motorRMoveBackward"></param>
        /// <param name="motorRPwm"></param>
        /// <param name="wheelRSensor"></param>
        /// <param name="motorLMoveForward"></param>
        /// <param name="motorLMoveBackward"></param>
        /// <param name="motorLPwm"></param>
        /// <param name="wheelLSensor"></param>
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

            _goalTimer = new Timer((obj) =>
                {
                    Loop();
                },
                null, Timeout.Infinite, Timeout.Infinite);

            Init();
        }


        private void InitSensorCount()
        {
            _leftMotor.InitSensorCount();
            _rightMotor.InitSensorCount();
        }

        private void Init()
        {
            _leftMotor.Init();
            _rightMotor.Init();

            InitSensorCount();
            PositionList = new List<Point>();

            Stop();
        }

        public void TurnLeft()
        {
            _leftMotor.MoveForward();
            _rightMotor.MoveBackward();
        }

        public void SetSpeed(int value)
        {
            _rightMotor.SetSpeed(value);
            _leftMotor.SetSpeed(value);
        }

        public void TurnRight()
        {
            _rightMotor.MoveForward();
            _leftMotor.MoveBackward();
        }

        public void Forward()
        {
            _leftMotor.MoveForward();
            _rightMotor.MoveForward();
        }

        public void Backward()
        {
            _leftMotor.MoveBackward();
            _rightMotor.MoveBackward();
        }

        public void Stop()
        {
            _leftMotor.Stop();
            _rightMotor.Stop();
            _leftMotor.SetSpeed(0);
            _rightMotor.SetSpeed(0);
            _goalTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _currentState = State.Stopped;
        }

        //Variables allant contenir les positions du robots
        private double _xR = 0;
        private double _yR = 0;

        //Variable contenant le cap du robot
        private double _currentAngleInRad = 0;

        //Variables permettant de stocker la position de la cible du robot
        private double _xC = 0;
        private double _yC = 0;

        public void MoveToPosition(int x, int y)
        {
            _xC = x;
            _yC = y;

            if( _currentState != State.MovingToGoal )
            { 
                Forward();
                _goalTimer.Change(20, 50);
                _goalWatch.Start();
                _currentState = State.MovingToGoal;
            }
        }

        //Variables utilisées pour asservir le robot
        double erreurPre = 0;
        double deltaErreur = 0;

        void Log(string log)
        {
            Console.WriteLine(log);
        }

        private void Loop()
        {
            _goalWatch.Stop();
            Log($"-------- LOOP { _goalWatch.ElapsedMilliseconds }------------- ");
            Log($"SensorCount L: {_leftMotor.SensorCount } R: {_rightMotor.SensorCount} ");
            _goalWatch.Restart();

            int L = 19; // Largeur en cm entre les deux roues
            


            // Algo: http://manubatbat.free.fr/doc/positionning/node5.html

            // Chemin parcouru pendant le dernier intervalle de temps en cm
            double dLeft = Constants.CoeffLDist * _leftMotor.SensorCount;
            double dRight = Constants.CoeffRDist * _rightMotor.SensorCount;
            double dDist = (dLeft + dRight) / 2.0;
            double deltaDifn = dRight - dLeft;

            // Quand la différence entre les deux roue est de 2PiR ( ici R = L ) on a fait un tour complet soit un angle de 2pi
            // pour deltaDif l'angle parcouru est donc : deltaDif/2piL * 2pi soit deltaDif/L
            double dAnglInRad = deltaDifn / L;

            double previousAngleInRad = _currentAngleInRad;
            _currentAngleInRad += dAnglInRad;

            // On garde l'angle en 0 et 2Pi
            _currentAngleInRad = _currentAngleInRad % (2 * Math.PI);

            // Si l'angle est negatif on le ramène en positif
            if (_currentAngleInRad < 0)
            {
                _currentAngleInRad = (2 * Math.PI) + _currentAngleInRad;
            }


            Log($"Delta since last loop, Distance: {dDist} Angle: {dAnglInRad * Constants.RAD_TO_DEG} ");

            // La distance a été parcouru avec l'angle précédent. ( Estimation de parcour en ligne droite )
            double deltaXn = dDist * Math.Cos(previousAngleInRad);
            double deltaYn = dDist * Math.Sin(previousAngleInRad);

            // Math.PI / 180 degres vers rad
            //Actualisation de la position du robot en xy et en orientation
            _xR += deltaXn;
            _yR += deltaYn;

            PositionList.Add(new Point((int)_xR, (int)_yR));

            double distanceCibleY = _yC - _yR;
            double distanceCibleX = _xC - _xR;

            // On calcule la distance séparant le robot de sa cible
            // Avec Pythagore
            // As we use Pow2 we don't care if negative
            double distanceCible = Math.Sqrt( Math.Pow((distanceCibleX),2) + Math.Pow((distanceCibleY), 2) );

            // Alpha est l'angle entre la cible et l'axe des abscice
            double alphaInRad = Math.Atan(Math.Abs(distanceCibleY) / Math.Abs(distanceCibleX));

            //double alpha = Math.Acos(Math.Abs(distanceCibleX) / distanceCible) * (180 / Math.PI);

            double angleCibleInRad = 0;
            if (distanceCibleX > 0 && distanceCibleY > 0)
            {
                angleCibleInRad = alphaInRad;
            }
            else if (distanceCibleX >= 0 && distanceCibleY < 0)
            {
                angleCibleInRad = 2* Math.PI - alphaInRad;
            }
            else if (distanceCibleX < 0 && distanceCibleY >= 0)
            {
                angleCibleInRad =  Math.PI - alphaInRad;
            }
            else if (distanceCibleX < 0 && distanceCibleY < 0)
            {
                angleCibleInRad =  Math.PI + alphaInRad;
            }

            Log($"Current position: xR: {_xR} yR: {_yR} Angle: {_currentAngleInRad * Constants.RAD_TO_DEG} Angle Cible: {angleCibleInRad * Constants.RAD_TO_DEG}");


            // We must check if C is left or right from the robot
            bool isRight = false;


            // si l'angle est supérieur à Pi radian ça vaut le coup de tourner à droite 
            double erreurAngleInRad = angleCibleInRad - _currentAngleInRad;

            if (erreurAngleInRad < 0)
            {
                isRight = true;
                erreurAngleInRad = Math.Abs(erreurAngleInRad);
                // The error is negative, by default it is right, unless the angle is bigger than 180
                if (erreurAngleInRad > Math.PI)
                {
                    isRight = false;
                    erreurAngleInRad = (2 * Math.PI)- erreurAngleInRad;
                }
            }
            // It is positif, so by default it is left unless the angle is bigger than 180
            else if (erreurAngleInRad > Math.PI)
            {
                isRight = true;
                erreurAngleInRad = (2 * Math.PI) - erreurAngleInRad;
            }


            Log($"From target: Distance: {distanceCible} Erreur Angle:{erreurAngleInRad * Constants.RAD_TO_DEG}" + (isRight?"Right":"Left") );


            // On calcule la consigne pour l'orientation
            // Coeff proportionel pour l'orientation
            float pAngle = 3;

            // on ramène la consigne à 255 (vitesse max ) : Règle de trois * 255/2Pi
            int consigneOrientation =(int) Math.Round( Math.Abs( (255 * erreurAngleInRad* pAngle) / (Math.PI *2) ));


            // Et maintenant on calcule la consigne pour la distance
            // on applique un coeff pour qu'à 1 cm on soit à la vitesse 100 
            // Si la cible est à droite il faut ralentir la vitesse de la roue droite

            int pDirection = 100;
            int maxSpeedFromDistance = (int)Math.Min(distanceCible* pDirection, 255);

            // On arrête quand on se raproche de 10 !
            if (distanceCible <= 3)
            {
                Log($"Position reached: {_xR},{_yR}");
                Stop();
            }

            Log($"Consigne distance: {maxSpeedFromDistance} direction: {consigneOrientation} " + (isRight ? "R" : "L"));

            int consigneRight = maxSpeedFromDistance - (isRight ? consigneOrientation : 0);
            int consigneLeft =  maxSpeedFromDistance - (isRight ? 0  : consigneOrientation);

            consigneRight = Math.Max(consigneRight, 0);
            consigneLeft = Math.Max(consigneLeft, 0);

            //Calcul de la différence entre l'erreur au coup précédent et l'erreur actuelle.
            deltaErreur = erreurAngleInRad - erreurPre;

            //Mise en mémoire de l'erreur actuelle
            erreurPre = erreurAngleInRad;

            // We must invert it as to turn right we set speed on left etc.
            _leftMotor.SetSpeed(consigneLeft);
            _rightMotor.SetSpeed(consigneRight);

            Log($"Speed L: {_leftMotor.CurrentSpeed} Speed R:{_rightMotor.CurrentSpeed}");


            InitSensorCount();
        }
    }
}
