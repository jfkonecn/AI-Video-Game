using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsteroidsHandler;
using System.Drawing;

namespace AsteroidsHandler.FlyingObjects.Functions
{
    class PointAdjuster
    {

        private static int setStdX(int centerX, int stdXOffCenter, int stdYOffCenter, int panelWidth, double objScale = 1)
        {
            
            //adjust based on panel and scale
            return (int)(objScale * (double)stdXOffCenter * ((double)panelWidth / (double)GenericDrawingPanel.PanelWidthStd));
        }

        private static int setStdY(int centerY, int stdXOffCenter, int stdYOffCenter, int panelHeight, double objScale = 1)
        {

            //adjust based on panel and scale
            return (int)(objScale * (double)stdYOffCenter * ((double)panelHeight / (double)GenericDrawingPanel.PanelHeightStd));
        }

        /// <summary>
        /// Will determine the correct x point based on panel conditions. Standard panel conditions are determined by GenericDrawingPanel.PanelWidthStd
        /// </summary>
        /// <param name="centerX">The point where stdXOffCenter is referencing</param>
        /// <param name="stdXOffCenter"></param>
        /// <param name="stdYOffCenter"></param>
        /// <param name="panelWidth">current drawing panel width</param>
        /// <param name="objScale">Mutilplies the corrected distance by this facto</param>
        /// <param name="angleShift">Angle shift in degrees</param>
        /// <returns>corrected x point</returns>
        private static int xOffCenter(int centerX, int stdXOffCenter, int stdYOffCenter, int panelWidth, double objScale = 1, double angleShift = 0)
        {
            double radAngle = Math.PI / (double)180 * angleShift;
            //adjust based on angle
            return (int)(stdXOffCenter * Math.Cos(radAngle) - stdYOffCenter * Math.Sin(radAngle) + centerX);
            //return (int)(stdXOffCenter * Math.Sin(radAngle) + centerX);
        }
        /// <summary>
        /// Will determine the correct y point based on panel conditions. Standard panel conditions are determined by GenericDrawingPanel.PanelWidthStd
        /// </summary>
        /// <param name="centerY"></param>
        /// <param name="stdXOffCenter"></param>
        /// <param name="stdYOffCenter"></param>
        /// <param name="panelHeight">current drawing panel height</param>
        /// <param name="objScale">Mutilplies the corrected distance by this factor</param>
        /// <param name="angleShift">Angle shift in degrees</param>
        /// <returns>corrected y point</returns>
        private static int yOffCenter(int centerY, int stdXOffCenter, int stdYOffCenter, int panelHeight, double objScale = 1, double angleShift = 0)
        {
            double radAngle = Math.PI / (double)180 * angleShift;
            //adjust based on panel and scale
            //stdYOffCenter = (int)(objScale * (double)stdYOffCenter * ((double)panelHeight / (double)GenericDrawingPanel.PanelHeightStd));
            //adjust based on angle
            //return (int)(stdYOffCenter * Math.Cos(radAngle) + stdXOffCenter * Math.Sin(radAngle) + centerY);
            return (int)(stdYOffCenter * Math.Cos(radAngle) + stdXOffCenter * Math.Sin(radAngle) + centerY);
        }

        /// <summary>
        /// Will determine the correct point based on panel conditions. Standard panel conditions are determined by GenericDrawingPanel.
        /// </summary>
        /// <param name="retPoint">The point which will be adjusted(this point will be changed)</param>
        /// <param name="centerPoint">The current object center point(this point will not be changed)</param>
        /// <param name="stdXOffCenter">How off center the x is under standard conditions</param>
        /// <param name="stdYOffCenter">How off center the y is under standard conditions</param>
        /// <param name="panelWidth">Width of the drawing panel</param>
        /// <param name="panelHeight">Height of the drawing panel</param>
        /// <param name="objScale">Scales the standard conditions</param>
        /// <param name="angleShift">In degrees, rotation of the object</param>
        internal static Point adjustPoint(Point centerPoint, int stdXOffCenter, int stdYOffCenter, int panelWidth, int panelHeight, double objScale = 1, double angleShift = 0)
        {
            stdXOffCenter = setStdX(centerPoint.X, stdXOffCenter, stdYOffCenter, panelWidth, objScale);
            stdYOffCenter = setStdY(centerPoint.Y, stdXOffCenter, stdYOffCenter, panelHeight, objScale);
            return new Point(
                xOffCenter(centerPoint.X, stdXOffCenter, stdYOffCenter, panelWidth, objScale, angleShift),
                yOffCenter(centerPoint.Y, stdXOffCenter, stdYOffCenter, panelHeight, objScale, angleShift));
        }



        /// <summary>
        /// Will determine the correct point based on panel conditions. Standard panel conditions are determined by GenericDrawingPanel.
        /// </summary>
        /// <param name="retPoint">The point which will be adjusted(this point will be changed)</param>
        /// <param name="centerPoint">The current object center point(this point will not be changed)</param>
        /// <param name="stdXOffCenter">How off center the x is under standard conditions</param>
        /// <param name="stdYOffCenter">How off center the y is under standard conditions</param>
        /// <param name="panelWidth">Width of the drawing panel</param>
        /// <param name="panelHeight">Height of the drawing panel</param>
        /// <param name="objScale">Scales the standard conditions</param>
        /// <param name="angleShift">In degrees, rotation of the object</param>
        internal static Point adjustPointAcc(Point centerPoint, int stdXOffCenter, int stdYOffCenter, int panelWidth, int panelHeight, double objScale = 1, double angleShift = 0)
        {
            stdXOffCenter = setStdX(centerPoint.X, stdXOffCenter, stdYOffCenter, panelWidth, objScale);
            stdYOffCenter = setStdY(centerPoint.Y, stdXOffCenter, stdYOffCenter, panelHeight, objScale);
            return new Point(centerPoint.X + stdXOffCenter, centerPoint.Y + stdYOffCenter);
        }

        /// <summary>
        /// Adjusts center point if it goes off screen
        /// </summary>
        /// <param name="centerPoint"></param>
        /// <param name="panelWidth"></param>
        /// <param name="panelHeight"></param>
        /// <param name="panelXLoc"></param>
        /// <param name="panelYLoc"></param>
        /// <returns></returns>

        internal static Point centerPointFix(Point centerPoint, int panelWidth, int panelHeight, int panelXLoc, int panelYLoc)
        {
            if (centerPoint.X < panelXLoc)
            {
                centerPoint.X = panelWidth - (panelXLoc - centerPoint.X);
            }
            else if (centerPoint.X > panelWidth + panelXLoc)
            {
                centerPoint.X = centerPoint.X - panelWidth;
            }
            if (centerPoint.Y < panelYLoc)
            {
                centerPoint.Y = panelHeight - (panelYLoc - centerPoint.Y);
            }
            else if (centerPoint.Y > panelHeight + panelYLoc)
            {
                centerPoint.Y = centerPoint.Y - panelHeight;
            }
            return centerPoint;
        }

        /// <summary>
        /// Used for objects which start off of the screen
        /// </summary>
        /// <param name="centerPoint"></param>
        /// <param name="hasEntered"></param>
        /// <param name="panelWidth"></param>
        /// <param name="panelHeight"></param>
        /// <param name="panelXLoc"></param>
        /// <param name="panelYLoc"></param>
        /// <returns></returns>
        internal static Point centerPointFix(Point centerPoint, ref bool hasEntered, int panelWidth, int panelHeight, int panelXLoc, int panelYLoc)
        {
            if (hasEntered == false)
            {
                if (
                    centerPoint.X > panelXLoc &&
                    centerPoint.X < panelWidth + panelXLoc &&
                    centerPoint.Y > panelYLoc &&
                    centerPoint.Y < panelHeight + panelYLoc)
                {
                    hasEntered = true;
                }
                else
                {
                    return centerPoint;
                }
            }
            
            return centerPointFix(centerPoint, panelWidth, panelHeight, panelXLoc, panelYLoc);
        }
    }
}
