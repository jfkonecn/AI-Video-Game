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
    public class RecurrentVector : Vector, IRecurrent
    {
        public RecurrentVector() : base()
        {

        }

        public RecurrentVector(int totalElements) : base(totalElements)
        {

        }

        protected override void DetermineInputNodeSensitivity(Array sensitivity)
        {
            // do nothing because this is an input
        }


        protected override void InternalUpdateSensitivities(Array sensitivity, TrainingMode trainingMode)
        {
            // do nothing because this is an input
        }
        public override int MinInputs => 1;
        public override int MinOutputs => 1;
    }
}
