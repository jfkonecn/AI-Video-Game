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
            Output = layers[layers.Length].Output;
            for (int i = 1; i < layers.Length; i++)
            {
                ConnectNodes(layers[i - 1], layers[i], 0);
            }
        }

        public SeriesOfLayers(SeriesOfLayers old) : base(old)
        {

        }
    }
}
