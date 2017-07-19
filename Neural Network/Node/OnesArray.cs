using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.Node
{

    /****************************************************************************
    node which only returns a r x 1 array of ones where r is the number of ones. 
    *****************************************************************************/
    internal class OnesArray : AbstractNode
    {
        /****************************************************************************
         * Constructors 
         *****************************************************************************/

        /****************************************************************************
         * makes an ones array with outputLength ones
        *****************************************************************************/
        internal OnesArray(int outputLength)
        {
            double[] newArray = new double[outputLength];


            for (int i = 0; i < outputLength; i++)
            {
                newArray[i] = 1;
            }

            this.setOutputArray = newArray;
        }

        /****************************************************************************
         * Methods
         *****************************************************************************/
         /// <summary>
         /// Does nothing
         /// </summary>
        internal override void adjustWeights(int sigID)
        {
            return;
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        internal override void calculateResults(int sigID)
        {
            return;
        }

        internal override void flush(int sigID)
        {
            return;
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        internal override void resetInternalResultsArrays()
        {
            return;
        }
    }
}
