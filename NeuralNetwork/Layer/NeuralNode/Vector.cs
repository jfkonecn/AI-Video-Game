using NeuralNetwork.NeuralMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNetwork.Layer.NeuralNode
{
    /// <summary>
    /// Stores an array of values. Can be treated as an input, output or can concatenate two input vectors
    /// </summary>
    public class Vector : BaseNode
    {
        public Vector() : base()
        {

        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        public Vector(Vector old) : base(old)
        {

        }

        protected override void DetermineInputNodeSensitivity()
        {
            int offset = 0;

            for (int i = 0; i < InputNeighbors.Count; i++)
            {
                BaseNode node = (BaseNode)InputNeighbors[i];
                if (InputNeighbors.Count > 1 && !(node is Vector))
                    throw new ArgumentException("All inputs to a vector node must be a vector");
                if (InputSensitivities[i] == null || InputSensitivities[i].GetLength(1) != node.OutputArray.GetLength(1))
                    InputSensitivities[i] = new double[InputNeighbors[i].OutputArray.GetLength(1), 1];
                for (int j = 0; j < node.OutputArray.GetLength(1); j++)
                    ((double[,])InputSensitivities[i])[j, 0] = ((double[,])Sensitivity)[offset + j, 0];
                offset += node.OutputArray.GetLength(1);
            }
        }

        protected override void InternalCalculate()
        {
            if (InputNeighbors.Count == 0 && OutputArray == null)
                throw new ArgumentNullException("If this is intended to be an input vector then use the " +
                    "SetOutputArrayWithVector method before calculating. If vector is intended to have an input, " +
                    "then use a layer object to connect it with another node.", nameof(OutputArray));
            if (InputNeighbors.Count == 0)
                return;
            int totalElements = 0;
            foreach (BaseNode node in InputNeighbors)
            {
                if (!(node is Vector) && InputNeighbors.Count != 1)
                    throw new ArgumentException("All inputs to a vector node must be a vector, if there is more than one input!");
                totalElements += node.OutputArray.GetLength(0);
            }
            if(OutputArray == null || OutputArray.GetLength(0) != totalElements)
            {
                OutputArray = new double[totalElements, 1];
                Sensitivity = new double[totalElements, 1];
            }
            int offset = 0;
            foreach (BaseNode node in InputNeighbors)
            {
                for (int i = 0; i < node.OutputArray.GetLength(0); i++)
                    ((double[,])OutputArray)[offset + i, 0] = ((double[,])node.OutputArray)[i, 0];
                offset += node.OutputArray.GetLength(0);
            }
        }


        /// <summary>
        /// Cannot be set if this node has any inputs
        /// </summary>
        /// <param name="vector"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void SetOutputArrayWithVector(double[] vector)
        {
            if (InputNeighbors.Count != 0)
                throw new InvalidOperationException("Cannot set OutputArray if there are inputs into this node");
            OutputArray = new double[vector.Length, 1];
            for (int i = 0; i < vector.Length; i++)
                ((double[,])OutputArray)[i, 0] = vector[i];
        }

        /// <summary>
        /// Cannot be set if this node has any inputs
        /// </summary>
        /// <returns></returns>
        public double[] GetOutputArrayAsVector()
        {
            if (OutputNeighbors.Count != 0)
                throw new InvalidOperationException("Cannot get OutputArray if there are outputs from this node");
            double[] vector = new double[OutputArray.GetLength(0)];
            for (int i = 0; i < vector.Length; i++)
                vector[i] = ((double[,])OutputArray)[i, 0];
            return vector;
        }
    }
}
