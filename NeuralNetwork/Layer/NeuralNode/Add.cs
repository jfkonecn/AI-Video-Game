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

        public override int MaxInputs => int.MaxValue;

        public override int MinInputs => 2;

        public override int MaxOutputs => int.MaxValue;

        public override int MinOutputs => 1;

        protected override void DetermineInputNodeSensitivity(Array sensitivity)
        {
            for (int i = 0; i < InputSensitivities.Count; i++)
            {
                if (InputSensitivities[i] == null)
                    InputSensitivities[i] = Matrix.CreateArrayWithMatchingDimensions(sensitivity);
                Matrix.SetArraysEqualToEachOther(sensitivity, InputSensitivities[i]);
            }
        }

        protected override void InternalCalculate()
        {
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
    }
}
