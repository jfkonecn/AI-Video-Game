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
            Input = layers[0].Input;
            Output = layers[layers.Length - 1].Output;
            Layers = layers;
            for (int i = 1; i < layers.Length; i++)
            {
                ConnectNodes(layers[i - 1], layers[i], 0);
            }
        }

        public SeriesOfLayers(SeriesOfLayers old) : base(old)
        {

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

        protected BaseLayer[] Layers { get; set; } = null;

        private BaseLayer LastLayer { get => Layers[Layers.Length - 1]; }
        private BaseLayer FirstLayer { get => Layers[0]; }
    }
}
