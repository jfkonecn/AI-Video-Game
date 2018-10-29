using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuralNetwork;
using NeuralNetwork.Layer;
using NeuralNetwork.Layer.NeuralNode;

namespace UnitTests
{
    [TestClass]
    public class BaseNetworkTests
    {
        [TestMethod]
        public void Calculate()
        {
            BaseNetwork net = new BaseNetwork(
                new LayerOfNeurons(new double[,] { { 3, 2 } }, 
                new double[] { 1.2 }, TransferFunction.LogSigmoid));
            double[] expected = { (1d / (1d + Math.Exp(1.8))) };
            MatrixTestHelpers.AssertArraysAreEqual(expected, net.Calculate(new double[] { -5, 6 }));
            
        }
    }
}
