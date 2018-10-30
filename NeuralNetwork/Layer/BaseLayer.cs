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
    public class BaseLayer : INeuralNode, IEnumerable<INeuralNode>
    {
        protected BaseLayer()
        {
            if (Nodes == null)
                Nodes = new NeuralNodeList();
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public BaseLayer(BaseLayer old) : this()
        {
            if (old == null)
                throw new ArgumentNullException(nameof(old));
            foreach (INeuralNode oldNode in old.Nodes)
            {
                INeuralNode newNode = (INeuralNode)oldNode.GetType().
                    GetConstructor(new Type[] { oldNode.GetType() }).Invoke(new object[] { oldNode });
                AddNode(newNode);
                BaseLayer inOutCheckLayer = oldNode is BaseLayer ? (BaseLayer)oldNode : old;

                INeuralNode inputCheck = oldNode is BaseLayer ? ((BaseLayer)oldNode).Input : oldNode,
                    outputCheck = oldNode is BaseLayer ? ((BaseLayer)oldNode).Output : oldNode;
                if (old.Input.Equals(inputCheck))
                {
                    Input = newNode is Vector ? (Vector)newNode : ((BaseLayer)newNode).Input;
                }
                if (old.Output.Equals(outputCheck))
                {
                    Output = newNode is Vector ? (Vector)newNode : ((BaseLayer)newNode).Output;
                }                
            }
            if (Nodes.Count != old.Nodes.Count || Input == null || Output == null)
                throw new Exception("something went wrong");

            for (int i = 0; i < Nodes.Count; i++)
            {
                INeuralNode newNode = Nodes[i],
                    oldNode = old.Nodes[i];
                for (int j = 0; j < oldNode.InputNeighbors.Count; j++)
                {
                    int idx = old.Nodes.IndexOf(oldNode.InputNeighbors[j]);
                    ConnectNodes(Nodes[idx], newNode, oldNode.InputPriorities[j]);
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
            foreach (INeuralNode node in Nodes)
            {
                if(node is BaseLayer layer)
                {
                    layer.Mutate(lowerValueLimit, upperValueLimit, stdDev);
                }
                else if (node is Weight wtNode)
                {
                    Matrix.PerformActionOnEachArrayElement(wtNode.OutputArray, (indices) =>
                    {
                        double num = ZScore.RandomGaussianDistribution(mean, stdDev);
                        if (num > upperValueLimit)
                            num = upperValueLimit;
                        else if (num < lowerValueLimit)
                            num = lowerValueLimit;
                        wtNode.OutputArray.SetValue(num, indices);
                    });
                }
            }
        }
        /// <summary>
        /// Adds node to graph
        /// </summary>
        /// <param name="node"></param>
        public void AddNode(INeuralNode node)
        {
            Nodes.Add(node);
        }

        public void ConnectNodes(INeuralNode from, INeuralNode to, uint priority)
        {
            if (from is BaseLayer fromLayer)
                from = fromLayer.Output;
            if (to is BaseLayer toLayer)
                to = toLayer.Input;
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
        public bool Remove(INeuralNode node)
        {
            INeuralNode nodeToRemove = Nodes.Where(x => x.Equals(node)).SingleOrDefault();
            if (nodeToRemove == null)
                // node wasn't found
                return false;

            if (nodeToRemove is BaseLayer layer)
            {
                if (layer.Input != null && layer.Input.Equals(Input))
                    Input = null;
                if (layer.Output != null && layer.Output.Equals(Output))
                    Output = null;
                RemoveNodeFromAllEdges(layer.Input);
                RemoveNodeFromAllEdges(layer.Output);
            }
            else
            {
                if (nodeToRemove.Equals(Input))
                    Input = null;
                if (nodeToRemove.Equals(Output))
                    Output = null;
                RemoveNodeFromAllEdges(nodeToRemove);
            }
            // otherwise, the node was found
            Nodes.Remove(nodeToRemove);
            
            return true;
        }

        private void RemoveNodeFromAllEdges(INeuralNode nodeToRemove)
        {
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
        }

        public IEnumerator<INeuralNode> GetEnumerator()
        {
            return Nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Nodes.GetEnumerator();
        }

        public void Calculate()
        {
            Stack<Thread> allThreads = new Stack<Thread>();
            Thread thread = new Thread(() => { Input.Calculate(); });
            thread.Start();
            allThreads.Push(thread);
            CalculateHelper(ref allThreads);
        }

        private void CalculateHelper(ref Stack<Thread> allThreads)
        {
            foreach (INeuralNode node in Nodes)
            {
                if (node is RecurrentVector || node is Weight)
                {
                    Thread thread = new Thread(() => { node.Calculate(); });
                    thread.Start();
                    allThreads.Push(thread);
                }
                if (node is BaseLayer baseLayer)
                {
                    baseLayer.CalculateHelper(ref allThreads);
                }
            }
            while (allThreads.Count > 0) { allThreads.Pop().Join(); }
        }

        public void UpdateSensitivities(Array sensitivity, TrainingMode trainingMode)
        {
            Output.UpdateSensitivities(sensitivity, trainingMode);
        }

        public void Learn(double learningRate)
        {
            Output.Learn(learningRate);
        }

        /// <summary>
        /// Performs an action on the output and all recurrent vectors in different threads
        /// </summary>
        /// <param name="action">Passes an the output node or a recurrent vector</param>
        private void TrainHelper(Action<BaseNode> action)
        {
            Stack<Thread> allThreads = new Stack<Thread>();
            Thread thread = new Thread(() => { action(Output); });
            thread.Start();
            allThreads.Push(thread);
            foreach (BaseNode node in Nodes)
            {
                if (node is RecurrentVector)
                {
                    thread = new Thread(() => { action(node); });
                    thread.Start();
                    allThreads.Push(thread);
                }
            }
            while (allThreads.Count > 0) { allThreads.Pop().Join(); }
        }

        public void Reset()
        {
            foreach (INeuralNode node in Nodes)
                node.Reset();
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
                    throw new ArgumentException("Input must be contained in this layer");
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

        public Guid Id => Guid.NewGuid();

        public Array OutputArray => Output.OutputArray;

        public NeuralNodeList InputNeighbors => Input.InputNeighbors;

        public NeuralNodeList OutputNeighbors => Output.OutputNeighbors;

        public List<uint> InputPriorities => Input.InputPriorities;

        public List<Array> InputSensitivities => Input.InputSensitivities;
    }
}
