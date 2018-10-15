using NeuralNetwork.Layer.NeuralNode;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuralNetwork.Layer
{
    /// <summary>
    /// Stores a graph of neural nodes
    /// base on
    /// From https://msdn.microsoft.com/en-us/library/ms379574(v=vs.80).aspx#datastructures20_5_topic3
    /// </summary>
    public abstract class Layer : IEnumerable<BaseNode>
    {
        /// <summary>
        /// Total number of input layers into this layer
        /// </summary>
        protected abstract uint TotalInputs { get; }
        /// <summary>
        /// Total number of outputs layers from this layer
        /// </summary>
        protected abstract uint TotalOutputs { get; }
        /// <summary>
        /// Create exact copy of the passed layer
        /// </summary>
        /// <param name="oldLayer">Layer to be copied</param>
        /// <returns>New Copy</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public abstract Layer CopyLayer(Layer oldLayer);
        /// <summary>
        /// Randomly Changes the weights and biases in this layer
        /// </summary>
        /// <param name="lowerValueLimit">The lower limit for the weights</param>
        /// <param name="upperValueLimit">The upper limit for the weights</param>
        /// <param name="stdDev">The standard deviation for changes to the weights and biases</param>
        public abstract void Mutate(double lowerValueLimit, double upperValueLimit, double stdDev);
        /// <summary>
        /// Adds node to graph
        /// </summary>
        /// <param name="node"></param>
        public void AddNode(BaseNode node)
        {            
            Nodes.Add(node);
        }

        public void ConnectNodes(BaseNode from, BaseNode to, uint priority)
        {
            to.InputNeighbors.Add(from);
            from.OutputNeighbors.Add(to);
            to.InputPriorities.Add(priority);
            to.InputSensitivities.Add(null);
        }

        public bool Contains(BaseNode node)
        {
            return Nodes.Contains(node);
        }

        /// <summary>
        /// Finds the first match of this node and removes it
        /// </summary>
        /// <param name="node"></param>
        /// <returns>true if successful</returns>
        public bool Remove(BaseNode node)
        {
            BaseNode nodeToRemove = Nodes.Where(x => x.Equals(node)).SingleOrDefault();
            if (nodeToRemove == null)
                // node wasn't found
                return false;

            // otherwise, the node was found
            Nodes.Remove(nodeToRemove);

            // enumerate through each node in the nodeSet, removing edges to this node
            foreach (BaseNode gnode in Nodes)
            {
                int index = gnode.InputNeighbors.IndexOf(nodeToRemove);
                gnode.OutputNeighbors.Remove(nodeToRemove);
                if (index != -1)
                {
                    // remove the reference to the node and associated cost
                    gnode.InputNeighbors.RemoveAt(index);
                    gnode.InputPriorities.RemoveAt(index);
                    gnode.InputSensitivities.RemoveAt(index);
                }
            }

            return true;
        }

        public IEnumerator<BaseNode> GetEnumerator()
        {
            return Nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Nodes.GetEnumerator();
        }

        public NeuralNodeList Nodes { get; }

        public int Count
        {
            get { return Nodes.Count; }
        }

    }
}
