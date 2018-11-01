using System;

namespace NeuralNetwork.Layer.NeuralNode
{
    public interface ITransferFunction : INode
    {
        Func<double, double> Fx { get; set; }
        Func<double, double> FxPrime { get; set; }
    }
}