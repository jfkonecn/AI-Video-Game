using NeuralNetwork.NeuralMath;
using NeuralNetwork.NeuralMath.FiniteDifferenceFormulas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace NeuralNetwork.Layer.NeuralNode
{
    public class TransferFunction : BaseNode
    {
        public static TransferFunction LogSigmoid()
        {
            return new TransferFunction(x => 1d / (1d + Math.Exp(-1d * x)),
                x => Math.Exp(-1d * x) / Math.Pow(1d + Math.Exp(-1d * x), 2));
        }

        public static TransferFunction PureLine()
        {
            return new TransferFunction(x => x, x => 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fx">Must be defined for all x</param>
        /// <param name="fxPrime">Derivative of fx</param>
        public TransferFunction(Func<double, double> fx, Func<double, double> fxPrime) : base()
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

        /// <summary>
        /// Copy Constructor
        /// </summary>
        public TransferFunction(TransferFunction old) : base(old)
        {
            Fx = old.Fx;
            FxPrime = old.FxPrime;
        }

        protected Func<double, double> Fx { get; }
        protected Func<double, double> FxPrime { get; }


        protected override void DetermineInputNodeSensitivity()
        {
            if (InputSensitivities[0] == null)
                InputSensitivities[0] = Matrix.CreateArrayWithMatchingDimensions(Sensitivity);
            Matrix.SetArraysEqualToEachOther(Sensitivity, InputSensitivities[0]);
        }

        protected override void InternalCalculate()
        {
            if (InputNeighbors.Count != 1)
                throw new ArgumentException("Must have exactly one input!", nameof(InputNeighbors));

            Array inputArray = InputNeighbors[0].OutputArray;
            if (OutputArray == null || Sensitivity == null || TempArray == null
                || !Matrix.CheckIfArraysAreSameSize(false, inputArray, OutputArray))
            {
                OutputArray = Matrix.CreateArrayWithMatchingDimensions(inputArray);
                Sensitivity = Matrix.CreateArrayWithMatchingDimensions(inputArray);
                TempArray = Matrix.CreateArrayWithMatchingDimensions(inputArray);
            }
            Matrix.PerformActionOnEachArrayElement(inputArray, (indices) =>
            {
                OutputArray.SetValue(Fx((double)inputArray.GetValue(indices)), indices);
            });

        }

        protected override void InternalUpdateSensitivities(Array sensitivity, TrainingMode trainingMode)
        {
            Array inputArray = InputNeighbors[0].OutputArray;
            Matrix.PerformActionOnEachArrayElement(inputArray, (indices) =>
            {
                TempArray.SetValue(FxPrime((double)inputArray.GetValue(indices)) * (double)sensitivity.GetValue(indices), indices);
            });
            base.InternalUpdateSensitivities(TempArray, trainingMode);
        }
        /// <summary>
        /// Stores the derivative of the transfer function evaluated at the inputArray
        /// </summary>
        [XmlIgnore]
        private Array TempArray { get; set; }

        public override int MaxInputs => 1;

        public override int MinInputs => 1;

        public override int MaxOutputs => 1;

        public override int MinOutputs => 1;
    }
}
