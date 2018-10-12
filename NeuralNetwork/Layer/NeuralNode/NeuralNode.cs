using NeuralNetwork.DataStructures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NeuralNetwork.Layer.NeuralNode
{
    internal abstract class NeuralNode : GraphNode<Array>, INeuralComponent
    {
        /// <summary>
        /// Vector
        /// </summary>
        protected NeuralNode() : base()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        protected NeuralNode(Array value) : base(value)
        {
            
        }
        public Guid Id { get; protected set; } = Guid.NewGuid();

        protected NodeList<Array> InputNeighbors { get; set; } = new NodeList<Array>();

        /// <summary>
        /// Counts the number of inputs which enter the node
        /// </summary>
        protected uint InputCounter { get; set; } = 0;
        /// <summary>
        /// Stores all of the derivative of the error with respect to this node(Sensitivity) for each output path
        /// </summary>
        protected Stack<Array> OutputSensitivities { get; } = new Stack<Array>();
        private readonly object InputCountLock = new object();
        private readonly object OutputSensitivitiesLock = new object();
        /// <summary>
        /// Set all in ONLY this node to initial conditions
        /// </summary>
        public virtual void Reset()
        {
            InputCounter = 0;
            while (OutputSensitivities.Count > 0) { OutputSensitivities.Pop(); }
            ResetValue();
        }
        /// <summary>
        /// Resets this node's value to initial conditions
        /// </summary>
        protected abstract void ResetValue();

        /// <summary>
        /// 
        /// </summary>
        public void Calculate()
        {
            foreach (NeuralNode node in Neighbors)
            {
                Thread thread = new Thread(() => { node.Calculate(this); });
                thread.Start();
            }
        }

        public void Calculate(NeuralNode sender)
        {
            lock (InputCountLock)
            {
                if(InputNeighbors.Count != 0)
                    InputCounter++;                
                if (InputNeighbors.Count < InputCounter)
                    throw new ArgumentException("Total Inputs exceeded! Did you forget to call reset?", nameof(InputCounter));
                if (InputNeighbors.Count != InputCounter)
                    return;
            }
            InternalCalculate();
            Calculate();
        }
        /// <summary>
        /// Called after all input threads arrive
        /// </summary>
        protected abstract void InternalCalculate();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sensitivity">The derivative of the error with respect to this node</param>
        public void Train(Array sensitivity)
        {
            lock (OutputSensitivitiesLock)
            {
                if(OutputSensitivities.Count != 0)
                    OutputSensitivities.Push(sensitivity);
                if (Neighbors.Count < OutputSensitivities.Count)
                    throw new ArgumentException("Total Output exceeded! Did you forget to call reset?", nameof(InputCounter));
                if (Neighbors.Count != OutputSensitivities.Count)
                    return;
            }
            Array avgSen = OutputSensitivities.Pop();
            while (OutputSensitivities.Count != 0)
            {
                //NeuralMath.Matrix.Add()
            }
            //InternalTrain();
            foreach (NeuralNode node in InputNeighbors)
            {
                Thread thread = new Thread(() => { node.Calculate(this); });
                thread.Start();
            }
        }
        /// <summary>
        /// Called after all output threads arrive
        /// </summary>
        /// <param name="sensitivity">The</param>
        public abstract void InternalTrain(Array sensitivity);

        public void Train()
        {
            throw new NotImplementedException();
        }
    }
}
