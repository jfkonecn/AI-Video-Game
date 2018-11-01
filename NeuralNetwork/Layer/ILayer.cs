using System;
using System.Collections.Generic;
using NeuralNetwork.Layer.NeuralNode;

namespace NeuralNetwork.Layer
{
    public interface ILayer : INeuralComponent, IEnumerable<INeuralComponent>
    {
        int Count { get; }
        Vector Input { get; set; }
        NeuralNodeList<INeuralComponent> Nodes { get; }
        Vector Output { get; set; }

        void AddNode(INeuralComponent node);
        void ConnectNodes(INeuralComponent from, INeuralComponent to, uint priority);
        bool Contains(BaseNode node);
        void Mutate(double lowerValueLimit, double upperValueLimit, double stdDev);
        bool Remove(INeuralComponent node);
    }
}