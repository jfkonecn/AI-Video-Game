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
                new double[] { 1.2 }, TransferFunction.LogSigmoid());
            double[] expected = { (1d / (1d + Math.Exp(1.8))) };
            double[] input = new double[] { -5, 6 };
            for(int i = 0; i < 2; i++)
                MatrixTestHelpers.AssertArraysAreEqual(expected, net.Calculate(input));
            
        }
        /// <summary>
        /// Make sure no pointer sharing happens
        /// </summary>
        [DataTestMethod]
        [DataRow(new double[] { 1, -1, 0, 2 })]
        public void NetworkCopy(double[] testInput)
        {            
            FeedForwardNetwork oldNet = new FeedForwardNetwork(testInput.Length, 5, true, TransferFunction.LogSigmoid());
            FeedForwardNetwork newNet = new FeedForwardNetwork(oldNet);
            MatrixTestHelpers.AssertArraysAreEqual(oldNet.Calculate(testInput),
                newNet.Calculate(testInput));
            Assert.IsTrue(NetsAreCopies(oldNet, newNet, false));
            newNet.Mutate();
            Assert.IsTrue(!NetsAreCopies(oldNet, newNet, false));
        }

        /// <summary>
        /// Tests the training methods of the neural network
        /// </summary>
        [TestMethod]        
        public void NetworkIncrementalTrain()
        {
            BaseNetwork net = new BaseNetwork(
                new LayerOfNeurons(new double[,] { { -0.27 }, { -0.41 } }, new double[] { -0.48, -0.13 }, TransferFunction.LogSigmoid()), 
                new LayerOfNeurons(new double[,] { { 0.09, -0.17 } }, new double[] { 0.48 }, TransferFunction.PureLine()));
            TrainingPoint point = new TrainingPoint(new double[] { 1 }, new double[] { 1 + Math.Sin(Math.PI / 4) });
            double[] expected = new double[] { 0.44628202808935191 };

            MatrixTestHelpers.AssertArraysAreEqual(net.Calculate(point.Input), expected);
            net.IncrementalTrain(point, 0.1);
            MatrixTestHelpers.AssertArraysAreEqual(net.Calculate(point.Input), new double[] { 0.75931135950548934 });


            // make sure we don't crash without a bias
            net = new FeedForwardNetwork(1, 1, true, TransferFunction.LogSigmoid());
            net.IncrementalTrain(point, 1);
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
        private bool NodesAreCopies(INeuralNode A, INeuralNode B, bool ignoreWeightValues)
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
