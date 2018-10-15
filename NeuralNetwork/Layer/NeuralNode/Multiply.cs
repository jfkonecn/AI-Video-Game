using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNetwork.Layer.NeuralNode
{
    public class Multiply : BaseNode
    {
        public override void DetermineInputNodeSensitivity()
        {
            throw new NotImplementedException();
        }

        public override void InternalTrain(double learningRate, Array sensitivity)
        {
            throw new NotImplementedException();
        }

        protected override void InternalCalculate()
        {
            throw new NotImplementedException();
        }

        protected override void ResetValue()
        {
            
        }
    }
}
