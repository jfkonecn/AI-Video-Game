using NeuralNetwork.NeuralMath.FiniteDifferenceFormulas;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNetwork.Layer.NeuralNode
{
    public class TransferFunction : BaseNode
    {
        public static readonly TransferFunction LogSigmoid =
            new TransferFunction(x => 1d / (1d + Math.Exp(-1d * x)), 
                x => Math.Exp(-1d * x) / Math.Pow(1d + Math.Exp(-1d * x), 2));
        public static readonly TransferFunction PureLine =
            new TransferFunction(x => x, x => 1);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fx">Must be defined for all x</param>
        /// <param name="fxPrime">Derivative of fx</param>
        public TransferFunction(Func<double, double> fx, Func<double, double> fxPrime)
        {
            Fx = fx;
            FxPrime = fxPrime;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fx">Must be defined for all x</param>
        /// <param name="fxPrime">Derivative of fx</param>
        public TransferFunction(Func<double, double> fx) : this(fx, (x) => FirstDerivative.TwoPointCentral(x, fx, 0.001))
        {

        }

        protected Func<double, double> Fx { get; }
        protected Func<double, double> FxPrime { get; }


        protected override void DetermineInputNodeSensitivity()
        {
            throw new NotImplementedException();
        }

        protected override void InternalCalculate()
        {
            throw new NotImplementedException();
        }

        protected override void InternalTrain(double learningRate, Array sensitivity)
        {
            throw new NotImplementedException();
        }
    }
}
