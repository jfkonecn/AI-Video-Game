using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.Node
{
    class InputNode:AbstractNode
    {
        /****************************************************************************
        * Constructors 
        *****************************************************************************/

        /// <summary>
        /// Create a node which takes inputs for the entire net
        /// </summary>
        /// <param name="inputLength"></param>
        internal InputNode(int inputLength)
        {
            double[] newArray = new double[inputLength];


            for (int i = 0; i < inputLength; i++)
            {
                newArray[i] = 1;
            }

            //Set input and output equal to eachother since no calculations are performed
            this.setOutputArray = newArray;
            this.InputArray = newArray;
        }

        /// <summary>
        /// creates copy of input node
        /// </summary>
        /// <param name="node"></param>
        internal InputNode(ref InputNode copyNode)
        {
            double[] newArray = new double[copyNode.InputArray.Length];


            for (int i = 0; i < copyNode.InputArray.Length; i++)
            {
                newArray[i] = 1;
            }

            //Set input and output equal to eachother since no calculations are performed
            this.setOutputArray = newArray;
            this.InputArray = newArray;
        }

        /****************************************************************************
         * Properties
        *****************************************************************************/

        internal double[] InputArray { get; set; }


        readonly int id = 0;

        /****************************************************************************
         * Methods
         *****************************************************************************/
        internal void adjustWeights()
        {
            this.adjustWeights(id);
        }

        /// <summary>
        /// Adjust the next weight
        /// </summary>
        internal override void adjustWeights(int sigID)
        {
            this.OutputNode.adjustWeights(sigID);
        }


        internal void calculateResults()
        {
            this.calculateResults(id);
        }

        /// <summary>
        /// Calls outputNode to calculated results. Note: Outputnode is set by the
        /// property is set by the construction of the outputNode and not by "this" node
        /// </summary>
        internal override void calculateResults(int sigID)
        {
            this.OutputNode.calculateResults(sigID);
        }


        internal void flush()
        {
            this.flush(id);
        }

        internal override void flush(int sigID)
        {
            this.OutputNode.flush(sigID);
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
