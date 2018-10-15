﻿using NeuralNetwork.NeuralMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNetwork.Layer.NeuralNode
{
    /// <summary>
    /// Multiplies exactly two nodes
    /// </summary>
    public class Multiply : BaseNode
    {
        protected override void DetermineInputNodeSensitivity()
        {
            FindLeftAndRightIndex(out int leftIdx, out int rightIdx);
            try
            {
                Matrix.Multiply(Sensitivity, Matrix.Transpose(InputNeighbors[rightIdx].OutputArray), InputSensitivities[leftIdx]);
                Matrix.Multiply(Sensitivity, Matrix.Transpose(InputNeighbors[leftIdx].OutputArray), InputSensitivities[rightIdx]);
            }
            catch
            {
                InputSensitivities[leftIdx] = Matrix.Multiply(Sensitivity, Matrix.Transpose(InputNeighbors[rightIdx].OutputArray));
                InputSensitivities[rightIdx] = Matrix.Multiply(Sensitivity, Matrix.Transpose(InputNeighbors[leftIdx].OutputArray));
            }
        }

        protected override void InternalTrain(double learningRate, Array sensitivity)
        {
            Matrix.SetArraysEqualToEachOther(sensitivity, Sensitivity);
        }

        protected override void InternalCalculate()
        {
            FindLeftAndRightIndex(out int leftIdx, out int rightIdx);
            Array left = InputNeighbors[leftIdx].OutputArray, 
                right = InputNeighbors[rightIdx].OutputArray;
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

        private void FindLeftAndRightIndex(out int leftIdx, out int rightIdx)
        {
            if (InputNeighbors.Count != 2)
                throw new ArgumentException("Must have exactly two inputs!", nameof(InputNeighbors));
            if (InputPriorities[0] <= InputPriorities[1])
            {
                leftIdx = 0;
                rightIdx = 1;
            }
            else
            {
                leftIdx = 1;
                rightIdx = 0;
            }
        }


    }
}
