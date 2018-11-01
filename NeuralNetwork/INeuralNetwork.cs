using System.Collections.Generic;
using NeuralNetwork.Layer;

namespace NeuralNetwork
{
    public interface INeuralNetwork
    {
        ILayer Network { get; set; }

        void BatchTrain(List<ITrainingPoint> trainingSet, double minRSquaredValue, double learningRate);
        double[] Calculate(double[] input);
        void IncrementalTrain(ITrainingPoint trainingPoint, double learningRate);
        void Mutate(double lowerValueLimit = -1, double upperValueLimit = 1, double stdDev = 0.1);
    }
}