using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNetwork.Layer
{
    public abstract class Layer
    {
        /// <summary>
        /// Total number of input layers into this layer
        /// </summary>
        protected abstract uint TotalInputs { get; }
        /// <summary>
        /// Total number of outputs layers from this layer
        /// </summary>
        protected abstract uint TotalOutputs { get; }
        /// <summary>
        /// Create exact copy of the passed layer
        /// </summary>
        /// <param name="oldLayer">Layer to be copied</param>
        /// <returns>New Copy</returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal abstract Layer CopyLayer(Layer oldLayer);
        /// <summary>
        /// Randomly Changes the weights and biases in this layer
        /// </summary>
        /// <param name="lowerValueLimit">The lower limit for the weights</param>
        /// <param name="upperValueLimit">The upper limit for the weights</param>
        /// <param name="stdDev">The standard deviation for changes to the weights and biases</param>
        internal abstract void Mutate(double lowerValueLimit, double upperValueLimit, double stdDev);
    }
}
