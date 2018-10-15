using NeuralNetwork.NeuralMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNetwork.Layer.NeuralNode
{
    public class Multiply : BaseNode
    {
        protected override void DetermineInputNodeSensitivity()
        {
            FindLeftAndRightArrays(out BaseNode left, out BaseNode right);
            
        }

        protected override void InternalTrain(double learningRate, Array sensitivity)
        {
            Matrix.SetArraysEqualToEachOther(sensitivity, Sensitivity);
        }

        protected override void InternalCalculate()
        {
            FindLeftAndRightArrays(out BaseNode leftNode, out BaseNode rightNode);
            Array left = leftNode.OutputArray, right = rightNode.OutputArray;
            try
            {
                Matrix.Multiply(left, right, OutputArray);
            }
            catch
            {
                // OuputArray is null or the dimensions are wrong
                // If we fail here then the user gave this node a bad input
                OutputArray = Matrix.Multiply(left, right);
                Sensitivity = Matrix.CreateArrayWithMatchingDimensions(OutputArray);
            }
        }

        private void FindLeftAndRightArrays(out BaseNode left, out BaseNode right)
        {
            if (InputNeighbors.Count != 2)
                throw new ArgumentException("Must have exactly two inputs!", nameof(InputNeighbors));
            if (InputPriorities[0] <= InputPriorities[1])
            {
                left = InputNeighbors[0];
                right = InputNeighbors[1];
            }
            else
            {
                left = InputNeighbors[1];
                right = InputNeighbors[0];
            }
        }

        protected override void ResetValue()
        {
            if (OutputArray == null)
                return;
            Matrix.SetAll(OutputArray, 0);
        }
    }
}
