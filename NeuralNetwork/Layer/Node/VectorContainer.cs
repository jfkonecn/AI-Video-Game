using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNetwork.Layer.Node
{
    internal abstract class VectorContainer
    {
        /// <summary>
        /// Vector
        /// </summary>
        /// <param name="vector">This pointer will be shared with the Vector pr</param>
        protected VectorContainer(double[] vector)
        {
            Vector = vector;
        }
        public double[] Vector { get; }
    }
}
