using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuralNetwork.Matrix;
using NeuralNetwork.Node.Functions;
namespace NeuralNetwork.Node
{
    /****************************************************************************
     * r x 1 layer of neurons where r is the number of neurons. 
    *****************************************************************************/
    internal class LayerOfNeurons : AbstractNode
    {

        /****************************************************************************
         * Constructors 
         *****************************************************************************/

        /// <summary>
        /// Sets topInputNode = this.topInputNode.
        /// Sets bottomInputNode = OnesArray object else it's null.
        /// </summary>
        /// <param name="topInputNode"></param>
        /// <param name="outputLength">Number of elements in the output</param>
        /// <param name="makeOnesArray"></param>
        internal LayerOfNeurons(ref AbstractNode topInputNode, int outputLength, Function activationFun, bool makeOnesArray = true)
        {
            this.initializer(ref topInputNode, outputLength, activationFun, makeOnesArray);
            this.createWeights(outputLength);
        }

        /// <summary>
        /// Uses two node inputs instead of one input node and a ones node
        /// </summary>
        /// <param name="topInputNode"></param>
        /// <param name="bottomInputNode"></param>
        /// <param name="outputLength"></param>
        /// <param name="activationFun"></param>
        internal LayerOfNeurons(ref AbstractNode topInputNode, ref AbstractNode bottomInputNode, int outputLength, Function activationFun)
        {
            this.initializer(ref topInputNode, ref bottomInputNode, outputLength, activationFun);
            this.createWeights(outputLength);
        }

        /// <summary>
        /// makes copy of layer of neurons
        /// </summary>
        /// <param name="copyNode"></param>
        /// <param name="topInputNode"></param>
        /// <param name="outputLength"></param>
        /// <param name="makeOnesArray"></param>
        internal LayerOfNeurons(ref LayerOfNeurons copyNode, ref AbstractNode topInputNode)
        {
            bool hasOnesArray = true;
            if(copyNode.BottomInputNode == null)
            {
                hasOnesArray = false;
            }
            this.initializer(ref topInputNode, copyNode.OutputArray.Length, copyNode.ActivationFunction, hasOnesArray);
            this.copyWeights(ref copyNode);

        }

        /// <summary>
        /// makes copy of layer of neurons
        /// </summary>
        /// <param name="copyNode"></param>
        /// <param name="topInputNode"></param>
        /// <param name="bottomInputNode"></param>
        /// <param name="activationFun"></param>
        internal LayerOfNeurons(ref LayerOfNeurons copyNode, ref AbstractNode topInputNode, ref AbstractNode bottomInputNode)
        {
            this.initializer(ref topInputNode, ref bottomInputNode, copyNode.OutputArray.Length, copyNode.ActivationFunction);
            this.copyWeights(ref copyNode);
        }

        /****************************************************************************
         * Properties
        *****************************************************************************/
        private static Random rand = new Random();
        private double LowerWeightLimit { get; set; }
        private double UpperWeightLimit { get; set; }
        private double[,] TopInputWeights { get; set; }
        private double[,] BottomInputWeights { get; set; }
        private double[] onesInputWeights { get; set; }

        internal delegate double Function(double x);

        /// <summary>
        /// Function that determines what inputs are consider to be 0 or 1
        /// </summary>
        private Function ActivationFunction { get; set; }


        /****************************************************************************
         * Methods
         *****************************************************************************/
        /// <summary>
        /// initializes everything but the weights
        /// </summary>
        /// <param name="topInputNode"></param>
        /// <param name="outputLength"></param>
        /// <param name="activationFun"></param>
        /// <param name="makeOnesArray"></param>
        private void initializer(ref AbstractNode topInputNode, int outputLength, Function activationFun, bool makeOnesArray)
        {
            this.ActivationFunction = activationFun;
            this.UpperWeightLimit = 1;
            this.LowerWeightLimit = -1;
            //checks parameters
            if (topInputNode == null)
            {
                throw new System.ArgumentException("Parameter cannot be null", "topInputNode");
            }
            else if (outputLength < 1)
            {
                throw new System.ArgumentException("Parameter less than 1", "outputLength");
            }

            AbstractNode onesArray = null;

            if (makeOnesArray)
            {
                onesArray = new OnesArray(topInputNode.OutputArray.Length);
            }

            this.updateInputNodesOutputNodes(ref topInputNode, ref onesArray);
            this.setOutputArray = new double[outputLength];
        }

        /// <summary>
        /// initializes everything but the weights
        /// </summary>
        /// <param name="topInputNode"></param>
        /// <param name="bottomInputNode"></param>
        /// <param name="outputLength"></param>
        /// <param name="activationFun"></param>
        private void initializer(ref AbstractNode topInputNode, ref AbstractNode bottomInputNode, int outputLength, Function activationFun)
        {
            this.ActivationFunction = activationFun;
            this.UpperWeightLimit = 1;
            this.LowerWeightLimit = -1;
            //checks parameters
            if (topInputNode == null)
            {
                throw new System.ArgumentException("Parameter cannot be null", "topInputNode");
            }
            else if (bottomInputNode == null)
            {
                throw new System.ArgumentException("Parameter cannot be null", "bottomInputNode");
            }
            else if (topInputNode.OutputArray.Length != bottomInputNode.OutputArray.Length)
            {
                throw new System.ArgumentException("Lengths of top and bottom nodes must be equal", "getOutputLength");
            }
            else if (outputLength < 1)
            {
                throw new System.ArgumentException("Parameter less than 1", "outputLength");
            }

            this.BottomInputWeights = new double[this.BottomInputNode.OutputArray.Length, this.BottomInputNode.OutputArray.Length];

            this.updateInputNodesOutputNodes(ref topInputNode, ref bottomInputNode);
            this.setOutputArray = new double[outputLength];
        }


        /// <summary>
        /// Initializes top and bottom input weights.
        /// Assumes this.TopInputNode and this.BottomInputNode are already set.
        /// </summary>
        private void createWeights(int outputLength)
        {
            if (this.TopInputNode == null)
            {
                //attach an inputArray node if this is the beginning of your neural net
                throw new System.ArgumentException("Parameter cannot be null", "this.TopInputNode");
            }
            else
            {
                this.TopInputWeights = new double[this.TopInputNode.OutputArray.Length, outputLength];
                initializeArray(this.TopInputWeights, this.TopInputNode.OutputArray.Length, outputLength, this.LowerWeightLimit, this.UpperWeightLimit);
            }

            //onesInputWeights is null when a user doesn't want a onesArray node
            if (this.onesInputWeights != null)
            {
                initializeArray(this.onesInputWeights, this.LowerWeightLimit, this.UpperWeightLimit);
            }
            //if null then there is only one input node
            else if (this.BottomInputNode != null)
            {
                this.BottomInputWeights = new double[this.BottomInputNode.OutputArray.Length, outputLength];
                initializeArray(this.BottomInputWeights, this.BottomInputNode.OutputArray.Length, outputLength, this.LowerWeightLimit, this.UpperWeightLimit);
            }
        }

        /// <summary>
        /// copies over the weights from copyNode to this node
        /// </summary>
        /// <param name="copyNode"></param>
        private void copyWeights(ref LayerOfNeurons copyNode)
        {
            this.TopInputWeights = new double[copyNode.TopInputNode.OutputArray.Length, copyNode.OutputArray.Length];
            copyArray(this.TopInputWeights, copyNode.TopInputWeights, copyNode.TopInputNode.OutputArray.Length, copyNode.OutputArray.Length);


            if (this.onesInputWeights != null)
            {
                this.onesInputWeights = new double[copyNode.onesInputWeights.Length];
                copyArray(this.onesInputWeights, copyNode.onesInputWeights);
            }
            else if (this.BottomInputNode != null)
            {
                this.BottomInputWeights = new double[copyNode.BottomInputNode.OutputArray.Length, copyNode.OutputArray.Length];
                copyArray(this.BottomInputWeights, copyNode.BottomInputWeights, copyNode.BottomInputNode.OutputArray.Length, copyNode.OutputArray.Length);
            }
        }

        /// <summary>
        /// Copies numbers between two weight arrays
        /// </summary>
        /// <param name="cpyArray">the copy of the original array</param>
        /// <param name="orgArray">the original array</param>
        /// <param name="numRows"></param>
        /// <param name="numCols"></param>
        /// <param name="stdev"></param>
        private void copyArray(double[,] cpyArray, double[,] orgArray, int numRows, int numCols)
        {
            if (orgArray.Length < 1)
            {
                throw new System.ArgumentException("length must be >= 1", "length");
            }
            else if (orgArray == null)
            {
                throw new System.ArgumentException("Parameter cannot be null", "array");
            }

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    cpyArray[i, j] = orgArray[i, j];
                }
            }


        }

        
        /// <summary>
        /// Copies numbers between two weight arrays
        /// </summary>
        /// <param name="cpyArray">the copy of the original array</param>
        /// <param name="orgArray">the original array</param>
        /// <param name="numRows"></param>
        /// <param name="numCols"></param>
        /// <param name="stdev"></param>
        private void copyArray(double[] cpyArray, double[] orgArray)
        {
            if (orgArray.Length < 1)
            {
                throw new System.ArgumentException("length must be >= 1", "length");
            }
            else if (orgArray == null)
            {
                throw new System.ArgumentException("Parameter cannot be null", "array");
            }

            for (int i = 0; i < orgArray.Length; i++)
            {
                cpyArray[i] = orgArray[i];
            }


        }

        /// <summary>
        /// fills array with random numbers between [low, high]
        /// </summary>
        /// <param name="array"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        private void initializeArray(double[,] array, int numRows, int numCols, double low = -1, double high = 1)
        {
            if (low >= high)
            {
                throw new System.ArgumentException("low must be less than high", "high");
            }
            else if(numRows < 1 || numCols < 1)
            {
                throw new System.ArgumentException("numRows and numCols must be >= 1", "length");
            }





            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    
                    array[i, j] = rand.NextDouble() * (high - low) + low;
                }
            }
        }

        /// <summary>
        /// fills array with random numbers between [low, high]
        /// </summary>
        /// <param name="array"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        private void initializeArray(double[] array, double low = -1, double high = 1)
        {
            if (low >= high)
            {
                throw new System.ArgumentException("low must be less than high", "high");
            }
            else if (array.Length < 1)
            {
                throw new System.ArgumentException("length must be >= 1", "length");
            }
            else if (array == null)
            {
                throw new System.ArgumentException("Parameter cannot be null", "array");
            }

            array = new double[array.Length];




            for (int i = 0; i < array.Length; i++)
            {
                array[i] = rand.NextDouble() * (high - low) + low;
            }
        }

        /// <summary>
        /// alters all values in weight arrays based on a gaussian distribution
        /// </summary>
        internal override void adjustWeights(int sigID)
        {

            mutateArray(this.TopInputWeights, this.TopInputNode.OutputArray.Length, this.OutputArray.Length);


            if (this.onesInputWeights != null)
            {
                mutateArray(this.BottomInputWeights, this.BottomInputNode.OutputArray.Length, this.OutputArray.Length);
            }
            else if (this.BottomInputNode != null)
            {
                mutateArray(this.BottomInputWeights, this.BottomInputNode.OutputArray.Length, this.OutputArray.Length);
            }
            this.OutputNode.adjustWeights(sigID);

        }

        /// <summary>
        /// randomly changes all values in the array based on a gaussian distribution
        /// </summary>
        /// <param name="array"></param>
        /// <param name="stdev"></param>
        private void mutateArray(double[,] array, int numRows, int numCols, double stdev = 0.1)
        {
            if (stdev <= 0)
            {
                throw new System.ArgumentException("standard deviation cannot be <= 0", "stdev");
            }
            else if (array.Length < 1)
            {
                throw new System.ArgumentException("length must be >= 1", "length");
            }
            else if (array == null)
            {
                throw new System.ArgumentException("Parameter cannot be null", "array");
            }

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    //will check to see if above the lower or upper limit
                    double tempNum = Stats.ZScore.randomGaussianDistribution(array[i, j], stdev);
                    Math.Round(tempNum, 2);
                    if(this.LowerWeightLimit > tempNum)
                    {
                        tempNum = (this.LowerWeightLimit - tempNum) + this.LowerWeightLimit;
                    }
                    else if (this.UpperWeightLimit < tempNum)
                    {
                        tempNum = (this.UpperWeightLimit - tempNum) + this.UpperWeightLimit;
                    }
                    array[i, j] = tempNum;
                }
            }


        }


        /// <summary>
        /// calculates the output array then makes a call to the output node to calculate its results
        /// </summary>
        internal override void calculateResults(int sigID)
        {
            this.resetInternalResultsArrays();

            Matrix.Matrix.multiply(this.TopInputNode.OutputArray, this.TopInputNode.OutputArray.Length, this.TopInputWeights, this.OutputArray.Length, this.OutputArray);

            //onesInputWeights is null when a user doesn't want a onesArray node
            if (this.onesInputWeights != null)
            {
                Matrix.Matrix.add(this.OutputArray, this.onesInputWeights, this.OutputArray, this.OutputArray.Length);
            }
            //if null then there is only one input node
            else if (this.BottomInputNode != null)
            {
                double[] tempArray = new double[this.OutputArray.Length];
                Matrix.Matrix.multiply(this.BottomInputNode.OutputArray, this.BottomInputNode.OutputArray.Length, this.BottomInputWeights, this.OutputArray.Length, tempArray);
                Matrix.Matrix.add(this.OutputArray, tempArray, this.OutputArray, this.OutputArray.Length);
            }

            for (int i = 0; i < this.OutputArray.Length; i++)
            {
                this.OutputArray[i] = this.ActivationFunction(this.OutputArray[i]);
            }

            this.OutputNode.calculateResults(sigID);
        }

        /// <summary>
        /// clears the output array with zeros
        /// </summary>
        internal override void resetInternalResultsArrays()
        {
            for (int i = 0; i < this.OutputArray.Length; i++)
                this.OutputArray[i] = 0;
        }

        /// <summary>
        /// default activation function for the output array
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>


        internal override void flush(int sigID)
        {
            this.OutputNode.flush(sigID);
        }
    }
}
