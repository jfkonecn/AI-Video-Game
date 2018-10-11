using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsteroidsHandler.FlyingObjects.Functions;

namespace AsteroidsHandler.FlyingObjects
{
    internal class Asteroid : AbstractObject
    {
        
        internal Asteroid(GenericDrawingPanel gamePanel, int frameRate)
        {
            this.GamePanel = gamePanel;
            this.FrameRate = frameRate;
            this.IsKillable = true;
            regenerate();
        }

        /****************************************************************************
        * Properties
        *****************************************************************************/
        private static Random rand = new Random();
        /// <summary>
        /// The magnitude of the velocity in pixels per frame
        /// </summary>
        private readonly double TotalVelocity = 1;

        /// <summary>
        /// The total lifespan of the asteroid in seconds
        /// </summary>
        private readonly int LifeSpan = 120;

        internal override Point[] AllPoints
        {
            get
            {
                return new Point[] {
                    PointAdjuster.adjustPoint(this.CenterPoint, 30, 0, this.GamePanel.GetPanelWidth, this.GamePanel.GetPanelHeight),
                    PointAdjuster.adjustPoint(this.CenterPoint, 0, 30, this.GamePanel.GetPanelWidth, this.GamePanel.GetPanelHeight),
                    PointAdjuster.adjustPoint(this.CenterPoint, -30, 0, this.GamePanel.GetPanelWidth, this.GamePanel.GetPanelHeight),
                    PointAdjuster.adjustPoint(this.CenterPoint, 0, -30, this.GamePanel.GetPanelWidth, this.GamePanel.GetPanelHeight),
                    PointAdjuster.adjustPoint(this.CenterPoint, 30, 0, this.GamePanel.GetPanelWidth, this.GamePanel.GetPanelHeight) };
            }
        }


        internal override Point[] CollisionPoints
        {
            get
            {
                return this.AllPoints;
            }
        }


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

        /// <summary>
        /// The number of frames which equal a second
        /// </summary>
        private int FrameRate { get; set; }

        /// <summary>
        /// The total number of frames remaining until the object dies
        /// </summary>
        private int FramesRemaining { get; set; }



        /// <summary>
        /// Stores useful data about the drawing panel
        /// </summary>
        private GenericDrawingPanel GamePanel { get; set; }

        /// <summary>
        /// true if the asteroid has entered the screen
        /// </summary>
        private bool _HasEntered = false;

        /****************************************************************************
        * Methods
        *****************************************************************************/
        /// <summary>
        /// Resets the position of the asteroid
        /// </summary>
        private void regenerate()
        {
            this.IsAlive = true;

            _HasEntered = false;

            this.VeloXTrun = 0;
            this.VeloYTrun = 0;

            this.FramesRemaining = this.FrameRate * this.LifeSpan;

            double asteroidAngle = rand.NextDouble() * 360;

            double radAngle = Math.PI / (double)180 * (asteroidAngle + 90);
            this.VeloX = this.TotalVelocity * Math.Cos(radAngle);
            this.VeloY = this.TotalVelocity * Math.Sin(radAngle);
            double radius = Math.Sqrt(Math.Pow(this.GamePanel.GetPanelHeight, 2) + Math.Pow(this.GamePanel.GetPanelWidth, 2)) / 2;
            this.CenterPoint = new Point(
                (int)(radius * Math.Cos(radAngle - Math.PI)) + this.GamePanel.GetPanelWidth / 2 + this.GamePanel.GetPanelLocX,
                (int)(radius * Math.Sin(radAngle - Math.PI)) + this.GamePanel.GetPanelHeight / 2 + this.GamePanel.GetPanelLocY);
        }

        /// <summary>
        /// Updates the current position of the points and returns the resulting points
        /// </summary>
        /// <returns></returns>
        internal override Stack<Point[]> updateAllPoints()
        {
            Stack<Point[]> retStack = new Stack<Point[]>();

            if (this.FramesRemaining <= 0 || !this.IsAlive)
            {
                this.regenerate();
            }

            //move center point
            this.CenterPoint = PointAdjuster.adjustPoint(
                this.CenterPoint,
                (int)Math.Truncate(this.VeloX + this.VeloXTrun),
                (int)Math.Truncate(this.VeloY + this.VeloYTrun),
                this.GamePanel.GetPanelWidth,
                this.GamePanel.GetPanelHeight);

            this.VeloXTrun += this.VeloX - Math.Truncate(this.VeloX + this.VeloXTrun);
            this.VeloYTrun += this.VeloY - Math.Truncate(this.VeloY + this.VeloYTrun);

            //if center point goes off screen then put it back on screen
            this.CenterPoint = PointAdjuster.centerPointFix(
                this.CenterPoint, 
                ref _HasEntered,
                this.GamePanel.GetPanelWidth,
                this.GamePanel.GetPanelHeight,
                this.GamePanel.GetPanelLocX,
                this.GamePanel.GetPanelLocY);

            this.FramesRemaining--;

            retStack.Push(this.AllPoints);
            return retStack;
        }
    }
}
