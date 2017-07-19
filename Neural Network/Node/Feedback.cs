using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.Node
{
    class Feedback : AbstractNode
    {

        Feedback()
        {
            this.SigIDSet = false;
        }
        /****************************************************************************
        * Properties
        *****************************************************************************/
        private int _SigID;
            
        /// <summary>
        /// An output which will feedback in the net
        /// </summary>
        private int SigID
        {
            get
            {
                return _SigID;
            }
            set
            {
                if (this.SigIDSet)
                {
                    throw new System.InvalidOperationException("SigIDSet can only be changed once!");
                }
                else
                {
                    _SigID = value;
                }
            }
        }
        private bool SigIDSet { get; set; }

        /****************************************************************************
        * Methods
        *****************************************************************************/
        internal override void adjustWeights(int sigID)
        {
            setISigID(sigID);
            if (this.SigID != sigID)
            {
                this.OutputNode.adjustWeights(sigID);
                this.FeedbackNode.adjustWeights(this.SigID);
            }
        }

        internal override void flush(int sigID)
        {
            setISigID(sigID);
            if (this.SigID != sigID)
            {
                this.OutputNode.flush(sigID);
                this.FeedbackNode.flush(this.SigID);
            }

        }

        internal override void calculateResults(int sigID)
        {
            setISigID(sigID);
            if(this.SigID != sigID)
            {
                this.OutputNode.flush(sigID);
                this.FeedbackNode.flush(this.SigID);
            }


        }

        internal override void resetInternalResultsArrays()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputNode"></param>
        internal void setInputNode(ref AbstractNode inputNode)
        {
            if(this.TopInputNode != null)
            {
                throw new System.ArgumentException("Can only set the inputNode once!", "TopInputNode");
            }
            this.updateInputNodesOutputNodes(ref inputNode);
            this.setOutputArray = inputNode.OutputArray;
        }

        /// <summary>
        /// 
        /// </summary>
        private void setISigID(int incommingSigID)
        {
            if(!this.SigIDSet)
            {
                this.SigID = 1 + incommingSigID;
            }
        }
    }
}
