using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNetwork.DataStructures
{
    /// <summary>
    /// From https://msdn.microsoft.com/en-us/library/ms379572(v=vs.80).aspx
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Node<T>
    {
        public Node() { }
        public Node(T data) : this(data, null) { }
        public Node(T data, NodeList<T> neighbors)
        {
            this.Value = data;
            this.Neighbors = neighbors;
        }

        public T Value { get; set; }

        protected NodeList<T> Neighbors { get; set; } = null;
    }
}

