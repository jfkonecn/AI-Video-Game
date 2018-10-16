using NeuralNetwork.Layer.NeuralNode;
using NeuralNetwork.NeuralMath;
using NeuralNetwork.NeuralMath.Stats;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NeuralNetwork.Layer
{
    /// <summary>
    /// Stores a graph of neural nodes
    /// base on
    /// From https://msdn.microsoft.com/en-us/library/ms379574(v=vs.80).aspx#datastructures20_5_topic3
    /// </summary>
    public class Layer : BaseNode, IEnumerable<BaseNode>
    {
        public Layer()
        {

        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public Layer(Layer old)
        {
            if (old == null)
                throw new ArgumentNullException(nameof(old));
            foreach (BaseNode oldNode in Nodes)
            {
                BaseNode newNode = (BaseNode)oldNode.GetType().
                    GetConstructor(new Type[] { oldNode.GetType() }).Invoke(new object[] { oldNode });
                if (oldNode.Id.Equals(old.Input.Id))
                    Input = (Vector)newNode;
                if (oldNode.Id.Equals(old.Output.Id))
                    Output = (Vector)newNode;
                AddNode(newNode); 
            }
            if (Nodes.Count != old.Nodes.Count)
                throw new Exception("something went wrong");
            for (int i = 0; i < Nodes.Count; i++)
            {
                BaseNode newNode = Nodes[i],
                    oldNode = old.Nodes[i];
                foreach (BaseNode oldInputNode in oldNode.InputNeighbors)
                {
                    int idx = old.Nodes.IndexOf(oldInputNode);
                    ConnectNodes(Nodes[idx], newNode, oldNode.InputPriorities[idx]);
                }
            }
        }


        /// <summary>
        /// Randomly Changes the weights and biases in this layer
        /// </summary>
        /// <param name="lowerValueLimit">The lower limit for the weights</param>
        /// <param name="upperValueLimit">The upper limit for the weights</param>
        /// <param name="stdDev">The standard deviation for changes to the weights and biases</param>
        /// <exception cref="ArgumentException"></exception>
        public void Mutate(double lowerValueLimit, double upperValueLimit, double stdDev)
        {
            if (upperValueLimit <= lowerValueLimit)
                throw new ArgumentException("Upper Limit must be higher than Lower Limit");
            double mean = (upperValueLimit - lowerValueLimit) / 2.0;
            foreach (BaseNode node in Nodes)
            {
                if(node is Weight wtNode)
                {
                    Matrix.PerformActionOnEachArrayElement(node.OutputArray, (indices) =>
                    {
                        double num = ZScore.RandomGaussianDistribution(mean, stdDev);
                        if (num > upperValueLimit)
                            num = upperValueLimit;
                        else if (num < lowerValueLimit)
                            num = lowerValueLimit;
                        node.OutputArray.SetValue(num, indices);
                    });
                }
            }
        }
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
            if (nodeToRemove.Equals(Input))
                Input = null;
            if (nodeToRemove.Equals(Output))
                Output = null;

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

        public override void Calculate()
        {
            Stack<Thread> allThreads = new Stack<Thread>();
            Thread thread = new Thread(() => { Input.Calculate(); });
            thread.Start();
            allThreads.Push(thread);
            foreach (RecurrentVector node in Nodes)
            {
                if (node is RecurrentVector)
                {
                    thread = new Thread(() => { node.Calculate(); });
                    thread.Start();
                    allThreads.Push(thread);
                }
            }
            while (allThreads.Count > 0) { allThreads.Pop().Join(); }
        }

        public override void Train(double learningRate, Array sensitivity)
        {
            Output.Train(learningRate, sensitivity);
        }

        public override void Reset()
        {
            foreach (BaseNode node in Nodes)
                node.Reset();
        }

        protected override void InternalCalculate()
        {
            Calculate();
        }

        protected override void InternalTrain(double learningRate, Array sensitivity)
        {
            Train(learningRate, sensitivity);
        }

        protected override void DetermineInputNodeSensitivity()
        {
            // This method call Input.DetermineInputNodeSensitivity, but it will already be called after InternalTrain is called
            return;
        }

        public NeuralNodeList Nodes { get; }

        public int Count
        {
            get { return Nodes.Count; }
        }
        private Vector _Input = null;
        /// <summary>
        /// 
        /// </summary>
        public Vector Input
        {
            get
            {
                return _Input;
            }
            set
            {
                if (!Nodes.Contains(value) && value != null)
                    throw new ArgumentException("Input must be a node in this layer");
                _Input = value;
            }
        }
        private Vector _Output = null;
        /// <summary>
        /// 
        /// </summary>
        public Vector Output
        {
            get
            {
                return _Output;
            }
            set
            {
                if (!Nodes.Contains(value) && value != null)
                    throw new ArgumentException("Output must be a node in this layer");
                _Output = value;
            }
        }        
    }
}
