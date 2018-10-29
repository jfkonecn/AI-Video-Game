using NeuralNetwork.Layer.NeuralNode;
using NeuralNetwork.NeuralMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNetwork.Layer
{
    public class LayerOfNeurons : BaseLayer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="totalInputs">Number of inputs for this layer of neurons</param>
        /// <param name="totalOutputs">Number of outputs for this layers</param>
        /// <param name="hasBias"></param>
        /// <param name="transferFunction"></param>
        public LayerOfNeurons(int totalInputs, int totalOutputs, bool hasBias, TransferFunction transferFunction) : base()
        {
            Weight weight = new Weight(totalOutputs, totalInputs),
                bias = hasBias ? new Weight(totalOutputs, 1) : null;
            FinishUp(weight, bias, transferFunction);
        }
        /// <summary>
        /// Used for unit testing
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="biases"></param>
        /// <param name="transferFunction"></param>
        public LayerOfNeurons(double[,] weights, double[] biases, TransferFunction transferFunction)
        {
            if (weights == null)
                throw new ArgumentNullException(nameof(weights));

            
            if (weights.GetLength(0) != biases.Length)
                throw new ArgumentException("The number of rows in the weights must match the total elements in biases");
            double[,] weightCpy = (double[,])Matrix.CreateArrayWithMatchingDimensions(weights);

            Weight bias = null;
            if (biases != null)
            {
                double[,] biasCpy = new double[biases.Length, 1];
                for (int i = 0; i < biases.Length; i++)
                {
                    biasCpy[i, 1] = biases[i];
                }
                bias = new Weight(biasCpy);
            }           

            Matrix.SetArraysEqualToEachOther(weights, weightCpy);
            Weight weight = new Weight(weightCpy);                
            FinishUp(weight, bias, transferFunction);
        }

        private void FinishUp(Weight weight, Weight bias, TransferFunction transferFunction)
        {
            if (transferFunction == null)
                throw new ArgumentNullException(nameof(transferFunction));
            Vector inputVector = new Vector(), outputVector = new Vector();
            Add addNode = new Add();
            Multiply multiplyNode = new Multiply();
            Nodes.Add(inputVector);
            Nodes.Add(outputVector);
            Nodes.Add(weight);
            Nodes.Add(bias);
            Nodes.Add(transferFunction);
            Nodes.Add(addNode);
            Nodes.Add(multiplyNode);
            Input = inputVector;
            Output = outputVector;

            ConnectNodes(inputVector, multiplyNode, 0);
            ConnectNodes(weight, multiplyNode, 1);
            ConnectNodes(multiplyNode, addNode, 0);
            if (bias != null)
                ConnectNodes(bias, addNode, 0);
            ConnectNodes(addNode, transferFunction, 0);
            ConnectNodes(transferFunction, outputVector, 0);
        }
        /// <summary>
        /// Copy Constructor
        /// </summary>
        public LayerOfNeurons(LayerOfNeurons old) : base(old)
        {

        }
    }
}
