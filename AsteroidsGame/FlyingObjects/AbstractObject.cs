using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AsteroidsHandler.FlyingObjects
{
    internal abstract class AbstractObject
    {

        /****************************************************************************
        * Properties
        *****************************************************************************/
  
        /// <summary>
        /// Unique identifier for this flying object
        /// </summary>
        private int _IDTag = -1;
        /// <summary>
        /// gets id tag
        /// </summary>
        internal int GetIDTag
        {
            get
            {
                return _IDTag;
            }
        }
        /// <summary>
        /// sets id tag
        /// </summary>
        internal int SetIDTag
        {
            set
            {
                _IDTag = value;
            }
        }

        /// <summary>
        /// Indicates if the object is alive
        /// </summary>
        internal bool IsAlive { get; set; }

        /// <summary>
        /// Returns all drawing points
        /// </summary>
        abstract internal Point[] AllPoints { get; }


        /// <summary>
        /// Returns all points which counts for collisions
        /// </summary>
        abstract internal Point[] CollisionPoints { get; }

        /// <summary>
        /// Center point of the asteroid
        /// </summary>
        internal Point CenterPoint { get; set; }

        /// <summary>
        /// true if the ship can be killed
        /// </summary>
        internal bool IsKillable { get; set; }



        /****************************************************************************
        * Methods
        *****************************************************************************/

        /// <summary>
        /// Updates all points
        /// </summary>
        /// <returns>a stack which contains all separate drawing point arrays</returns>
        abstract internal Stack<Point[]> updateAllPoints();

        /// <summary>
        /// The next id which will be given to the next flying object
        /// </summary>
        private static int _NextID = 0;

        /// <summary>
        /// sets this object's id tag to the current value of _NextID
        /// </summary>
        internal void giveIDTag()
        {
            this.SetIDTag = _NextID;
            _NextID++;
        }




    }
}
