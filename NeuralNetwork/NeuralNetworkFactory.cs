using NeuralNetwork.Layer;
using NeuralNetwork.Layer.NeuralNode;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNetwork
{
    public static class NeuralNetworkFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="net"></param>
        /// <returns></returns>
        public static INeuralNetwork Copy(INeuralNetwork oldNet)
        {
            INeuralNetwork newNet = (INeuralNetwork)Activator.CreateInstance(oldNet.GetType());
            newNet.Network = LayerFactory.Copy(oldNet.Network);
            return newNet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="totalInputs">Number of inputs for this layer of neurons</param>
        /// <param name="totalOutputs">Number of outputs for this layers</param>
        /// <param name="hasBias"></param>
        /// <param name="transferFunction"></param>
        public static INeuralNetwork FeedForwardNetwork(int totalInputs, int totalOutputs, bool hasBias, ITransferFunction transferFunction)
        {
            return SimpleNetwork(LayerFactory.LayerOfNeurons(totalInputs, totalOutputs, hasBias, transferFunction));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="totalInputs">Number of inputs for this layer of neurons</param>
        /// <param name="totalOutputs">Number of outputs for this layers</param>
        /// <param name="hasBias"></param>
        /// <param name="transferFunction"></param>
        public static INeuralNetwork FeedForwardNetwork(double[,] weights, double[] biases, ITransferFunction transferFunction)
        {
            return SimpleNetwork(LayerFactory.LayerOfNeurons(weights, biases, transferFunction));
        }

        /// <summary>
        /// Connects all the layers in series inside a neural network
        /// </summary>
        /// <param name="layers"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static INeuralNetwork SimpleNetwork(params ILayer[] layers)
        {
            if (layers == null)
                throw new ArgumentNullException(nameof(layers));
            INeuralNetwork net = new BaseNetwork()
            {
                Network = LayerFactory.SeriesOfLayers(layers)
            };
            return net;
        }
    }
}
