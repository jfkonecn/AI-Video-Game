using NeuralNetwork.NeuralMath;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NeuralNetwork.Layer.NeuralNode
{
    public abstract class BaseNode : INode
    {
        /// <summary>
        /// Vector
        /// </summary>
        protected BaseNode()
        {

        }


        public Guid Id { get; protected set; } = Guid.NewGuid();
        /// <summary>
        /// Stores all input nodes
        /// </summary>
        public NeuralNodeList<INode> InputNeighbors { get; protected set; } = new NeuralNodeList<INode>();
        /// <summary>
        /// Stores all output nodes
        /// </summary>
        public NeuralNodeList<INode> OutputNeighbors { get; protected set; } = new NeuralNodeList<INode>();
        /// <summary>
        /// Use to determine the order in which arrays are evaluated
        /// </summary>
        public List<uint> InputPriorities { get; } = new List<uint>();
        /// <summary>
        /// Stores the sensitivities foreach node, Note: arrays begin null
        /// </summary>
        public List<Array> InputSensitivities { get; } = new List<Array>();

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
        protected int InputCounter { get; set; } = 0;

        /// <summary>
        /// Counts the number of outputs which enter the node
        /// </summary>
        protected int OutputCounter { get; set; } = 0;
        /// <summary>
        /// Stores all of the derivative of the error with respect to this node(Sensitivity) for each output path
        /// </summary>
        protected Stack<Array> OutputSensitivities { get; } = new Stack<Array>();

        public abstract int MaxInputs { get; }
        public abstract int MinInputs { get; }

        public abstract int MaxOutputs { get; }

        public abstract int MinOutputs { get; }

        private readonly object InputCountLock = new object();
        private readonly object OutputSensitivitiesLock = new object();
        private readonly object OutputCounterLock = new object();
        /// <summary>
        /// Set all in ONLY this node to initial conditions
        /// </summary>
        public virtual void Reset()
        {
            InputCounter = 0;
            OutputCounter = 0;
            while (OutputSensitivities.Count > 0) { OutputSensitivities.Pop(); }
            ResetValues();
        }
        /// <summary>
        /// Resets this node's value and sensitivities to initial conditions
        /// </summary>
        protected virtual void ResetValues()
        {
            if (OutputArray == null)
                return;
            Matrix.SetAll(OutputArray, 0);
            if (Sensitivity != null)
                Matrix.SetAll(Sensitivity, 0);
            foreach (Array arr in InputSensitivities)
            {
                if (arr != null)
                    Matrix.SetAll(arr, 0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Calculate()
        {
            InputOutputNeighborsCheck();
            if (this is RecurrentVector)
                return;            
            OutgoingThreadHelper(OutputNeighbors.Count, 
                (idx) => 
                {
                    INeuralComponent node = OutputNeighbors[idx];
                    if (node is RecurrentVector) return;
                    if (node is BaseNode baseNode)
                        baseNode.CalculateHelper();
                    else if (node is BaseLayer baseLayer)
                        baseLayer.Input.CalculateHelper();
                    else
                        throw new Exception("Unsupported Node Type");
                });
        }

        private void CalculateHelper()
        {            
            // don't continue if we're not allowed to
            if (!IncomingThreadHelper(() => InputCountLock, () => InputNeighbors.Count,
                () => InputCounter, (obj) => InputCounter++))
                return;
            InputCounter = 0;
            InternalCalculate();
            Calculate();
        }


        /// <summary>
        /// Called after all input threads arrive. Sets the OutputArray.
        /// </summary>
        protected abstract void InternalCalculate();


        /// <summary>
        /// 
        /// </summary>
        /// <param name = "learningRate" > A fraction which determines how big of a step the change in the weights and biases will be along the gradient
        /// <para>Note: Must be greater than or equal to -1, but less than or equal to 1</para> 
        /// <para>Negative learning rate means the network is being "punished"</para></param>
        /// <param name="sensitivity">The derivative of the error with respect to this node</param>
        /// <param name="trainingMode"></param>
        /// <exception cref="ArgumentException"></exception>
        public void UpdateSensitivities(Array sensitivity, TrainingMode trainingMode)
        {
            if (sensitivity == null)
                throw new ArgumentNullException($"{nameof(sensitivity)}");
            InputOutputNeighborsCheck();
            Array avgSen = Matrix.CreateArrayWithMatchingDimensions(OutputArray);
            if(OutputNeighbors.Count == 0)
            {
                Matrix.SetArraysEqualToEachOther(sensitivity, avgSen);
            }
            else
            {
                if (!IncomingThreadHelper(() => OutputSensitivitiesLock,
                    () => OutputNeighbors.Count,
                    () => OutputSensitivities.Count,                        
                        (obj) => OutputSensitivities.Push((Array)obj[0]),
                        sensitivity))
                    return;
                while (OutputSensitivities.Count != 0)
                {
                    Array temp = OutputSensitivities.Pop();
                    // could be null if the path does not go to the output
                    if (temp != null)
                        Matrix.Add(avgSen, temp, avgSen);
                }
                Matrix.ScalarMultiplication(1.0 / OutputNeighbors.Count, avgSen);
            }


            InternalUpdateSensitivities(avgSen, trainingMode);
            DetermineInputNodeSensitivity(avgSen);
            if (this is RecurrentVector)
                return;
            OutgoingThreadHelper(InputNeighbors.Count, 
                (idx) => InputNeighbors[idx].UpdateSensitivities(InputSensitivities[idx], trainingMode));
        }

        


        /// <summary>
        /// Uses exisiting sensitivities to train and update weights
        /// </summary>
        /// <param name = "learningRate" > A fraction which determines how big of a step the change in the weights and biases will be along the gradient
        /// <para>Note: Must be greater than or equal to -1, but less than or equal to 1</para> 
        /// <para>Negative learning rate means the network is being "punished"</para></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public virtual void Learn(double learningRate)
        {
            InputOutputNeighborsCheck();
            if (learningRate < -1 || learningRate > 1)
                throw new ArgumentOutOfRangeException(nameof(learningRate));
            if (!IncomingThreadHelper(() => OutputCounterLock,
                () => OutputNeighbors.Count,
                () => OutputCounter,
                (obj) => OutputCounter++))
                return;
            InternalLearn(learningRate);
            if (this is RecurrentVector)
                return;
            OutgoingThreadHelper(InputNeighbors.Count,
                (idx) => InputNeighbors[idx].Learn(learningRate));
        }

        /// <summary>
        /// Handles Incoming Threads
        /// </summary>
        /// <param name="getArgHandlerLock">gets a lock which should protect argsHandler from race conditions</param>
        /// <param name="totalExpectedThread">gets the total number of threads expected</param>
        /// <param name="totalThreadsPassed">gets the total number of threads passed</param>
        /// <param name="argsHandler">Passes args here under the protection of lock</param>
        /// <param name="args">arguments to be passed to argsHandler</param>
        /// <returns>true if this thread should continue</returns>
        protected bool IncomingThreadHelper(Func<object> getLock, Func<int> totalExpectedThread, 
            Func<int> totalThreadsPassed, Action<object[]> argsHandler, params object[] args)
        {
            lock (getLock())
            {
                int expectedThreads = totalExpectedThread();
                if (expectedThreads != 0)
                    argsHandler(args);
                int threadsPassed = totalThreadsPassed();
                if (expectedThreads < threadsPassed)
                    throw new ArgumentException("Total expected thread count exceeded! Did you forget to call reset?", 
                        nameof(totalExpectedThread));
                if (expectedThreads != threadsPassed)
                    return false;
            }
            return true;
        }


        /// <summary>
        /// Handles outgoing threads
        /// </summary>
        /// <param name="totalOutgoingThreads"></param>
        /// <param name="threadMethod">Given an index to represent the thread (from 0 to totalOutgoingThreads - 1) performs an action</param>
        /// <returns></returns>
        protected void OutgoingThreadHelper(int totalOutgoingThreads, Action<int> threadMethod)
        {
            Stack<Thread> allThreads = new Stack<Thread>();
            for (int i = 0; i < totalOutgoingThreads; i++)
            {
                // create local variable to avoid sharing
                int temp = i;
                Thread thread = new Thread(() => 
                {
                    threadMethod(temp);
                });
                thread.Start();
                allThreads.Push(thread);
            }
            while (allThreads.Count > 0) { allThreads.Pop().Join(); }
        }
        /// <summary>
        /// Called after all output threads arrive. Responsible for setting this node's Sensitivity array and input sensitivity arrays.
        /// </summary>
        /// <param name="sensitivity">The derivative of the error with respect to this node</param>
        /// <param name="trainingMode"></param>
        protected virtual void InternalUpdateSensitivities(Array sensitivity, TrainingMode trainingMode)
        {
            Sensitivity = sensitivity;
            if (Sensitivity == null || !Matrix.CheckIfArraysAreSameSize(false, Sensitivity, sensitivity))
                Sensitivity = Matrix.CreateArrayWithMatchingDimensions(sensitivity);
            switch (trainingMode)
            {
                case TrainingMode.Incremental:
                    Matrix.SetArraysEqualToEachOther(sensitivity, Sensitivity);
                    break;
                case TrainingMode.Batch:
                default:
                    Matrix.Add(sensitivity, Sensitivity, Sensitivity);                    
                    break;
            }            
        }

        /// <summary>
        /// Called after all output threads arrive. Responsible for setting this node's weights, if any.
        /// </summary>
        /// <param name="learningRate"></param>
        /// <param name="sensitivity">The derivative of the error with respect to this node
        /// <para>WARNING: This can be null and if this is null we cannot throw an error</para>
        /// </param>
        /// <param name="trainingMode"></param>
        /// <param name="updateWeights">updateWeights</param>
        protected virtual void InternalLearn(double learningRate)
        {
            // usually there are no weights so we don't need to do anything
        }

        /// <summary>
        /// Sets the <see cref="InputSensitivities"/> arrays. Called after the this node's sensitivities are set
        /// </summary>
        protected abstract void DetermineInputNodeSensitivity(Array sensitivity);
        /// <summary>
        /// Throws exception if an improper amount of inputs and outputs are used
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        protected void InputOutputNeighborsCheck()
        {
            if (InputNeighbors.Count > MaxInputs)
                throw new ArgumentException($"Must have at most {MaxOutputs} outputs!", GetType().ToString());
            if (InputNeighbors.Count < MinInputs)
                throw new ArgumentException($"Must have at least {MinInputs} outputs!", GetType().ToString());
            if (OutputNeighbors.Count > MaxOutputs)
                throw new ArgumentException($"Must have at most {MaxOutputs} outputs!", GetType().ToString());
            if (OutputNeighbors.Count < MinOutputs)
                throw new ArgumentException($"Must have at least {MinOutputs} outputs!", GetType().ToString());
        }


    }

}
