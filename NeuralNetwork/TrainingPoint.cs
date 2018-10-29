using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNetwork
{
    public class TrainingPoint
    {
        TrainingPoint(double[] input, double[] expectedOutput)
        {
            Input = input;
            ExpectedOutput = expectedOutput;
        }
        public double[] Input { get; }
        public double[] ExpectedOutput { get; }
    }
}
