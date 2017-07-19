using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuralNetwork.Node;

namespace NeuralNetwork
{
    abstract public class AbstractNet
    {
        /****************************************************************************
        * Properties
        *****************************************************************************/
        //output node
        OutputNode _OutputNode;

        /// <summary>
        /// only one change to the output node is allowed
        /// </summary>
        bool _OutputNodeUpdated = false;

        /// <summary>
        /// To issues with program later done the line, the outputNode can only
        /// be set once
        /// </summary>
        internal OutputNode OutputNode
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

        //input node
        InputNode _InputNode;
        /// <summary>
        /// only one change to the input node is allowed
        /// </summary>
        bool _InputNodeUpdated = false;

        /// <summary>
        /// To issues with program later done the line, the inputNode can only
        /// be set once
        /// </summary>
        internal InputNode InputNode
        {
            get { return _InputNode; }
            set
            {
                if (!_InputNodeUpdated)
                {
                    _InputNodeUpdated = true;
                    _InputNode = value;
                }
                else
                {
                    throw new System.InvalidOperationException("InputNode can only be changed once!");
                }
            }
        }

        /// <summary>
        /// output for the entire net
        /// </summary>
        public double[] OutputArray
        {
            get
            {
                return _OutputNode.OutputArray;
            }
        }

        /// <summary>
        /// input for the entire net
        /// </summary>
        public double[] InputArray
        {
            set
            {
                _InputNode.InputArray = value;
            }
            get
            {
                return _InputNode.InputArray;
            }
        }


        /****************************************************************************
        * Methods
        *****************************************************************************/
        /// <summary>
        /// Calculates neural net based on the values of input array. Stores results
        /// in outputArray.
        /// </summary>
        public void calculateResults()
        {
            this.InputNode.calculateResults();
        }
        public void adjustWeights()
        {
            this.InputNode.adjustWeights();
            return;
        }

        /// <summary>
        /// creates an exact copy of a neural net
        /// </summary>
        /// <param name="net"></param>
        /// <returns></returns>
        public abstract AbstractNet copy();
    }
}