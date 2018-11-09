using NeuralNetwork.Layer;
using NeuralNetwork.NeuralMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNetwork
{
    public class BaseNetwork : INeuralNetwork
    {
        public BaseNetwork()
        {

        }


        public ILayer Network { get; set; }

        /// <summary>
        /// Determines the error for all points in the collection then updates the weights and biases using the mean square error
        /// <para>Algorithm is based on Neural Network Design 2nd Edition Chapter 11's algorithm</para>
        /// </summary>
        /// <param name="trainingSet">All training points</param>
        /// <param name="minRSquaredValue">The minimum allowed R^2 value for any of the outputs
        /// <para>Note: Must be greater than 0, but less than or equal to 1</para> </param>
        /// <param name = "learningRate" > A fraction which determines how big of a step the change in the weights and biases will be along the gradient
        /// <para>Note: Must be greater than or equal to -1, but less than or equal to 1</para> 
        /// <para>Negative learning rate means the network is being "punished"</para></param>
        /// <exception cref="MaxTriesReachedException"></exception>
        public void BatchTrain(List<ITrainingPoint> trainingSet, double minRSquaredValue, double learningRate)
        {
            if (minRSquaredValue <= 0 || minRSquaredValue > 1)
            {
                throw new ArgumentOutOfRangeException("Must be between 1 and 0", nameof(minRSquaredValue));
            }
            else if (learningRate < -1 || learningRate > 1)
            {
                throw new ArgumentOutOfRangeException("Must be between -1 and 1", nameof(learningRate));
            }
            double[] totalSumOfSquares = TotalSumOfSquares(trainingSet);
            int j = 0;
            Network.Reset();
            while(j < MAX_TRIES)
            {
                double[] residualSumOfSquares = new double[totalSumOfSquares.Length];
                // sum all derivatives
                foreach (TrainingPoint trainingPoint in trainingSet)
                {
                    Calculate(trainingPoint.Input);
                    Matrix.Add(residualSumOfSquares,
                        SquareOfResiduals(trainingPoint.ExpectedOutput, Network.Output.GetOutputArrayAsVector()),
                        residualSumOfSquares);
                    Network.UpdateSensitivities(CalculateErrorDerivative(trainingPoint), TrainingMode.Batch);
                }
                // check if we reached our goal
                double[] rSquared = new double[residualSumOfSquares.Length];
                for (int i = 0; i < residualSumOfSquares.Length; i++)
                {
                    rSquared[i] = 1 - (residualSumOfSquares[i] / totalSumOfSquares[i]);
                    if (minRSquaredValue > rSquared[i])
                    {
                        // no need to continue we have to loop again
                        break;
                    }
                    if (i == residualSumOfSquares.Length)
                        return;
                }
                Network.Learn(learningRate / trainingSet.Count);
                j++;
            }
            throw new MaxTriesReachedException();
        }

        /// <summary>
        /// Determines the errors for the training point and then updates the weights and biases a single time
        /// <para>Algorithm is based on Neural Network Design 2nd Edition Chapter 11's algorithm</para>
        /// </summary>
        /// <param name="trainingPoint"></param>
        /// <param name = "learningRate" > A fraction which determines how big of a step the change in the weights and biases will be along the gradient
        /// <para>Note: Must be greater than or equal to -1, but less than or equal to 1</para> 
        /// <para>Negative learning rate means the network is being "punished"</para></param>
        public void IncrementalTrain(ITrainingPoint trainingPoint, double learningRate)
        {
            if (learningRate < -1 || learningRate > 1)
            {
                throw new ArgumentOutOfRangeException("learningRate");
            }
            Calculate(trainingPoint.Input);
            Network.UpdateSensitivities(CalculateErrorDerivative(trainingPoint), TrainingMode.Incremental);
            Network.Learn(learningRate);
        }

        /// <summary>
        /// Calculates error derivative with respect to this network's output
        /// </summary>
        /// <param name="trainingPoint"></param>
        /// <returns></returns>
        private double[,] CalculateErrorDerivative(ITrainingPoint trainingPoint)
        {
            double[,] temp = (double[,])Matrix.CreateArrayWithMatchingDimensions(Network.Output.OutputArray);            
            double[,] expected = new double[trainingPoint.ExpectedOutput.Length, 1];
            Matrix.SetArraysEqualToEachOther(Network.Output.OutputArray, temp);
            Matrix.ScalarMultiplication(-1, temp);
            Matrix.PerformActionOnEachArrayElement(expected, (idx) => expected.SetValue(trainingPoint.ExpectedOutput[idx[0]], idx));
            Matrix.Add(expected, temp, temp);
            Matrix.ScalarMultiplication(-2, temp);
            return temp;
        }

        /// <summary>
        /// Performs the calculation for the neural network
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public double[] Calculate(double[] input)
        {
            Network.Input.SetOutputArrayWithVector(input);
            Network.Calculate(this);
            return Network.Output.GetOutputArrayAsVector();
        }

        /// <summary>
        /// Randomly Changes the weights in this network
        /// </summary>
        /// <param name="lowerValueLimit">The lower limit for the weights</param>
        /// <param name="upperValueLimit">The upper limit for the weights</param>
        /// <param name="stdDev">The standard deviation for changes to the weights and biases</param>
        public void Mutate(double lowerValueLimit = -1, double upperValueLimit = 1, double stdDev = 0.1)
        {
            Network.Mutate(lowerValueLimit, upperValueLimit, stdDev);
        }

        /// <summary>
        /// https://en.wikipedia.org/wiki/Total_sum_of_squares
        /// </summary>
        /// <param name="trainingPoints"></param>
        /// <returns>Total sum of squares for the training points</returns>
        /// <exception cref="ArgumentException"></exception>
        private double[] TotalSumOfSquares(List<ITrainingPoint> trainingPoints)
        {
            double[] meanOfExpected = new double[Network.Output.OutputArray.GetLength(1)];

            // find the mean of the expected points
            foreach (TrainingPoint point in trainingPoints)
            {
                if (point.ExpectedOutput.Length != meanOfExpected.Length)
                {
                    throw new ArgumentException("All training point arrays be the same as the number of rows in the last hidden layer's weight rows");
                }

                for (int i = 0; i < meanOfExpected.Length; i++)
                {
                    meanOfExpected[i] += point.ExpectedOutput[i];
                }
            }

            for (int i = 0; i < meanOfExpected.Length; i++)
            {
                meanOfExpected[i] /= trainingPoints.Count;
            }

            double[] totalSumOfSquares = new double[meanOfExpected.Length];

            // calculate the total sum of squares
            foreach (TrainingPoint point in trainingPoints)
            {
                for (int i = 0; i < meanOfExpected.Length; i++)
                {
                    totalSumOfSquares[i] += Math.Pow(meanOfExpected[i] - point.ExpectedOutput[i], 2);
                }
            }

            return totalSumOfSquares;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        private double[] SquareOfResiduals(double[] expected, double[] actual)
        {
            if (expected.Length != actual.Length)
            {
                throw new ArgumentException("Expected and Actual must be the same size");
            }

            double[] squareOfResiduals = new double[expected.Length];
            for (int i = 0; i < squareOfResiduals.Length; i++)
            {
                squareOfResiduals[i] = Math.Pow(expected[i] - actual[i], 2);
            }

            return squareOfResiduals;
        }

        /// <summary>
        /// The maximum tries allowed to optimize the weights and biases in the neural network
        /// </summary>
        private static readonly int MAX_TRIES = (int)1e6;
    }

    public class MaxTriesReachedException : Exception { }
}
