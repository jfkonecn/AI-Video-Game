using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNetwork.DataStructures
{
    /// <summary>
    /// From https://msdn.microsoft.com/en-us/library/ms379574(v=vs.80).aspx#datastructures20_5_topic3
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GraphNode<T> : Node<T>
    {
        private List<int> costs;

        public GraphNode() : base() { }
        public GraphNode(T value) : base(value) { }
        public GraphNode(T value, NodeList<T> neighbors) : base(value, neighbors) { }

        new public NodeList<T> Neighbors
        {
            get
            {
                if (base.Neighbors == null)
                    base.Neighbors = new NodeList<T>();

                return base.Neighbors;
            }
        }

        public List<int> Costs
        {
            get
            {
                if (costs == null)
                    costs = new List<int>();

                return costs;
            }
        }
    }
}
