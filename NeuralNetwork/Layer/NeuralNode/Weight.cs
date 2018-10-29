using NeuralNetwork.NeuralMath;
using NeuralNetwork.NeuralMath.Stats;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNetwork.Layer.NeuralNode
{
    /// <summary>
    /// Represents an array of values which only changes when trained
    /// </summary>
    public class Weight : BaseNode
    {
        
        /// <summary>
        /// Copy Constructor
        /// </summary>
        public Weight(Weight old) : base(old)
        {

        }
        
        /// <summary>
        /// Creates an output array with random values
        /// </summary>
        /// <param name="rows">total rows for output array</param>
        /// <param name="cols">total columns for output array</param>
        /// <param name="mean">The average value of the random elements</param>
        /// <param name="stdev">Standard deviation from the mean</param>
        public Weight(int rows, int cols, double mean = 0, double stdev = 0.1) : base()
        {
            OutputArray = new double[rows, cols];
            Matrix.PerformActionOnEachArrayElement(OutputArray,
                (indices) => OutputArray.SetValue(ZScore.RandomGaussianDistribution(mean, stdev), indices));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        public Weight(double[,] outputArray) : base()
        {
            OutputArray = outputArray;
        }

        protected override void DetermineInputNodeSensitivity()
        {
            if (InputNeighbors.Count != 0)
                throw new ArgumentException("No inputs are allowed!", nameof(InputNeighbors));
            // we shouldn't have inputs
            return;
        }

        protected override void InternalCalculate()
        {
            if (InputNeighbors.Count != 0)
                throw new ArgumentException("No inputs are allowed!", nameof(InputNeighbors));
            // No calculations done here
        }

        protected override void InternalLearn(double learningRate)
        {
            Matrix.ScalarMultiplication(-learningRate, Sensitivity);
            Matrix.Add(OutputArray, Sensitivity, OutputArray);
        }
    }
}
