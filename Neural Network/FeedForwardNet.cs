using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuralNetwork.Node;
using NeuralNetwork.Node.Functions;

namespace NeuralNetwork
{
    public class FeedForwardNet:AbstractNet
    {
        /// <summary>
        /// Testing
        /// </summary>
        /// <param name="numInputs"></param>
        /// <param name="numOutputs"></param>
        /// <param name="numLayers"></param>
        public FeedForwardNet(int numInputs, int numOutputs, int numLayers)
        {
            AbstractNode abNode;
            this.InputNode = new InputNode(numInputs);
            abNode = (AbstractNode)this.InputNode;
            LayerOfNeurons curLayer = new LayerOfNeurons(ref abNode, numOutputs, ActivationFunctions.defaultActivationFunction);
            for (int i = 0; i < numLayers - 1; i++)
            {
                abNode = (AbstractNode)curLayer;
                curLayer = new LayerOfNeurons(ref abNode, numOutputs, ActivationFunctions.defaultActivationFunction);
            }
            abNode = (AbstractNode)curLayer;
            this.OutputNode = new OutputNode(ref abNode);
        }

        public FeedForwardNet(ref FeedForwardNet copyNet)
        {

            AbstractNode abNode;
            InputNode inNode = copyNet.InputNode;
            this.InputNode = new InputNode(ref inNode);
            abNode = (AbstractNode)this.InputNode;
            //original layer
            LayerOfNeurons curOrgLayer = (LayerOfNeurons)inNode.OutputNode;
            //copy layer
            LayerOfNeurons curCpyLayer = new LayerOfNeurons(ref curOrgLayer, ref abNode);
            while (curOrgLayer.OutputNode.GetType() == typeof(LayerOfNeurons))
            {
                curOrgLayer = (LayerOfNeurons)curOrgLayer.OutputNode;
                abNode = (AbstractNode)curCpyLayer;
                curCpyLayer = new LayerOfNeurons(ref curOrgLayer, ref abNode);
            }
            abNode = (AbstractNode)curCpyLayer;
            this.OutputNode = new OutputNode(ref abNode);
        }

        public override AbstractNet copy()
        {
            FeedForwardNet tempNet = this;
            return new FeedForwardNet(ref tempNet);
        }
    }
}
