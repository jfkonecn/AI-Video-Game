using System;
using System.Collections.Generic;
using System.Drawing;
using AsteroidsHandler.FlyingObjects.Functions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidsHandler.FlyingObjects
{
    internal class Missle:AbstractObject
    {
        internal Missle(Point centerPoint, double missleAngle, int id, GenericDrawingPanel gamePanel, int frameRate)
        {
            this.GamePanel = gamePanel;
            this.FrameRate = frameRate;
            this.SetIDTag = id;
            this.IsAlive = true;
            this.IsKillable = true;

            this.VeloXTrun = 0;
            this.VeloYTrun = 0;

            this.FramesRemaining = this.FrameRate * this.LifeSpan;

            double radAngle = Math.PI / (double)180 * (missleAngle + 90);
            this.VeloX = this.TotalVelocity * Math.Cos(radAngle);
            this.VeloY = this.TotalVelocity * Math.Sin(radAngle);

            this.CenterPoint = PointAdjuster.adjustPointAcc(
                    centerPoint,
                    (int)this.VeloX * 2,
                    (int)this.VeloY * 2,
                    this.GamePanel.GetPanelWidth,
                    this.GamePanel.GetPanelHeight);
        }

        /****************************************************************************
        * Properties
        *****************************************************************************/
        /// <summary>
        /// The magnitude of the velocity in pixels per frame
        /// </summary>
        private readonly double TotalVelocity = 3;

        /// <summary>
        /// The total lifespan of the asteroid in seconds
        /// </summary>
        private readonly int LifeSpan = 2;

        internal override Point[] AllPoints
        {
            get
            {
                return new Point[] {
                    PointAdjuster.adjustPoint(this.CenterPoint, 3, 0, this.GamePanel.GetPanelWidth, this.GamePanel.GetPanelHeight),
                    PointAdjuster.adjustPoint(this.CenterPoint, 0, 3, this.GamePanel.GetPanelWidth, this.GamePanel.GetPanelHeight),
                    PointAdjuster.adjustPoint(this.CenterPoint, -3, 0, this.GamePanel.GetPanelWidth, this.GamePanel.GetPanelHeight),
                    PointAdjuster.adjustPoint(this.CenterPoint, 0, -3, this.GamePanel.GetPanelWidth, this.GamePanel.GetPanelHeight),
                    PointAdjuster.adjustPoint(this.CenterPoint, 3, 0, this.GamePanel.GetPanelWidth, this.GamePanel.GetPanelHeight) };
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

        /****************************************************************************
        * Methods
        *****************************************************************************/

        /// <summary>
        /// Updates the current position of the points and returns the resulting points
        /// </summary>
        /// <returns></returns>
        internal override Stack<Point[]> updateAllPoints()
        {
            Stack<Point[]> retStack = new Stack<Point[]>();

            if (this.FramesRemaining <= 0 || !this.IsAlive)
            {
                this.IsAlive = false;
                return retStack;
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
