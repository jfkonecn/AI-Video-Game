using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsteroidsHandler.FlyingObjects;
using System.Drawing;

namespace AsteroidsHandler
{
    internal class GenericDrawingPanel
    {
        internal GenericDrawingPanel(List<AbstractObject> flyingObjects, int panelHeight = -1, int panelWidth = -1, int panelLocX = -1, int panelLocY = -1, bool isMinimized = true)
        {
            this.FlyingObjects = flyingObjects;
            this.updatePanelSize(panelHeight, panelWidth, panelLocX, panelLocY, isMinimized);
        }


        /****************************************************************************
        * Properties
        *****************************************************************************/
        /// <summary>
        /// Stores what the width aspect ratio for all objects should be
        /// </summary>
        internal static readonly int PanelWidthStd = 797;

        /// <summary>
        /// Stores what the height aspect ratio for all objects should be
        /// </summary>
        internal static readonly int PanelHeightStd = 381;

        /// <summary>
        /// If the game is minimized, then no drawing will occur
        /// </summary>
        private bool IsMinimized { get; set; }


        internal bool GetIsMinized { get; }

        /// <summary>
        /// Stores the width of the game drawing panel
        /// </summary>
        private int PanelWidth { get; set; }

        internal int GetPanelWidth
        {
            get
            {
                return this.PanelWidth;
            }
        }

        /// <summary>
        /// Stores the height of the game drawing panel
        /// </summary>
        private int PanelHeight { get; set; }

        internal int GetPanelHeight
        {
            get
            {
                return this.PanelHeight;
            }
        }




        /// <summary>
        /// stores the upper left x location of the panel
        /// </summary>
        private int PanelLocX { get; set; }


        internal int GetPanelLocX
        {
            get
            {
                return this.PanelLocX;
            }
        }

        /// <summary>
        /// stores the upper left y location of the panel
        /// </summary>
        private int PanelLocY { get; set; }

        internal int GetPanelLocY
        {
            get
            {
                return this.PanelLocY;
            }
        }

        /// <summary>
        /// Stores all of the objects in the game
        /// </summary>
        private List<AbstractObject> FlyingObjects { get; set; }

        /****************************************************************************
        * Methods
        *****************************************************************************/
        internal void updatePanelSize(int panelHeight, int panelWidth, int panelLocX, int panelLocY, bool isMinimized)
        {
            this.IsMinimized = isMinimized;
            if (this.IsMinimized)
            {
                return;
            }
            double deltaXPan = (double)panelWidth / (double)this.PanelWidth;
            double deltaYPan = (double)panelHeight / (double)this.PanelHeight;
            double deltaXLoc = (double)panelLocX / (double)this.PanelLocX;
            double deltaYLoc = (double)panelLocY / (double)this.PanelLocY;

            foreach (AbstractObject obj in this.FlyingObjects)
            {


                obj.CenterPoint = new Point(
                    (int)((double)(obj.CenterPoint.X - this.PanelLocX) * deltaXPan) + this.PanelLocX,
                    (int)((double)(obj.CenterPoint.Y - this.PanelLocY) * deltaYPan) + this.PanelLocY);
            }
            this.PanelHeight = panelHeight;
            this.PanelWidth = panelWidth;
            this.PanelLocX = panelLocX;
            this.PanelLocY = panelLocY;
        }
    }
}
