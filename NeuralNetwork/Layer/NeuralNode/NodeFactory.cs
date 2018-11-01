using NeuralNetwork.NeuralMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNetwork.Layer.NeuralNode
{
    public static class NodeFactory
    {
        public static INode Copy(INode oldNode)
        {
            INode newNode = (INode)Activator.CreateInstance(oldNode.GetType());
            if (oldNode.OutputArray != null)
            {                
                newNode.OutputArray = Matrix.CreateArrayWithMatchingDimensions(oldNode.OutputArray);
                Matrix.PerformActionOnEachArrayElement(newNode.OutputArray,
                    (indices) => newNode.OutputArray.SetValue(oldNode.OutputArray.GetValue(indices), indices));
            }
            if(newNode is ITransferFunction transNode)
            {
                transNode.Fx = ((ITransferFunction)oldNode).Fx;
                transNode.FxPrime = ((ITransferFunction)oldNode).FxPrime;
            }
            return newNode;
        }
        public static Add AddNode()
        {
            return new Add();
        }
        public static Multiply MultiplyNode()
        {
            return new Multiply();
        }
        public static RecurrentVector RecurrentVector()
        {
            return new RecurrentVector();
        }
        public static ITransferFunction LogSigmoidTransferFunction()
        {
            return new TransferFunction(x => 1d / (1d + Math.Exp(-1d * x)),
                x => Math.Exp(-1d * x) / Math.Pow(1d + Math.Exp(-1d * x), 2));
        }
        public static ITransferFunction PureLineTransferFunction()
        {
            return new TransferFunction(x => x, x => 1);
        }
        public static Vector VectorNode()
        {
            return new Vector();
        }
        public static Weight WeightNode(int rows, int cols, double mean = 0, double stdev = 0.1)
        {
            return new Weight(rows, cols, mean, stdev);
        }
        public static Weight WeightNode(double[,] weights)
        {
            return new Weight(weights);
        }
    }
}
