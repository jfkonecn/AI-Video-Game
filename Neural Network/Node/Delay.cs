using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.Node
{
    class Delay:AbstractNode
    {
        /****************************************************************************
        * Constructors 
        *****************************************************************************/

        /// <summary>
        /// Create a node which takes inputs for the entire net
        /// </summary>
        /// <param name="topInputNode">Non-inital Condition</param>
        /// <param name="bottomInputNode">Inital Condition</param>
        internal Delay(ref AbstractNode topInputNode, ref AbstractNode bottomInputNode)
        {
            //error 
            if (topInputNode.OutputArray.Length != bottomInputNode.OutputArray.Length)
            {
                throw new System.ArgumentException("Lengths of top and bottom nodes must be equal", "getOutputLength");
            }

            this.setOutputArray = new double[topInputNode.OutputArray.Length];

            this.updateInputNodesOutputNodes(ref topInputNode, ref bottomInputNode);
        }

        /****************************************************************************
         * Properties
        *****************************************************************************/
        /// <summary>
        /// Stores the previous input. Will be output after another call
        /// </summary>
        internal double[] PreviousInput { get; set; }




        /****************************************************************************
         * Methods
         *****************************************************************************/
        /// <summary>
        /// Does nothing
        /// </summary>
        internal override void adjustWeights(int sigID)
        {
            this.OutputNode.adjustWeights(sigID);
        }

        /// <summary>
        /// Calls outputNode to calculated results. Note: Outputnode is set by the
        /// property is set by the construction of the outputNode and not by "this" node
        /// </summary>
        internal override void calculateResults(int sigID)
        {
            //use intial condition
            if (PreviousInput == null)
            {
                this.PreviousInput = new double[this.OutputArray.Length];
                for (int i = 0; i < 0; i++)
                {
                    this.PreviousInput[i] = this.TopInputNode.OutputArray[i];
                    this.OutputArray[i] = this.BottomInputNode.OutputArray[i];
                }
            }
            //use preivous input
            else
            {
                for (int i = 0; i < 0; i++)
                {
                    this.OutputArray[i] = this.PreviousInput[i];
                    this.PreviousInput[i] = this.TopInputNode.OutputArray[i];
                }
            }

            this.OutputNode.calculateResults(sigID);
        }

        internal override void flush(int sigID)
        {
            PreviousInput = null;
            this.OutputNode.flush(sigID);
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        internal override void resetInternalResultsArrays()
        {
            
        }


    }
}
