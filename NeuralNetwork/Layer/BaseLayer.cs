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
    public class BaseLayer :  ILayer
    {
        public BaseLayer()
        {
            if (Nodes == null)
                Nodes = new NeuralNodeList<INeuralComponent>();
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
            foreach (INeuralComponent node in Nodes)
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
        public void AddNode(INeuralComponent node)
        {
            Nodes.Add(node);
        }

        public void ConnectNodes(INeuralComponent from, INeuralComponent to, uint priority)
        {
            INode fromNode = from as INode,
                toNode = to as INode;

            if (from is ILayer fromLayer)
                fromNode = fromLayer.Output;
            if (fromNode == null)
                throw new ArgumentException("Unknown Type", nameof(from));
            if (to is ILayer toLayer)
                toNode = toLayer.Input;
            if (toNode == null)
                throw new ArgumentException("Unknown Type", nameof(to));

            if (to.MaxInputs - 1 < InputNeighbors.Count)
                throw new ArgumentException($"No more than {to.MaxInputs} allowed!", to.GetType().ToString());
            if (from.MaxOutputs - 1 < OutputNeighbors.Count)
                throw new ArgumentException($"No more than {from.MaxOutputs} allowed!", to.GetType().ToString());
            to.InputNeighbors.Add(fromNode);
            from.OutputNeighbors.Add(toNode);
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
        /// <param name="component"></param>
        /// <returns>true if successful</returns>
        public bool Remove(INeuralComponent component)
        {
            INeuralComponent componentToRemove = Nodes.Where(x => x.Equals(component)).SingleOrDefault();
            if (componentToRemove == null)
                // node wasn't found
                return false;

            if (componentToRemove is ILayer layer)
            {
                if (layer.Input != null && layer.Input.Equals(Input))
                    Input = null;
                if (layer.Output != null && layer.Output.Equals(Output))
                    Output = null;
                RemoveNodeFromAllEdges(layer.Input);
                RemoveNodeFromAllEdges(layer.Output);
            }
            else if(componentToRemove is INode node)
            {
                if (componentToRemove.Equals(Input))
                    Input = null;
                if (componentToRemove.Equals(Output))
                    Output = null;
                RemoveNodeFromAllEdges(node);
            }
            // otherwise, the node was found
            Nodes.Remove(componentToRemove);
            
            return true;
        }

        private void RemoveNodeFromAllEdges(INode nodeToRemove)
        {
            // enumerate through each node in the nodeSet, removing edges to this node
            foreach (INode gnode in Nodes)
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

        public IEnumerator<INeuralComponent> GetEnumerator()
        {
            return Nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Nodes.GetEnumerator();
        }

        public void Calculate(object sender)
        {
            Stack<Thread> allThreads = new Stack<Thread>();
            Thread thread = new Thread(() => { Input.Calculate(this); });
            thread.Start();
            allThreads.Push(thread);
            CalculateHelper(ref allThreads);
        }

        private void CalculateHelper(ref Stack<Thread> allThreads)
        {
            foreach (INeuralComponent node in Nodes)
            {
                if (node is IRecurrent || node is Weight)
                {
                    Thread thread = new Thread(() => { node.Calculate(this); });
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
                if (node is IRecurrent)
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
            foreach (INeuralComponent node in Nodes)
                node.Reset();
        }

        public NeuralNodeList<INeuralComponent> Nodes { get; }

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

        public NeuralNodeList<INode> InputNeighbors => Input.InputNeighbors;

        public NeuralNodeList<INode> OutputNeighbors => Output.OutputNeighbors;

        public List<uint> InputPriorities => Input.InputPriorities;

        public List<Array> InputSensitivities => Input.InputSensitivities;

        public int MaxInputs => Input.MaxInputs;

        public int MaxOutputs => Output.MaxOutputs;

        public int MinInputs => Input.MinInputs;

        public int MinOutputs => Output.MinOutputs;
    }
}
