﻿using NeuralNetwork.DataStructures;
using NeuralNetwork.NeuralMath;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NeuralNetwork.Layer.NeuralNode
{
    internal abstract class NeuralNode : INeuralComponent
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
        protected NeuralNode(Array value)
        {
            
        }
        public Guid Id { get; protected set; } = Guid.NewGuid();

        public NeuralNodeList InputNeighbors { get; set; } = new NeuralNodeList();
        public NeuralNodeList OutputNeighbors { get; set; } = new NeuralNodeList();
        /// <summary>
        /// Use to determine the order in which arrays are evaluated
        /// </summary>
        public List<uint> InputPriorities { get; } = new List<uint>();

        /// <summary>
        /// The array which is used for calculations for all outputs
        /// </summary>
        public Array OutputArray { get; set; }
        /// <summary>
        /// <param name="sensitivity">The derivative of the error with respect to this node</param>
        /// </summary>
        public Array Sensitivity { get; protected set; } = null;


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
            Stack<Thread> allThreads = new Stack<Thread>();
            foreach (NeuralNode node in OutputNeighbors)
            {
                Thread thread = new Thread(() => { node.Calculate(this); });
                thread.Start();
                allThreads.Push(thread);
            }
            while (allThreads.Count > 0) { allThreads.Pop().Join(); }
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
        /// <param name = "learningRate" > A fraction which determines how big of a step the change in the weights and biases will be along the gradient
        /// <para>Note: Must be greater than or equal to -1, but less than or equal to 1</para> 
        /// <para>Negative learning rate means the network is being "punished"</para></param>
        /// <param name="sensitivity">The derivative of the error with respect to this node</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void Train(double learningRate, Array sensitivity)
        {
            if (learningRate < -1 || learningRate > 1)
                throw new ArgumentOutOfRangeException(nameof(learningRate));
            lock (OutputSensitivitiesLock)
            {
                if(OutputSensitivities.Count != 0)
                    OutputSensitivities.Push(sensitivity);
                if (OutputNeighbors.Count < OutputSensitivities.Count)
                    throw new ArgumentException("Total Output exceeded! Did you forget to call reset?", nameof(InputCounter));
                if (OutputNeighbors.Count != OutputSensitivities.Count)
                    return;
            }
            Array avgSen = OutputSensitivities.Pop();
            while (OutputSensitivities.Count != 0)
            {
                Matrix.Add(avgSen, OutputSensitivities.Pop(), avgSen);
            }
            Matrix.ScalarMultiplication(1.0 / OutputNeighbors.Count, avgSen);
            InternalTrain(learningRate, avgSen);
            Stack<Thread> allThreads = new Stack<Thread>();
            foreach (NeuralNode node in InputNeighbors)
            {
                Thread thread = new Thread(() => { node.Calculate(this); });
                thread.Start();
                allThreads.Push(thread);
            }
            while(allThreads.Count > 0) { allThreads.Pop().Join(); }
        }
        /// <summary>
        /// Called after all output threads arrive
        /// </summary>
        /// <param name="sensitivity">The derivative of the error with respect to this node</param>
        public abstract void InternalTrain(double learningRate, Array sensitivity);

    }
}
