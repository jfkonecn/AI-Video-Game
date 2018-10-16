using NeuralNetwork.NeuralMath;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace NeuralNetwork.Layer.NeuralNode
{
    /// <summary>
    /// 
    /// </summary>
    public class RecurrentVector : Vector
    {
        protected override void DetermineInputNodeSensitivity()
        {
            // do nothing because this is an input
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void InternalCalculate()
        {
            if (InputNeighbors.Count == 0 || OutputNeighbors.Count == 0)
                throw new ArgumentException($"{nameof(RecurrentVector)} must have atleast one input and one output");
            if(OutputArray == null)
            {
                // This is our first calculation
                int totalElements = 0;
                foreach (BaseNode node in InputNeighbors)
                    totalElements += node.OutputArray.GetLength(1);
                OutputArray = new double[totalElements, 1];
            }
            else
            {
                base.InternalCalculate();
            }           
        }

        protected override void InternalTrain(double learningRate, Array sensitivity)
        {
            // do nothing because this is an input
        }

    }
}
