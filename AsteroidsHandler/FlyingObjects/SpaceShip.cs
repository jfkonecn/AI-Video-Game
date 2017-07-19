using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using NeuralNetwork;
using NeuralNetwork.Trainers;
using AsteroidsHandler.FlyingObjects.Functions;

namespace AsteroidsHandler.FlyingObjects
{
    internal class SpaceShip : AbstractObject
    {
        /// <summary>
        /// User controlled ship
        /// </summary>
        /// <param name="flyingObjects">Stores all of the detectable objects</param>
        /// <param name="frameRate">The number of frames which equal a second</param>
        /// <param name="totalMissles">The total number of missles the space ship will be allowed to fire</param>
        internal SpaceShip(GenericDrawingPanel gamePanel, List<AbstractObject> flyingObjects, int frameRate, int totalMissles)
        {
            this.RestPoint = new Point(
                    this.GamePanel.GetPanelWidth / 2 + this.GamePanel.GetPanelLocX,
                    this.GamePanel.GetPanelHeight / 2 - this.GamePanel.GetPanelLocY);

            spaceShipInit(ref gamePanel, false, ref flyingObjects, frameRate, totalMissles);
        }

        /// <summary>
        /// neural net controlled ship
        /// </summary>
        /// <param name="flyingObjects">Stores all of the detectable objects</param>
        /// <param name="frameRate">The number of frames which equal a second</param>
        /// <param name="totalMissles">The total number of missles the space ship will be allowed to fire</param>
        /// <param name="numEyes">The number of eyes the ship will used to detect other flying objects</param>
        /// <param name="numLayers">The number of hidden layers in the feedforward network</param>
        /// <param name="totalNets">The total number of neural nets the space ship will user in its genetic algorithm</param>

        internal SpaceShip(
            GenericDrawingPanel gamePanel,
            List<AbstractObject> flyingObjects,
            int frameRate,
            int totalMissles,
            int numEyes = 8,
            int numLayers = 0,
            int totalNets = 40
            )
        {
            this.RestPoint = new Point(
                    gamePanel.GetPanelWidth / 2 + gamePanel.GetPanelLocX,
                    gamePanel.GetPanelHeight / 2 - gamePanel.GetPanelLocY);
            spaceShipInit(ref gamePanel, false, ref flyingObjects, frameRate, totalMissles, numEyes, numLayers, totalNets);
        }

        /// <summary>
        /// neural net controlled ship
        /// </summary>
        /// <param name="flyingObjects">Stores all of the detectable objects</param>
        /// <param name="frameRate">The number of frames which equal a second</param>
        /// <param name="totalMissles">The total number of missles the space ship will be allowed to fire</param>
        /// <param name="numEyes">The number of eyes the ship will used to detect other flying objects</param>
        /// <param name="numLayers">The number of hidden layers in the feedforward network</param>
        /// <param name="totalNets">The total number of neural nets the space ship will user in its genetic algorithm</param>

        internal SpaceShip(
            GenericDrawingPanel gamePanel,
            List<AbstractObject> flyingObjects,
            int frameRate,
            int totalMissles,
            Point restPoint,
            int numEyes = 8,
            int numLayers = 0,
            int totalNets = 40
            )
        {
            this.RestPoint = new Point(restPoint.X, restPoint.Y);
            spaceShipInit(ref gamePanel, false, ref flyingObjects, frameRate, totalMissles, numEyes, numLayers, totalNets);
        }
        /****************************************************************************
        * Properties
        *****************************************************************************/
        /// <summary>
        /// Scales the 1:1 ratio of the ship
        /// </summary>
        private readonly double ShipScale = 0.5;

        /// <summary>
        /// The total lifespan of the space ship in seconds
        /// </summary>
        private readonly int LifeSpan = 120;

        /// <summary>
        /// The total time a ship cannot be hurt in seconds
        /// </summary>
        private readonly int GraceSpan = 5;

        /// <summary>
        /// The total time a ship must wait to fire in seconds
        /// </summary>
        private readonly double MissleBreak = 1;

        /// <summary>
        /// The total distance a spaceship eye can see
        /// </summary>
        private readonly int EyeRadius = 600;

        /// <summary>
        /// max ship velocity
        /// </summary>
        private readonly double MaxVelocity = 50 * Math.Sqrt(2);

        /// <summary>
        /// The number of frames which equal a second
        /// </summary>
        private int FrameRate { get; set; }

        /// <summary>
        /// The total number of frames remaining until the object dies
        /// </summary>
        private int FramesRemaining { get; set; }



        /// <summary>
        /// Is this spaceship controlled by the user?
        /// </summary>
        private bool IsUser { get; set; }

        /// <summary>
        /// Total starting missles for the ship
        /// </summary>
        private int TotalMissles { get; set; }

        /// <summary>
        /// Total missles remaining
        /// </summary>
        private int MisslesRemaining { get; set; }

        /// <summary>
        /// store the f
        /// </summary>
        private int MissleFireFrame { get; set; }

        /// <summary>
        /// The total number of eye the space ship has
        /// </summary>
        private int NumEyes { get; set; }

        /// <summary>
        /// The direction the ship is facing in degrees
        /// </summary>
        private double ShipDirectionAngle { get; set; }

        /// <summary>
        /// Stores all of the detectable objects
        /// </summary>
        private List<AbstractObject> FlyingObjects { get; set; }

        /// <summary>
        /// Stores useful data about the drawing panel
        /// </summary>
        private GenericDrawingPanel GamePanel { get; set; }

        /// <summary>
        /// represents the furthest point where each eye can see
        /// </summary>
        private Point[] EyeOffcenterArray { get; set; }

        /// <summary>
        /// represents the point where each eye can currently see
        /// </summary>
        private Point[] EyeInputArray { get; set; }

        /// <summary>
        /// eye input vector for neural net
        /// </summary>
        private double[] EyeFractionArray { get; set; }

        /// <summary>
        /// bool which check if ship was killed by its own missle
        /// </summary>
        private bool KilledSelf { get; set; }
        /// <summary>
        /// number of kills by the ship
        /// </summary>
        internal int Kills { get; set; }

        /// <summary>
        /// get the score for the space ship
        /// </summary>
        private double Score
        {
            get
            {
                //return (double)(this.LifeSpan * this.FrameRate - this.FramesRemaining) / (double)this.LifeSpan + this.Kills;
                return this.Kills;
            }
        }
        /// <summary>
        /// number of misses by the ship
        /// </summary>
        private int Misses { get; set; }

        /// <summary>
        /// Where the ship is placed when it dies
        /// </summary>
        private Point RestPoint { get; set; }

        /// <summary>
        /// True if the ship is thrusting
        /// </summary>
        private bool IsThrust { get; set; }

        /// <summary>
        /// angular velocity of the ship
        /// </summary>
        private double AngularV { get; set; }

        /// <summary>
        /// velocity of the ship in the x direction
        /// </summary>
        private double VeloX { get; set; }

        /// <summary>
        /// velocity of the ship in the y direction
        /// </summary>
        private double VeloY { get; set; }

        /// <summary>
        /// The truncated velocity for x
        /// </summary>
        private double VeloXTrun { get; set; }

        /// <summary>
        /// The truncated velocity for y
        /// </summary>
        private double VeloYTrun { get; set; }


        private double TotalVelocity
        {
            get
            {
                return Math.Sqrt(this.VeloX * this.VeloX + this.VeloY * this.VeloY);
            }
        }

        private Point ShipNose
        {
            get
            {
                return PointAdjuster.adjustPoint(
                            this.CenterPoint,
                            0,
                            24,
                            this.GamePanel.GetPanelWidth,
                            this.GamePanel.GetPanelHeight,
                            this.ShipScale,
                            this.ShipDirectionAngle);
            }
        }
        private Point ShipRightWing
        {
            get
            {
                return PointAdjuster.adjustPoint(
                    this.CenterPoint,
                    18,
                    -24,
                    this.GamePanel.GetPanelWidth,
                    this.GamePanel.GetPanelHeight,
                    this.ShipScale,
                    this.ShipDirectionAngle);
            }
        }
        private Point ShipLeftWing
        {
            get
            {
                return PointAdjuster.adjustPoint(
                    this.CenterPoint,
                    -18,
                    -24,
                    this.GamePanel.GetPanelWidth,
                    this.GamePanel.GetPanelHeight,
                    this.ShipScale,
                    this.ShipDirectionAngle);
            }
        }
        private Point ShipStern
        {
            get
            {
                return PointAdjuster.adjustPoint(
                    this.CenterPoint,
                    0,
                    -18,
                    this.GamePanel.GetPanelWidth,
                    this.GamePanel.GetPanelHeight,
                    this.ShipScale,
                    this.ShipDirectionAngle);
            }
        }
        private Point ShipLeftSternMid
        {
            get
            {
                return PointAdjuster.adjustPoint(
                    this.CenterPoint,
                    -9,
                    -12,
                    this.GamePanel.GetPanelWidth,
                    this.GamePanel.GetPanelHeight,
                    this.ShipScale,
                    this.ShipDirectionAngle);
            }
        }
        private Point ShipRightSternMid
        {
            get
            {
                return PointAdjuster.adjustPoint(
                    this.CenterPoint,
                    9,
                    -12,
                    this.GamePanel.GetPanelWidth,
                    this.GamePanel.GetPanelHeight,
                    this.ShipScale,
                    this.ShipDirectionAngle);
            }
        }
        private Point ShipThrustFire
        {
            get
            {
                return PointAdjuster.adjustPoint(
                    this.CenterPoint,
                    0,
                    -24,
                    this.GamePanel.GetPanelWidth,
                    this.GamePanel.GetPanelHeight,
                    this.ShipScale,
                    this.ShipDirectionAngle);
            }
        }


        /// <summary>
        /// stores all the neural nets
        /// </summary>
        private GeneticNets allNets { get; set; }

        internal override Point[] AllPoints
        {
            get
            {
                if (!this.IsThrust)
                {
                    return new Point[] {
                            this.ShipNose,
                            this.ShipRightWing,
                            this.ShipStern,
                            this.ShipLeftWing,
                            this.ShipNose
                    };
                }
                else
                {
                    return new Point[] {
                            this.ShipLeftSternMid,
                            this.ShipLeftWing,
                            this.ShipNose,
                            this.ShipRightWing,
                            this.ShipRightSternMid,
                            this.ShipStern,
                            this.ShipLeftSternMid,
                            this.ShipThrustFire,
                            this.ShipRightSternMid
                    };
                }
            }
        }


        internal override Point[] CollisionPoints
        {
            get
            {
                return new Point[] {
                            this.ShipNose,
                            this.ShipRightWing,
                            this.ShipStern,
                            this.ShipLeftWing,
                            this.ShipNose};
            }
        }

        

        /****************************************************************************
        * Methods
        *****************************************************************************/
        /// <summary>
        /// Top of the stack contains the points of the space ship. The rest of the stack is for the eyes.
        /// </summary>
        /// <returns></returns>
        internal override Stack<Point[]> updateAllPoints()
        {
            Stack<Point[]> retStack = new Stack<Point[]>();

            if (this.FramesRemaining <= 0 || !this.IsAlive)
            {
                if (!this.IsUser)
                {
                    this.allNets.setCurrentScore(this.Score);
                    this.allNets.goToNextNet();
                }
                    
                this.restShip();
            }
            else if ((this.LifeSpan * this.FrameRate) - this.FramesRemaining > this.GraceSpan * this.FrameRate)
            {
                this.IsKillable = true;
            }


            //handles the thrusting
            if (this.IsThrust && this.TotalVelocity <= MaxVelocity)
            {
                this.IsThrust = true;
                double radAngle = Math.PI / (double)180 * (270 - this.ShipDirectionAngle);
                double acceration = 0.5;
                this.VeloX += acceration * Math.Cos(radAngle);
                this.VeloY -= acceration * Math.Sin(radAngle);
            }

            //move center point
            this.ShipDirectionAngle += this.AngularV;
            this.CenterPoint = PointAdjuster.adjustPointAcc(
                this.CenterPoint, 
                (int)Math.Truncate(this.VeloX + this.VeloXTrun),
                (int)Math.Truncate(this.VeloY + this.VeloYTrun),
                this.GamePanel.GetPanelWidth,
                this.GamePanel.GetPanelHeight,
                this.ShipScale,
                this.ShipDirectionAngle);
            this.VeloXTrun += this.VeloX - Math.Truncate(this.VeloX + this.VeloXTrun);
            this.VeloYTrun += this.VeloY - Math.Truncate(this.VeloY + this.VeloYTrun);

            //if center point goes off screen then put it back on screen
            this.CenterPoint = PointAdjuster.centerPointFix(
                this.CenterPoint, 
                this.GamePanel.GetPanelWidth, 
                this.GamePanel.GetPanelHeight, 
                this.GamePanel.GetPanelLocX, 
                this.GamePanel.GetPanelLocY);

            //update the sight of the ship
            updateEyes();

            this.FramesRemaining--;

            foreach(Point p in this.EyeInputArray)
            {
                retStack.Push(new Point[]{this.CenterPoint, p});
            }
            retStack.Push(this.AllPoints);
            return retStack;
        }
        /// <summary>
        /// Constructor helper function
        /// </summary>
        /// <param name="isUser">Is this a user controlled ship?</param>
        /// <param name="flyingObjects">Stores all of the detectable objects</param>
        /// <param name="frameRate">The number of frames which equal a second</param>
        /// <param name="totalMissles">The total number of missles the space ship will be allowed to fire</param>
        /// <param name="numEyes">The number of eyes the ship will used to detect other flying objects</param>
        /// <param name="numLayers">The number of hidden layers in the feedforward network</param>
        /// <param name="totalNets">The total number of neural nets the space ship will user in its genetic algorithm</param>

        private void spaceShipInit(ref GenericDrawingPanel gamePanel, bool isUser, ref List<AbstractObject> flyingObjects, int frameRate, int totalMissles, int numEyes = -1, int numLayers = -1, int totalNets = -1)
        {
            this.IsUser = isUser;
            this.TotalMissles = totalMissles;
            this.FrameRate = frameRate;

            this.giveIDTag();

            this.GamePanel = gamePanel;


            this.NumEyes = numEyes;
            this.FlyingObjects = flyingObjects;

            this.EyeInputArray = new Point[this.NumEyes];
            this.EyeOffcenterArray = new Point[this.NumEyes];
            this.EyeFractionArray = new double[this.NumEyes];
            restShip();
            double xPoint;
            double yPoint;

            for (int i = 0; i < this.NumEyes; i++)
            {
                double radAngle = (double)(((double)i * 2 * Math.PI) / (this.NumEyes)) + (double)(Math.PI / 2);
                xPoint = this.EyeRadius * Math.Cos(radAngle);
                yPoint = this.EyeRadius * Math.Sin(radAngle);

                this.EyeOffcenterArray[i] = new Point((int)xPoint, (int)yPoint);

            }
            updateEyes();

            if (!this.IsUser)
            {
                //all the eye distances will be inputs
                //Forward, left, right, shoot output
                this.allNets = new GeneticNets(this.NumEyes + 5, 4, numLayers, totalNets);
            }
        }

        /// <summary>
        /// Updates the sight of the spaceship's eyes
        /// </summary>
        private void updateEyes()
        {
            //true when an eye detects an object
            bool isFound = false;


            //detect the closest object within range of all eyes
            for (int i = 0; i < this.NumEyes; i++)
            {
                //number of loops equal to the maximum number of points checked for each eye
                for (int j = 0; j <= 100; j += 5)
                {
                    //check all of the objects in the game to see if they are within the eye's range
                    for (int k = 0; k < this.FlyingObjects.Count; k++)
                    {
                        //point where an object will be looked for
                        Point testPoint = PointAdjuster.adjustPoint(
                            this.CenterPoint,
                            (int)((double)this.EyeOffcenterArray[i].X * (double)j * (double)0.01),
                            (int)((double)this.EyeOffcenterArray[i].Y * (double)j * (double)0.01),
                            this.GamePanel.GetPanelWidth,
                            this.GamePanel.GetPanelHeight,
                            this.ShipScale,
                            this.ShipDirectionAngle);
                        if ((this.FlyingObjects[k].GetIDTag != this.GetIDTag && CollisionDetection.PointInPolygon(testPoint.X, testPoint.Y, this.FlyingObjects[k].CollisionPoints)) || j == 100)
                        {

                            this.EyeInputArray[i] = new Point(testPoint.X, testPoint.Y);
                            this.EyeFractionArray[i] = (double)j * (double)0.01;
                            isFound = true;
                            break;
                        }
                    }
                    if (isFound)
                    {
                        break;
                    }
                }
                isFound = false;
            }
        }

        /// <summary>
        /// resets the placement and score of the ship
        /// </summary>
        private void restShip()
        {
            this.IsKillable = false;
            this.KilledSelf = false;
            this.Kills = 0;
            this.Misses = 0;
            this.IsAlive = true;
            this.FramesRemaining = this.FrameRate * this.LifeSpan;
            this.MisslesRemaining = this.TotalMissles;
            this.AngularV = 0;
            this.VeloX = 0;
            this.VeloY = 0;
            this.VeloXTrun = 0;
            this.VeloYTrun = 0;
            this.ShipDirectionAngle = 0;
            this.MissleFireFrame = 100000000;
            this.CenterPoint = new Point(this.RestPoint.X, this.RestPoint.Y);
        }

        /// <summary>
        /// represents all the commands a ship can receive
        /// </summary>
        internal enum CommandOutput { Thrust, Left, Right, Shoot }

        /// <summary>
        /// Tells ship to perform a move using the bot
        /// </summary>
        internal void botMove()
        {
            //do nothing if a user should be controling
            if (this.IsUser)
                return;

            //update neural net input array
            //input the eye inputs
            Console.Clear();
            Console.Write("Net {0} of {1} in Generation {2}\n", this.allNets.GetCurrentNetIdx + 1, this.allNets.GetTotalNets, this.allNets.GetGenerationNum);
            for (int i = 0; i < NumEyes; i++)
            {
                this.allNets.CurrentNet.InputArray[i] = 1 - this.EyeFractionArray[i];
                Console.Write("Eye {0}:\t{1}\n", i, this.allNets.CurrentNet.InputArray[i]);
            }
            this.allNets.CurrentNet.InputArray[NumEyes] = this.VeloX / this.MaxVelocity;
            Console.Write("X Velocity:\t{0}\n", this.allNets.CurrentNet.InputArray[NumEyes]);

            this.allNets.CurrentNet.InputArray[NumEyes + 1] = this.VeloY / this.MaxVelocity;
            Console.Write("Y Velocity:\t{0}\n", this.allNets.CurrentNet.InputArray[NumEyes + 1]);

            this.allNets.CurrentNet.InputArray[NumEyes + 2] = (((double)(this.CenterPoint.X - this.GamePanel.GetPanelLocX)  / (double)(this.GamePanel.GetPanelWidth- this.GamePanel.GetPanelLocX) - 0.5) * 2.0);
            Console.Write("X Panel Location:\t{0}\n", this.allNets.CurrentNet.InputArray[NumEyes + 2]);

            this.allNets.CurrentNet.InputArray[NumEyes + 3] = (((double)(this.CenterPoint.Y - this.GamePanel.GetPanelLocY)/ (double)(this.GamePanel.GetPanelHeight - this.GamePanel.GetPanelLocY) - 0.5) * 2.0);
            Console.Write("Y Panel Location:\t{0}\n", this.allNets.CurrentNet.InputArray[NumEyes + 3]);

            this.allNets.CurrentNet.InputArray[NumEyes + 4] = 1 - (double)MisslesRemaining / (double)TotalMissles;
            Console.Write("Missles Fired:\t{0}\n\n", this.allNets.CurrentNet.InputArray[NumEyes + 4]);

            this.allNets.CurrentNet.calculateResults();
            Console.Write("Thrust:\t{0}\n", this.allNets.CurrentNet.OutputArray[0]);
            Console.Write("Left:\t{0}\n", this.allNets.CurrentNet.OutputArray[1]);
            Console.Write("Right:\t{0}\n", this.allNets.CurrentNet.OutputArray[2]);
            Console.Write("Shoot:\t{0}\n", this.allNets.CurrentNet.OutputArray[3]);


            if (this.allNets.CurrentNet.OutputArray[0] == 1)
            {
                this.keyDownCommand(CommandOutput.Thrust);
            }
            else
            {
                this.keyUpCommand(CommandOutput.Thrust);
            }
            if (this.allNets.CurrentNet.OutputArray[1] == 1 || this.allNets.CurrentNet.OutputArray[2] == 1)
            {
                if (this.allNets.CurrentNet.OutputArray[2] == 0)
                {
                    this.keyDownCommand(CommandOutput.Left);
                }
                else if (this.allNets.CurrentNet.OutputArray[1] == 0)
                {
                    this.keyDownCommand(CommandOutput.Right);
                }
                else
                {
                    this.keyUpCommand(CommandOutput.Left);
                    this.keyUpCommand(CommandOutput.Right);
                }

            }
            else
            {
                this.keyUpCommand(CommandOutput.Left);
                this.keyUpCommand(CommandOutput.Right);
            }
            if (this.allNets.CurrentNet.OutputArray[3] == 1)
            {
                this.keyDownCommand(CommandOutput.Shoot);
            }
            else
            {
                this.keyUpCommand(CommandOutput.Shoot);

            }
        }


        /// <summary>
        /// When user presses down on a button for a given command
        /// </summary>
        /// <param name="cmd"></param>
        internal void keyDownCommand(CommandOutput cmd)
        {
            if (cmd == CommandOutput.Left)
            {
                this.AngularV = -5;
            }
            if (cmd == CommandOutput.Right)
            {
                this.AngularV = 5;
            }
            if (cmd == CommandOutput.Thrust)
            {
                this.IsThrust = true;
            }
            if (cmd == CommandOutput.Shoot)
            {
                if (this.MissleFireFrame > this.FramesRemaining + this.FrameRate * this.MissleBreak && this.MisslesRemaining > 0 && this.IsKillable)
                {
                    Game.missleCollection.Add(new Missle(this.ShipNose, this.ShipDirectionAngle, this.GetIDTag, this.GamePanel, this.FrameRate));
                    this.MissleFireFrame = this.FramesRemaining;
                    this.MisslesRemaining--;
                }
            }
        }

        /// <summary>
        /// when user releases button for a given command
        /// </summary>
        /// <param name="cmd"></param>
        internal void keyUpCommand(CommandOutput cmd)
        {
            if (cmd == CommandOutput.Left || cmd == CommandOutput.Right)
            {
                this.AngularV = 0;
            }
            if (cmd == CommandOutput.Thrust)
            {
                this.IsThrust = false;
            }
        }
    }
}
