using System;
using System.Collections.Generic;

namespace NeuralNetwork.Layer.NeuralNode
{
    public interface INode : INeuralComponent
    {
        Array Sensitivity { get; }

        Array OutputArray { get; set; }
    }
}