using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNetwork.Layer
{
    interface INeuralComponent
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
