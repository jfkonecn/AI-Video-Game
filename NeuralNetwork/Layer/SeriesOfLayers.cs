using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNetwork.Layer
{
    public class SeriesOfLayers : BaseLayer
    {
        protected SeriesOfLayers() : base()
        {

        }

        public SeriesOfLayers(params BaseLayer[] layers) : base()
        {
            if (layers == null)
                throw new ArgumentNullException(nameof(layers));
            for (int i = 0; i < layers.Length; i++)
            {
                AddNode(layers[i]);
            }
            Input = FirstLayer.Input;
            Output = layers[layers.Length - 1].Output;
            for (int i = 1; i < layers.Length; i++)
            {
                ConnectNodes(layers[i - 1], layers[i], 0);
            }
        }

        public SeriesOfLayers(SeriesOfLayers old) : base(old)
        {

        }

        protected override void SetInputOutputOnCopy(BaseLayer old)
        {
            Input = FirstLayer.Input;
            Output = LastLayer.Output;
        }

        public override void Calculate()
        {
            FirstLayer.Calculate();
        }
        public override void Learn(double learningRate)
        {
            LastLayer.Learn(learningRate);
        }
        public override void UpdateSensitivities(Array sensitivity, TrainingMode trainingMode)
        {
            LastLayer.UpdateSensitivities(sensitivity, trainingMode);
        }

        private BaseLayer LastLayer { get => (BaseLayer)Nodes[Nodes.Count - 1]; }
        private BaseLayer FirstLayer { get => (BaseLayer)Nodes[0]; }
    }
}
