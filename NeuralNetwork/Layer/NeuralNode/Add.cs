using NeuralNetwork.NeuralMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNetwork.Layer.NeuralNode
{
    /// <summary>
    /// A Pointwise Operation which adds exactly two Nodes
    /// </summary>
    public class Add : BaseNode
    {

        public Add() : base()
        {

        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        public Add(Add old) : base(old)
        {

        }

        protected override void DetermineInputNodeSensitivity()
        {
            for (int i = 0; i < InputSensitivities.Count; i++)
            {
                if (InputSensitivities[i] == null)
                    InputSensitivities[i] = Matrix.CreateArrayWithMatchingDimensions(Sensitivity);
                Matrix.SetArraysEqualToEachOther(Sensitivity, InputSensitivities[i]);
            }
        }

        protected override void InternalCalculate()
        {
            if (InputNeighbors.Count != 2)
                throw new ArgumentException("Must have exactly two inputs!", nameof(InputNeighbors));
            try
            {
                Matrix.Add(InputNeighbors[0].OutputArray, InputNeighbors[1].OutputArray, OutputArray);
            }
            catch
            {
                // OuputArray is null or the dimensions are wrong
                // If we fail here then the user gave this node a bad input
                OutputArray = Matrix.Add(InputNeighbors[0].OutputArray, InputNeighbors[1].OutputArray);
                Sensitivity = Matrix.CreateArrayWithMatchingDimensions(OutputArray);
            }
        }

        protected override void InternalTrain(double learningRate, Array sensitivity)
        {
            Matrix.SetArraysEqualToEachOther(sensitivity, Sensitivity);
        }
    }
}
