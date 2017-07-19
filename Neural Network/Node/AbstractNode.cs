using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.Node
{
 /***************************************************************************
 Represents a point where two input nodes come together to create one
 output node
*****************************************************************************/
    abstract internal class AbstractNode
    {
        /****************************************************************************
         * Properties
        *****************************************************************************/

        /// <summary>
        /// 1st input node
        /// </summary>
        protected AbstractNode TopInputNode { get; set; }

        /// <summary>
        /// 2nd input node
        /// </summary>
        protected AbstractNode BottomInputNode { get; set; }


        /// <summary>
        /// output node
        /// </summary>
        AbstractNode _OutputNode;

        /// <summary>
        /// only one change to the output node is allowed
        /// </summary>
        bool _OutputNodeUpdated = false;

        /// <summary>
        /// To issues with program later done the line, the outputNode can only
        /// be set once
        /// </summary>
        internal AbstractNode OutputNode
        {
            get { return _OutputNode; }
            set
            {
                if (!_OutputNodeUpdated)
                {
                    _OutputNodeUpdated = true;
                    _OutputNode = value;
                }
                else
                {
                    throw new System.InvalidOperationException("OutputNode can only be changed once!");
                }                
            }
        }

        /// <summary>
        /// output node
        /// </summary>
        AbstractNode _FeedbackNode;

        /// <summary>
        /// only one change to the output node is allowed
        /// </summary>
        bool _FeedbackNodeUpdated = false;

        /// <summary>
        /// To issues with program later done the line, the outputNode can only
        /// be set once
        /// </summary>
        internal AbstractNode FeedbackNode
        {
            get { return _FeedbackNode; }
            set
            {
                if (!_FeedbackNodeUpdated)
                {
                    _FeedbackNodeUpdated = true;
                    _FeedbackNode = value;
                }
                else
                {
                    throw new System.InvalidOperationException("FeedbackNode can only be changed once!");
                }
            }
        }


        /// <summary>
        /// Array that leaves the node
        /// </summary>
        private double[] _OutputArray;
        internal double[] OutputArray { get { return _OutputArray; } }
        protected double[] setOutputArray { set { _OutputArray = value; } }

        /****************************************************************************
         * Methods
         *****************************************************************************/

        /// <summary>
        /// if there are weights in the node, adjust else do nothing
        /// </summary>
        /// <param name="sigID">he id for forward feeding signal (prevents infinite loops with a feedback node)</param>
        abstract internal void adjustWeights(int sigID);
        /// <summary>
        /// reset any nodes which rely on the previous output (i.e. delays or intergation blocks)
        /// </summary>
        /// <param name="sigID">The id for forward feeding signal (prevents infinite loops with a feedback node)</param>
        abstract internal void flush(int sigID);

        /// <summary>
        /// resets only arrays "owned" by the node and related to inputs and
        /// output (i.e. if there is weight matrix, it will NOT be reset)
        /// </summary>
        abstract internal void resetInternalResultsArrays();

        /// <summary>
        /// Updates the output array based on the current input arrays
        /// </summary>
        /// <param name="sigID">The id for forward feeding signal (prevents infinite loops with a feedback node)</param>
        abstract internal void calculateResults(int sigID);

        /// <summary>
        /// make the input and output nodes point to eachother
        /// </summary>
        /// <param name="bottomInputNode"></param>
        /// <param name="topInputNode"></param>
        internal void updateInputNodesOutputNodes(ref AbstractNode bottomInputNode, ref AbstractNode topInputNode, bool topIsFeedBack = false, bool bottomIsFeedBack = false)
        {
            if (bottomInputNode != null)
            {
                this.BottomInputNode = bottomInputNode;
                if (bottomIsFeedBack)
                {                  
                    this.BottomInputNode.FeedbackNode = this;
                }
                else
                {
                    this.BottomInputNode.OutputNode = this;
                }
                
            }
            if (topInputNode != null)
            {
                this.TopInputNode = topInputNode;
                if (topIsFeedBack)
                {
                    this.TopInputNode.FeedbackNode = this;
                }
                else
                {
                    this.TopInputNode.OutputNode = this;
                }
            }
        }

        /// <summary>
        /// make the input and output nodes point to eachother
        /// </summary>
        /// <param name="topInputNode"></param>
        internal void updateInputNodesOutputNodes(ref AbstractNode topInputNode, bool topIsFeedBack = false)
        {
            this.BottomInputNode = null;
            if (topInputNode != null)
            {
                this.TopInputNode = topInputNode;
                if (topIsFeedBack)
                {
                    this.TopInputNode.FeedbackNode = this;
                }
                else
                {
                    this.TopInputNode.OutputNode = this;
                }
            }
        }


    }
}
