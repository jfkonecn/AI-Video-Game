using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNetwork.Layer.Node
{
    /// <summary>
    /// An operation which creates two vectors from one vector
    /// </summary>
    internal abstract class PointwiseOperation : VectorContainer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="leftVector"><see cref="LeftVector"/></param>
        /// <param name="rightVector"><see cref="RightVector"/></param>
        /// <param name="ouputVectorLength">length of the resulting vector</param>
        /// <exception cref="ArgumentNullException">If vectors are null</exception>
        public PointwiseOperation(
            VectorContainer leftVector, 
            VectorContainer rightVector, 
            int ouputVectorLength) : base(new double[ouputVectorLength])
        {            
            LeftVector = leftVector ?? throw new ArgumentNullException(nameof(leftVector));
            RightVector = rightVector ?? throw new ArgumentNullException(nameof(rightVector));
        }
        /// <summary>
        /// Vector on the left side of operator
        /// </summary>
        protected VectorContainer LeftVector { get; }
        /// <summary>
        /// Vector on the right side of operator
        /// </summary>
        protected VectorContainer RightVector { get; }
    }
}
