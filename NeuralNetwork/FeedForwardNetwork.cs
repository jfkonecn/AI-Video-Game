using NeuralNetwork.Layer;
using NeuralNetwork.Layer.NeuralNode;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNetwork
{
    public class FeedForwardNetwork : BaseNetwork
    {
        protected FeedForwardNetwork() : base()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="totalInputs">Number of inputs for this layer of neurons</param>
        /// <param name="totalOutputs">Number of outputs for this layers</param>
        /// <param name="hasBias"></param>
        /// <param name="transferFunction"></param>
        public FeedForwardNetwork(int totalInputs, int totalOutputs, bool hasBias, TransferFunction transferFunction) 
            : base(new LayerOfNeurons(totalInputs, totalOutputs, hasBias, transferFunction))
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="biases"></param>
        /// <param name="transferFunction"></param>
        public FeedForwardNetwork(double[,] weights, double[] biases, TransferFunction transferFunction) 
            : base(new LayerOfNeurons(weights, biases, transferFunction))
        {

        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        public FeedForwardNetwork(FeedForwardNetwork old) : base(old)
        {

        }
    }
}
