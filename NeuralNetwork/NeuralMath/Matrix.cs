using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NeuralNetwork.NeuralMath
{
    public static class Matrix
    {
        /// <summary>
        /// Adds two arrays
        /// </summary>
        /// <param name="leftArray"></param>
        /// <param name="rightArray"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static double[] Add(double[] leftArray, double[] rightArray)
        {
            return (double[])Add(leftArray, rightArray, DblAdder);
        }

        /// <summary>
        /// Adds two arrays
        /// </summary>
        /// <param name="leftArray"></param>
        /// <param name="rightArray"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static double[,] Add(double[,] leftArray, double[,] rightArray)
        {
            return (double[,])Add(leftArray, rightArray, DblAdder);
        }

        /// <summary>
        /// Adds two doubles
        /// </summary>
        /// <param name="left">must be a double</param>
        /// <param name="right">must be a double</param>
        /// <returns>a double casted as an object</returns>
        private static object DblAdder(object left, object right)
        {
            return (double)left + (double)right;
        }
        
        /// <summary>
        /// Adds two arrays
        /// </summary>
        /// <param name="leftArray"></param>
        /// <param name="rightArray"></param>
        /// <param name="adder">Adds an element from the right and the left and returns the result</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static Array Add(Array leftArray, Array rightArray, Func<object, object, object> adder)
        {
            bool badDim = false;
            if (leftArray.Rank != rightArray.Rank)
                badDim = true;
            else
            {
                for (int i = 0; i < leftArray.Rank; i++)
                {
                    if (leftArray.GetLength(i) != rightArray.GetLength(i))
                    {
                        badDim = true;
                    }
                }
            }
            if(badDim)
                throw new ArgumentOutOfRangeException("Left and right array must have the same dimensions!");

            Array result = (Array)leftArray.Clone();
            int[] indices = new int[result.Rank];
            for (int i = 0; i < indices.Length; i++)
                indices[i] = 0;
            
            while (true)
            {
                object obj = adder(leftArray.GetValue(indices), rightArray.GetValue(indices));
                result.SetValue(obj, indices);
                for (int i = 0; i < indices.Length; i++)
                {
                    indices[i]++;
                    if (indices[i] == leftArray.GetLength(i))
                    {
                        if (i == indices.Length - 1)
                            return result;
                        indices[i] = 0;
                    }
                    else
                        break;
                }
            }
        }

    }
}
