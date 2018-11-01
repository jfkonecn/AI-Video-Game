using NeuralNetwork.Layer.NeuralNode;
using NeuralNetwork.NeuralMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNetwork.Layer
{
    public static class LayerFactory
    {
        public static ILayer Copy(ILayer oldLayer)
        {
            ILayer newLayer = (ILayer)Activator.CreateInstance(oldLayer.GetType());

            if (oldLayer == null)
                throw new ArgumentNullException(nameof(oldLayer));
            foreach (INeuralComponent oldNode in oldLayer.Nodes)
            {
                INeuralComponent newNode = null;
                if (oldNode is ILayer)
                    newNode = Copy((ILayer)oldNode);
                else if (oldNode is INode)
                    newNode = NodeFactory.Copy((INode)oldNode);
                else
                    throw new Exception("Unknown Type!");
                newLayer.AddNode(newNode);
                ILayer inOutCheckLayer = oldNode is ILayer ? (ILayer)oldNode : oldLayer;

                INeuralComponent inputCheck = oldNode is ILayer ? ((ILayer)oldNode).Input : oldNode,
                    outputCheck = oldNode is ILayer ? ((ILayer)oldNode).Output : oldNode;
                if (oldLayer.Input.Equals(inputCheck))
                {
                    newLayer.Input = newNode is Vector ? (Vector)newNode : ((ILayer)newNode).Input;
                }
                if (oldLayer.Output.Equals(outputCheck))
                {
                    newLayer.Output = newNode is Vector ? (Vector)newNode : ((ILayer)newNode).Output;
                }
            }
            if (newLayer.Nodes.Count != oldLayer.Nodes.Count || newLayer.Input == null || newLayer.Output == null)
                throw new Exception("something went wrong");

            for (int i = 0; i < newLayer.Nodes.Count; i++)
            {
                INeuralComponent newNode = newLayer.Nodes[i],
                    oldNode = oldLayer.Nodes[i];
                for (int j = 0; j < oldNode.InputNeighbors.Count; j++)
                {
                    int idx = oldLayer.Nodes.IndexOf(oldNode.InputNeighbors[j]);
                    newLayer.ConnectNodes(newLayer.Nodes[idx], newNode, oldNode.InputPriorities[j]);
                }
            }
            return newLayer;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="totalInputs">Number of inputs for this layer of neurons</param>
        /// <param name="totalOutputs">Number of outputs for this layers</param>
        /// <param name="hasBias"></param>
        /// <param name="transferFunction"></param>
        public static ILayer LayerOfNeurons(int totalInputs, int totalOutputs, bool hasBias, ITransferFunction transferFunction)
        {
            Weight weight = new Weight(totalOutputs, totalInputs),
                bias = hasBias ? new Weight(totalOutputs, 1) : null;
            return LayerOfNeurons(weight, bias, transferFunction);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="biases"></param>
        /// <param name="transferFunction"></param>
        public static ILayer LayerOfNeurons(double[,] weights, double[] biases, ITransferFunction transferFunction)
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
                    biasCpy[i, 0] = biases[i];
                }
                bias = new Weight(biasCpy);
            }

            Matrix.SetArraysEqualToEachOther(weights, weightCpy);
            Weight weight = new Weight(weightCpy);
            return LayerOfNeurons(weight, bias, transferFunction);
        }

        private static ILayer LayerOfNeurons(Weight weight, Weight bias, ITransferFunction transferFunction)
        {
            if (transferFunction == null)
                throw new ArgumentNullException(nameof(transferFunction));
            Vector inputVector = NodeFactory.VectorNode(), outputVector = NodeFactory.VectorNode();
            ILayer layer = new BaseLayer();
            INode multiplyNode = NodeFactory.MultiplyNode();
            layer.Nodes.Add(inputVector);
            layer.Nodes.Add(outputVector);
            layer.Nodes.Add(weight);
            layer.Nodes.Add(transferFunction);

            layer.Nodes.Add(multiplyNode);
            layer.Input = inputVector;
            layer.Output = outputVector;

            layer.ConnectNodes(inputVector, multiplyNode, 1);
            layer.ConnectNodes(weight, multiplyNode, 0);
            if (bias != null)
            {
                INode addNode = NodeFactory.AddNode();
                layer.Nodes.Add(bias);
                layer.Nodes.Add(addNode);
                layer.ConnectNodes(multiplyNode, addNode, 0);
                layer.ConnectNodes(bias, addNode, 0);
                layer.ConnectNodes(addNode, transferFunction, 0);
            }
            else
            {
                layer.ConnectNodes(multiplyNode, transferFunction, 0);
            }


            layer.ConnectNodes(transferFunction, outputVector, 0);
            return layer;
        }


        public static ILayer SeriesOfLayers(params ILayer[] layers)
        {
            ILayer layer = new BaseLayer();
            if (layers == null)
                throw new ArgumentNullException(nameof(layers));
            for (int i = 0; i < layers.Length; i++)
            {
                layer.AddNode(layers[i]);
            }
            layer.Input = layers[0].Input;
            layer.Output = layers[layers.Length - 1].Output;
            for (int i = 1; i < layers.Length; i++)
            {
                layer.ConnectNodes(layers[i - 1], layers[i], 0);
            }
            return layer;
        }
    }
}
