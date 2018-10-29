using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuralNetwork;
using NeuralNetwork.Layer;
using NeuralNetwork.Layer.NeuralNode;
using NeuralNetwork.NeuralMath;

namespace UnitTests
{
    [TestClass]
    public class BaseNetworkTests
    {
        [TestMethod]
        public void Calculate()
        {
            FeedForwardNetwork net = new FeedForwardNetwork(
                new double[,] { { 3, 2 } }, 
                new double[] { 1.2 }, TransferFunction.LogSigmoid);
            double[] expected = { (1d / (1d + Math.Exp(1.8))) };
            MatrixTestHelpers.AssertArraysAreEqual(expected, net.Calculate(new double[] { -5, 6 }));
            
        }
        /// <summary>
        /// Make sure no pointer sharing happens
        /// </summary>
        [TestMethod]
        public void NeuralNetworkCopy()
        {
            double[] testInput = new double[] { 1, -1, 0, 2 };
            FeedForwardNetwork oldNet = new FeedForwardNetwork(testInput.Length, 5, true, TransferFunction.LogSigmoid);
            FeedForwardNetwork newNet = new FeedForwardNetwork(oldNet);
            MatrixTestHelpers.AssertArraysAreEqual(oldNet.Calculate(testInput),
                newNet.Calculate(testInput));
            Assert.IsTrue(NetsAreCopies(oldNet, newNet, false));
            newNet.Mutate();
            Assert.IsTrue(!NetsAreCopies(oldNet, newNet, false));




        }
        /// <summary>
        /// True if the networks have the same nodes in the same place and same values
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        private bool NetsAreCopies(BaseNetwork A, BaseNetwork B, bool ignoreWeightValues)
        {
            return NodesAreCopies(A.Network, B.Network, ignoreWeightValues);
        }
        private bool NodesAreCopies(BaseNode A, BaseNode B, bool ignoreWeightValues)
        {
            if (!A.GetType().Equals(B.GetType()))
                return false;
            if(A is BaseLayer layerA)
            {
                BaseLayer layerB = (BaseLayer)B;
                for (int i = 0; i < layerA.Nodes.Count; i++)
                {
                    if (!NodesAreCopies(layerA.Nodes[i], layerB.Nodes[i], ignoreWeightValues))
                        return false;
                }
            }
            if(A is Weight)
            {
                for (int i = 0; i < A.OutputArray.Rank; i++)
                {
                    if (A.OutputArray.GetLength(i) != B.OutputArray.GetLength(i))
                        return false;
                }
                if(!ignoreWeightValues)
                    return MatrixTestHelpers.ArraysAreEqual(A.OutputArray, B.OutputArray);
            }
            return true;
        }
    }
}
