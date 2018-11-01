using NeuralNetwork.NeuralMath;
using NeuralNetwork.NeuralMath.FiniteDifferenceFormulas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace NeuralNetwork.Layer.NeuralNode
{
    public class TransferFunction : BaseNode, ITransferFunction
    {
        public TransferFunction() : base()
        {

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


        public Func<double, double> Fx { get; set; }
        public Func<double, double> FxPrime { get; set; }


        protected override void DetermineInputNodeSensitivity(Array sensitivity)
        {
            if (InputSensitivities[0] == null)
                InputSensitivities[0] = Matrix.CreateArrayWithMatchingDimensions(TempArray);
            Matrix.SetArraysEqualToEachOther(TempArray, InputSensitivities[0]);
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
