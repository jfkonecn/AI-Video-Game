using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using AsteroidsHandler.FlyingObjects;
using AsteroidsHandler.FlyingObjects.Functions;

namespace AsteroidsHandler
{
    public class Game
    {
        public Game(ref PictureBox mainPanel, bool isMinimized)
        {
            this.FlyingObjects = new List<AbstractObject>();
            this.GamePanel = new GenericDrawingPanel(this.FlyingObjects, mainPanel.Height, mainPanel.Width, mainPanel.Location.X, mainPanel.Location.Y, isMinimized);

            this.addShip(1000, 8, 2, 20);
            for (int i = 0; i < 10; i++)
            {
                this.addAsteroid();
            }
        }

        /// <summary>
        /// vs matchup
        /// </summary>
        /// <param name="mainPanel"></param>
        /// <param name="isMinimized"></param>
        /// <param name="ships"></param>
        public Game(ref PictureBox mainPanel, bool isMinimized, int ships)
        {
            this.FlyingObjects = new List<AbstractObject>();
            this.GamePanel = new GenericDrawingPanel(this.FlyingObjects, mainPanel.Height, mainPanel.Width, mainPanel.Location.X, mainPanel.Location.Y, isMinimized);
            for(int i = 0; i < ships; i++)
            {
                this.addShip(1000, 8, 2, 20);
            }
            
        }

        /****************************************************************************
        * Properties
        *****************************************************************************/
        private static Random rand = new Random();
        internal static List<Missle> missleCollection = new List<Missle>();

        /// <summary>
        /// The maximum amount of flying objects allowed
        /// </summary>
        private readonly int MaxObjects = 20;

        /// <summary>
        /// Stores all of the objects in the game
        /// </summary>
        private List<AbstractObject> FlyingObjects{get; set;}
         
        /// <summary>
        /// Stores useful data about the drawing panel
        /// </summary>
        private GenericDrawingPanel GamePanel { get; set; }

        /****************************************************************************
        * Methods
        *****************************************************************************/
        /// <summary>
        /// When the panel size is changed, this method must be called
        /// </summary>
        /// <param name="mainPanel"></param>
        /// <param name="isMinimized"></param>
        public void updatePanelSize(ref PictureBox mainPanel, bool isMinimized)
        {
            updatePanelSize(mainPanel.Height, mainPanel.Width, mainPanel.Location.X, mainPanel.Location.Y, isMinimized);
        }

        public void updatePanelSize(int panelHeight, int panelWidth, int panelLocX, int panelLocY, bool isMinimized)
        {
            this.GamePanel.updatePanelSize(panelHeight, panelWidth, panelLocX, panelLocY, isMinimized);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        public void draw(PaintEventArgs e)
        {
            Pen myPen = new Pen(Color.White);
            if (this.GamePanel.GetIsMinized)
            {
                return;
            }
            Dictionary<int, int> scoreLookup = new Dictionary<int, int>();
            this.FlyingObjects.AddRange(missleCollection);
            missleCollection.Clear();
            foreach (AbstractObject obj in this.FlyingObjects)
            {
                Stack<Point[]> stk = obj.updateAllPoints();
                if(stk.Count <= 0)
                {
                    //do nothing
                }
                else if (obj.GetType() == typeof(SpaceShip))
                {
                    SpaceShip temp = (SpaceShip)obj;
                    scoreLookup.Add(obj.GetIDTag, 0);
                    temp.botMove();
                    if (temp.IsKillable)
                    {
                        myPen.Color = Color.White;
                    }
                    else
                    {
                        myPen.Color = Color.Purple;
                    }
                    e.Graphics.DrawLines(myPen, stk.Pop());

                    myPen.Color = Color.Green;
                    while(stk.Count > 0)
                    {
                        e.Graphics.DrawLines(myPen, stk.Pop());
                    }
                }
                else
                {
                    myPen.Color = Color.White;
                    e.Graphics.DrawLines(myPen, stk.Pop());
                }
            }
            for (int i = 0; i < this.FlyingObjects.Count; i++)
            {
                for (int j = i + 1; j < this.FlyingObjects.Count; j++)
                {
                    if ((this.FlyingObjects[i].IsKillable &&
                        this.FlyingObjects[j].IsKillable &&
                        (this.FlyingObjects[j].GetType() != typeof(Asteroid) ||
                        this.FlyingObjects[i].GetType() != typeof(Asteroid)) &&
                        (CollisionDetection.PointInPolygon(this.FlyingObjects[i].CenterPoint.X, this.FlyingObjects[i].CenterPoint.Y, this.FlyingObjects[j].CollisionPoints) ||
                        CollisionDetection.PolygonInPolygon(this.FlyingObjects[i].CollisionPoints, this.FlyingObjects[j].CollisionPoints)))
                         )
                    {
                        this.FlyingObjects[i].IsAlive = false;
                        this.FlyingObjects[j].IsAlive = false;
                        if (this.FlyingObjects[j].GetType() == typeof(Missle) && this.FlyingObjects[i].GetType() != typeof(Missle))
                        {
                            scoreLookup[this.FlyingObjects[j].GetIDTag]++;
                        }
                        else if (this.FlyingObjects[i].GetType() == typeof(Missle) && this.FlyingObjects[j].GetType() != typeof(Missle))
                        {
                            scoreLookup[this.FlyingObjects[i].GetIDTag]++;
                        }
                    }
                }
            }
            int deadID = -5;
            for (int i = 0; i < this.FlyingObjects.Count;  i++)
            {
                
                if (this.FlyingObjects[i].GetType() == typeof(SpaceShip) )
                {
                    SpaceShip temp = (SpaceShip)this.FlyingObjects[i];
                    if (this.FlyingObjects[i].IsAlive)
                    {
                        temp.Kills += scoreLookup[this.FlyingObjects[i].GetIDTag];
                    }
                    else
                    {
                        deadID = temp.GetIDTag;
                    }
                    
                }
                else if ((!this.FlyingObjects[i].IsAlive && this.FlyingObjects[i].GetType() == typeof(Missle)))
                {
                    this.FlyingObjects.Remove(this.FlyingObjects[i]);
                }
            }

            if(deadID != -5)
            {
                for (int i = 0; i < this.FlyingObjects.Count; i++)
                {
                    this.FlyingObjects[i].IsAlive = false;
                }

            }
            //for (int i = 0; i < this.FlyingObjects.Count; i++)
            //{
            //    if(this.FlyingObjects[i].GetType() == typeof(Missle) && this.FlyingObjects[i].GetIDTag == deadID)
            //    {
            //        this.FlyingObjects[i].IsAlive = false;
            //    }
            //}

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="totalMissles"></param>
        /// <param name="numEyes"></param>
        /// <param name="numLayers"></param>
        /// <param name="totalNets"></param>
        public void addShip(
            int totalMissles,
            int numEyes = 8,
            int numLayers = 0,
            int totalNets = 40
            )
        {
            this.FlyingObjects.Add(new SpaceShip(this.GamePanel, this.FlyingObjects, 60, 1000, numEyes, numLayers, totalNets));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="totalMissles"></param>
        /// <param name="numEyes"></param>
        /// <param name="numLayers"></param>
        /// <param name="totalNets"></param>
        public void addVsShip(
            int totalMissles,
            int numEyes = 8,
            int numLayers = 0,
            int totalNets = 40
            )
        {
            Point restPoint = new Point(0, 0);
            restPoint.X = rand.Next();
            this.FlyingObjects.Add(new SpaceShip(this.GamePanel, this.FlyingObjects, 60, 1000, restPoint, numEyes, numLayers, totalNets));
        }

        /// <summary>
        /// 
        /// </summary>
        public void addAsteroid()
        {
            this.FlyingObjects.Add(new Asteroid(this.GamePanel, 60));
        }
    }

    
}
