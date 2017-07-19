using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.Matrix
{
    /****************************************************************************
    * does matrix math with arrays
    * assumes r x c
    * one dimensional arrays are considered to be r x 1 arrays 
    *****************************************************************************/
    internal class Matrix
    {
        /****************************************************************************
         * Methods
         *****************************************************************************/
        /// <summary>
        /// single dimensional array add
        /// assumes the array lengths are the same
        /// assume output, left and right are 1 x c xor 1 x r
        /// </summary>
        /// <param name="leftArray"></param>
        /// <param name="rightArray"></param>
        /// <param name="outputArray"></param>
        /// <param name="arrayLength"></param>
        internal static void add(double[] leftArray, double[] rightArray, double[] outputArray, int arrayLength)
        {
            for (int i = 0; i < arrayLength; i++)
            {
                outputArray[i] = leftArray[i] + rightArray[i];
            }
        }

        /// <summary>
        /// two dimensional array add
        /// assumes the array row and column lengths are the same
        /// </summary>
        /// <param name="leftArray"></param>
        /// <param name="rightArray"></param>
        /// <param name="outputArray"></param>
        /// <param name="rowLength"></param>
        /// <param name="columnLength"></param>
        internal static void add(double[,] leftArray, double[,] rightArray, double[,] outputArray, int rowLength, int columnLength)
        {
            for (int i = 0; i < rowLength; i++)
            {
                for (int j = 0; j < columnLength; j++)
                {
                    outputArray[i, j] = leftArray[i, j] + rightArray[i, j];
                }
            }
        }

        /// <summary>
        /// single dimensional array subtraction assumes the array lengths are the same
        /// </summary>
        /// <param name="leftArray"></param>
        /// <param name="rightArray"></param>
        /// <param name="outputArray"></param>
        /// <param name="arrayLength"></param>
        internal static void subtract(double[] leftArray, double[] rightArray, double[] outputArray, int arrayLength)
        {
            for (int i = 0; i < arrayLength; i++)
            {
                outputArray[i] = leftArray[i] - rightArray[i];
            }
        }


        /// <summary>
        /// two dimensional array subtraction
        /// C = A - B where A is the left, B is the right and C is returned
        /// assumes the array row and column lengths are the same
        /// </summary>
        /// <param name="leftArray"></param>
        /// <param name="rightArray"></param>
        /// <param name="outputArray"></param>
        /// <param name="rowLength"></param>
        /// <param name="columnLength"></param>
        internal static void subtract(double[,] leftArray, double[,] rightArray, double[,] outputArray, int rowLength, int columnLength)
        {
            for (int i = 0; i < rowLength; i++)
            {
                for (int j = 0; j < columnLength; j++)
                {
                    outputArray[i, j] = leftArray[i, j] - rightArray[i, j];
                }
            }
        }

        /// <summary>
        /// Multiplies matrix by a scalar assumes length of arrays are identical
        /// </summary>
        /// <param name="scalar"></param>
        /// <param name="curArray"></param>
        /// <param name="resultsArray"></param>
        /// <param name="arrayLength"></param>
        internal static void scalarMultiplication(double scalar, double[] curArray, double[] resultsArray, int arrayLength)
        {
            for (int i = 0; i < arrayLength; i++)
            {
                resultsArray[i] = curArray[i] * scalar;
            }
        }

        /// <summary>
        /// Multiplies matrix by a scalar
        /// assumes length of arrays are identical
        /// </summary>
        /// <param name="scalar"></param>
        /// <param name="curArray"></param>
        /// <param name="resultsArray"></param>
        /// <param name="rowLength"></param>
        /// <param name="columnLength"></param>
        internal static void scalarMultiplication(double scalar, double[,] curArray, double[,] resultsArray, int rowLength, int columnLength)
        {
            for (int i = 0; i < rowLength; i++)
            {
                for (int j = 0; j < columnLength; j++)
                {
                        resultsArray[i, j] = curArray[i, j] * scalar;
                }
            }
        }

        /// <summary>
        /// multiplies two matrices  
        /// assumes one dimensional left array is a 1 x c array
        /// assumes that leftArrayLength is equal to the number of rows in right Array
        /// assumes length of outputArray is equal to rightArrayRows
        /// </summary>
        /// <param name="leftArray"></param>
        /// <param name="leftArrayLength"></param>
        /// <param name="rightArray"></param>
        /// <param name="rightArrayRows"></param>
        /// <param name="outputArray"></param>
        /****************************************************************************

        *****************************************************************************/
        internal static void multiply(double[] leftArray, int leftArrayLength, double[,] rightArray, int rightArrayRows, double[] outputArray)
        {
            for (int i = 0; i < rightArrayRows; i++)
            {
                outputArray[i] = 0;
                for (int j = 0; j < leftArrayLength; j++)
                {
                    outputArray[i] += leftArray[j] * rightArray[j, i];
                }
            }
        }

        /// <summary>
        /// multiplies two matrices  
        /// assumes one dimensional left array is a 1 x c array
        /// assumes that leftArrayColumns is equal to the number of rows in right Array
        /// assumes column length of outputArray is equal to rightArrayColumns
        /// assumes row length of outputArray is equal to leftArrayRows
        /// </summary>
        /// <param name="leftArray"></param>
        /// <param name="leftArrayRows"></param>
        /// <param name="leftArrayColumns"></param>
        /// <param name="rightArray"></param>
        /// <param name="rightArrayRows"></param>
        /// <param name="rightArrayColumns"></param>
        /// <param name="outputArray"></param>
        internal static void multiply(double[,] leftArray, int leftArrayRows, int leftArrayColumns, double[,] rightArray, int rightArrayRows, int rightArrayColumns, double[,] outputArray)
        {
            for (int r = 0; r < leftArrayRows; r++)
            {
                for (int i = 0; i < leftArrayColumns; i++)
                {
                    outputArray[r, i] = 0;
                    for (int j = 0; j < rightArrayRows; j++)
                    {
                        outputArray[r, i] += leftArray[i, j] * rightArray[j, i];
                    }
                }
            }
        }
    }
}
