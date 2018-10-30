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

            for (int i = 1; i < layers.Length; i++)
            {
                ConnectNodes(layers[i - 1], layers[i], 0);
            }
            Input = FirstLayer.Input;
            Output = LastLayer.Output;
        }

        public SeriesOfLayers(SeriesOfLayers old) : base(old)
        {

        }

        private BaseLayer LastLayer { get => (BaseLayer)Nodes[Nodes.Count - 1]; }
        private BaseLayer FirstLayer { get => (BaseLayer)Nodes[0]; }

    }
}
