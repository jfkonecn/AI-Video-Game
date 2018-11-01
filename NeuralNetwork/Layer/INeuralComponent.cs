using NeuralNetwork.Layer.NeuralNode;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNetwork.Layer
{
    public interface INeuralComponent
    {
        void Calculate();
        void UpdateSensitivities(Array sensitivity, TrainingMode trainingMode);
        /// <summary>
        /// Update weights based on sensitivities. Call UpdateSensitivities first!
        /// </summary>
        void Learn(double learningRate);
        /// <summary>
        /// Set all entries to initial conditions
        /// </summary>
        void Reset();
        Guid Id { get; }
        

        /// <summary>
        /// Stores all input nodes
        /// </summary>
        NeuralNodeList<INode> InputNeighbors { get; }
        /// <summary>
        /// Max inputs allowed
        /// </summary>
        int MaxInputs { get; }
        /// <summary>
        /// Min inputs allowed
        /// </summary>
        int MinInputs { get; }
        /// <summary>
        /// Stores all output nodes
        /// </summary>
        NeuralNodeList<INode> OutputNeighbors { get; }
        /// <summary>
        /// Max Outputs allowed
        /// </summary>
        int MaxOutputs { get; }
        /// <summary>
        /// Min outputs allowed
        /// </summary>
        int MinOutputs { get; }
        /// <summary>
        /// Use to determine the order in which arrays are evaluated
        /// </summary>
        List<uint> InputPriorities { get; }
        /// <summary>
        /// Stores the sensitivities foreach node, Note: arrays begin null
        /// </summary>
        List<Array> InputSensitivities { get; }
    }

    public enum TrainingMode
    {
        /// <summary>
        /// A single training point is used
        /// </summary>
        Incremental,
        /// <summary>
        /// train a batch of points
        /// </summary>
        Batch
    }
}
