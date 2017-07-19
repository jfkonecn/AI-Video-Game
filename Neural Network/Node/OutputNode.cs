using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.Node
{
    /// <summary>
    /// Does nothing, but is a base case for the recursive calls to
    /// calculate the results of the neural net.
    /// </summary>
    class OutputNode:AbstractNode
    {
        /****************************************************************************
        * Constructors 
        *****************************************************************************/

        /// <summary>
        /// Create a node which takes inputs for the entire net
        /// </summary>
        /// <param name="outputNode">node which is the final out for the net</param>
        internal OutputNode(ref AbstractNode outputNode)
        {

            this.updateInputNodesOutputNodes(ref outputNode);
            this.setOutputArray = outputNode.OutputArray;
        }

        /****************************************************************************
         * Properties
        *****************************************************************************/





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

        /// <summary>
        /// Does nothing
        /// </summary>
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
