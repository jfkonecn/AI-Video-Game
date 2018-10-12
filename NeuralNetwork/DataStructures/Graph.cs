using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NeuralNetwork.DataStructures
{
    /// <summary>
    /// From https://msdn.microsoft.com/en-us/library/ms379574(v=vs.80).aspx#datastructures20_5_topic3
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Graph<T> : IEnumerable<T>
    {
        public Graph() : this(null) { }
        public Graph(NodeList<T> nodeSet)
        {
            if (nodeSet == null)
                this.Nodes = new NodeList<T>();
            else
                this.Nodes = nodeSet;
        }

        public void AddNode(GraphNode<T> node)
        {
            // adds a node to the graph
            Nodes.Add(node);
        }

        public void AddNode(T value)
        {
            // adds a node to the graph
            Nodes.Add(new GraphNode<T>(value));
        }

        public void AddDirectedEdge(GraphNode<T> from, GraphNode<T> to, int cost)
        {
            from.Neighbors.Add(to);
            from.Costs.Add(cost);
        }

        public void AddUndirectedEdge(GraphNode<T> from, GraphNode<T> to, int cost)
        {
            from.Neighbors.Add(to);
            from.Costs.Add(cost);

            to.Neighbors.Add(from);
            to.Costs.Add(cost);
        }

        public bool Contains(T value)
        {
            return Nodes.FindByValue(value) != null;
        }

        public bool Remove(T value)
        {
            // first remove the node from the nodeset
            GraphNode<T> nodeToRemove = (GraphNode<T>)Nodes.FindByValue(value);
            if (nodeToRemove == null)
                // node wasn't found
                return false;

            // otherwise, the node was found
            Nodes.Remove(nodeToRemove);

            // enumerate through each node in the nodeSet, removing edges to this node
            foreach (GraphNode<T> gnode in Nodes)
            {
                int index = gnode.Neighbors.IndexOf(nodeToRemove);
                if (index != -1)
                {
                    // remove the reference to the node and associated cost
                    gnode.Neighbors.RemoveAt(index);
                    gnode.Costs.RemoveAt(index);
                }
            }

            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>)Nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Nodes.GetEnumerator();
        }

        public NodeList<T> Nodes { get; }

        public int Count
        {
            get { return Nodes.Count; }
        }
    }
}
