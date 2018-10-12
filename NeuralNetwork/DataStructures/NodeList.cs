using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NeuralNetwork.DataStructures
{
    /// <summary>
    /// From https://msdn.microsoft.com/en-us/library/ms379572(v=vs.80).aspx
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NodeList<T> : Collection<Node<T>>
    {
        public NodeList() : base() { }

        public NodeList(int initialSize)
        {
            // Add the specified number of items
            for (int i = 0; i < initialSize; i++)
                base.Items.Add(default(Node<T>));
        }

        public Node<T> FindByValue(T value)
        {
            // search the list for the value
            foreach (Node<T> node in Items)
                if (node.Value.Equals(value))
                    return node;

            // if we reached here, we didn't find a matching node
            return null;
        }
    }
}
