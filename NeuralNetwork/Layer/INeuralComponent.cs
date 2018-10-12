﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNetwork.Layer
{
    interface INeuralComponent
    {
        void Calculate();
        void Train();
        /// <summary>
        /// Set all entries to initial conditions
        /// </summary>
        void Reset();
        Guid Id { get; }
    }
}