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
        public static Array Add(Array leftArray, Array rightArray)
        {
            Array outputArray = CreateArrayWithMatchingDimensions(leftArray);
            Add(leftArray, rightArray, outputArray);
            return outputArray;
        }
        /// <summary>
        /// Adds two arrays
        /// </summary>
        /// <param name="leftArray"></param>
        /// <param name="rightArray"></param>
        /// <param name="outputArray">Dumps results into this array</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void Add(Array leftArray, Array rightArray, Array outputArray)
        {
            CheckIfArraysAreSameSize(true, leftArray, rightArray, outputArray);
            Func<object, object, object> adder = DetermineAdder(leftArray.GetType().GetElementType());
            PerformActionOnEachArrayElement(outputArray, (indices) => 
            {
                object obj = adder(leftArray.GetValue(indices), rightArray.GetValue(indices));
                outputArray.SetValue(obj, indices);
            });
        }


        /// <summary>
        /// Multiplies matrix by a scalar
        /// </summary>
        /// <param name="scalar"></param>
        /// <param name="inputArray"></param>
        /// <param name="outputArray">Dumps results into this array</param>
        /// <returns></returns>
        public static void ScalarMultiplication(object scalar, Array inputArray, Array outputArray)
        {
            Func<object, object, object> multiplier = DetermineMultiplier(inputArray.GetType().GetElementType());
            PerformActionOnEachArrayElement(inputArray, (indices) => 
            {
                object obj = outputArray.GetValue(indices);
                outputArray.SetValue(multiplier(scalar, obj), indices);
            });
        }

        /// <summary>
        /// Multiplies matrix by a scalar
        /// </summary>
        /// <param name="scalar"></param>
        /// <param name="inputOuputArray">Multiplies by the scalar in for each element and palaces the results in the same array</param>
        /// <returns></returns>
        public static void ScalarMultiplication(object scalar, Array inputOuputArray)
        {
            ScalarMultiplication(scalar, inputOuputArray, inputOuputArray);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">Type to be multiplied</param>
        /// <returns>A function which passes two arguments of the passed type, multiplies them, then returns the result</returns>
        /// <exception cref="Exception"></exception>
        private static Func<object, object, object> DetermineMultiplier(Type type)
        {
            Func<object, object, object> multiplier;
            if (type.Equals(typeof(double)))
            {
                multiplier = (x, y) => { return Convert.ToDouble(x) * Convert.ToDouble(y); };
            }
            else if (type.Equals(typeof(int)))
            {
                multiplier = (x, y) => { return Convert.ToInt32(x) * Convert.ToInt32(y); };
            }
            else
            {
                MethodInfo addMeth = type.GetMethod("op_Multiply");
                if (addMeth == null)
                    throw new Exception("Left array elements do not have the multiply operator overloaded");
                multiplier = (x, y) => { return addMeth.Invoke(null, new object[] { x, y }); };
            }
            return multiplier;
        }
        /// <summary>
        /// Finds correct adder for given type
        /// </summary>
        /// <param name="type"></param>
        /// <returns>A function which passes two arguments of the passed type, adds them, then returns the result</returns>
        /// <exception cref="Exception"></exception>
        private static Func<object, object, object> DetermineAdder(Type type)
        {
            Func<object, object, object> adder;
            if (type.Equals(typeof(double)))
            {
                adder = (x, y) => { return Convert.ToDouble(x) + Convert.ToDouble(y); };
            }
            else if (type.Equals(typeof(int)))
            {
                adder = (x, y) => { return Convert.ToInt32(x) + Convert.ToInt32(y); };
            }
            else
            {
                MethodInfo addMeth = type.GetMethod("op_Addition");
                if (addMeth == null)
                    throw new Exception("Left array elements do not have the add operator overloaded");
                adder = (x, y) => { return addMeth.Invoke(null, new object[] { x, y }); };
            }
            return adder;
        }


        /// <summary>
        /// Randomly changes elements of the passes array within the upper and lower value limits
        /// </summary>
        /// <param name="array"></param>
        /// <param name="lowerValueLimit"></param>
        /// <param name="upperValueLimit"></param>
        /// <param name="stdev">Standard deviation, used to create a bell curve centered on the current element values in the array so that the weights have a higher chance of not changing drastically </param>
        public static void MutateMatrix(double[] array, double lowerValueLimit, double upperValueLimit, double stdev)
        {
            MutateMatrix((Array)array, lowerValueLimit, upperValueLimit, stdev);
        }

        /// <summary>
        /// Randomly changes elements of the passes array within the upper and lower value limits
        /// </summary>
        /// <param name="array"></param>
        /// <param name="lowerValueLimit"></param>
        /// <param name="upperValueLimit"></param>
        /// <param name="stdev">Standard deviation, used to create a bell curve centered on the current element values in the array so that the weights have a higher chance of not changing drastically </param>
        public static void MutateMatrix(double[,] array, double lowerValueLimit, double upperValueLimit, double stdev)
        {
            MutateMatrix((Array)array, lowerValueLimit, upperValueLimit, stdev);
        }

        /// <summary>
        /// Randomly changes elements of the passes array within the upper and lower value limits
        /// </summary>
        /// <param name="array">ASSUMES array is a double</param>
        /// <param name="lowerValueLimit"></param>
        /// <param name="upperValueLimit"></param>
        /// <param name="stdev">Standard deviation, used to create a bell curve centered on the current element values in the array so that the weights have a higher chance of not changing drastically </param>
        private static void MutateMatrix(Array array, double lowerValueLimit, double upperValueLimit, double stdev)
        {
            if (stdev <= 0)
            {
                throw new ArgumentException("standard deviation cannot be <= 0", nameof(stdev));
            }
            else if (lowerValueLimit >= upperValueLimit)
            {
                throw new ArgumentException("lowerValueLimit must be lower than the upper value limit", nameof(upperValueLimit));
            }
            PerformActionOnEachArrayElement(array, (indices) =>
            {
                double idxVal = (double)array.GetValue(indices);
                // make sure that the element isn't already out of range
                if (lowerValueLimit > idxVal)
                {
                    idxVal = lowerValueLimit;
                }
                else if (upperValueLimit < idxVal)
                {
                    idxVal = upperValueLimit;
                }
                // will check to see if above the lower or upper limit
                idxVal = Stats.ZScore.RandomGaussianDistribution(idxVal, stdev);
                Math.Round(idxVal, 2);
                if (lowerValueLimit > idxVal)
                {
                    idxVal = (lowerValueLimit - idxVal) + lowerValueLimit;
                }
                else if (upperValueLimit < idxVal)
                {
                    idxVal = (upperValueLimit - idxVal) + upperValueLimit;
                }
                array.SetValue(idxVal, indices);
            });

        }

        /// <summary>
        /// Performs an action for each combination of indices of the passed array
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="action">passes an array of ints which represents a combination of indices</param>
        private static void PerformActionOnEachArrayElement(Array arr, Action<int[]> action)
        {
            int[] indices = new int[arr.Rank];
            for (int i = 0; i < indices.Length; i++)
                indices[i] = 0;

            while (true)
            {
                action(indices);

                for (int i = 0; i < indices.Length; i++)
                {
                    indices[i]++;
                    if (indices[i] == arr.GetLength(i))
                    {
                        if (i == indices.Length - 1)
                            return;
                        indices[i] = 0;
                    }
                    else
                        break;
                }
            }
        }

        /// <summary>
        /// Sets all elements in the passed array to the passed value
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="value"></param>
        public static void SetAll(Array arr, object value)
        {
            PerformActionOnEachArrayElement(arr, (indices) => { arr.SetValue(value, indices); });
        }

        /// <summary>
        /// Creates an array with matching dimensions
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static Array CreateArrayWithMatchingDimensions(Array arr)
        {
            int[] lengths = new int[arr.Rank];
            for (int i = 0; i < arr.Rank; i++)
            {
                lengths[i] = arr.GetLength(i);
            }
            return Array.CreateInstance(arr.GetType().GetElementType(), lengths);
        }
        /// <summary>
        /// Make each element in output array a shallow copy of each element in inputArray
        /// </summary>
        /// <param name="inputArray"></param>
        /// <param name="outputArray"></param>
        public static void SetArraysEqualToEachOther(Array inputArray, Array outputArray)
        {
            CheckIfArraysAreSameSize(true, inputArray, outputArray);
            PerformActionOnEachArrayElement(inputArray, (indices) => 
            {
                outputArray.SetValue(inputArray.GetValue(indices), indices);
            });
        }

        /// <summary>
        /// Checks if all the passed arrays have the size.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="throwException">If true, throws <see cref="ArgumentOutOfRangeException"/> if arrays are not the same size</param>
        /// <param name="arrays"></param>
        /// <returns>true if arrays are the same size</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static bool CheckIfArraysAreSameSize(bool throwException, Array arr,  params Array[] arrays)
        {
            bool badDim = false;
            for(int i = 0; i < arrays.Length; i++)
            {
                if (arr.Rank != arrays[i].Rank)
                    badDim = true;
                else
                    for (int j = 0; j < arr.Rank; j++)
                        if (arr.GetLength(j) != arrays[i].GetLength(j))
                            badDim = true;
                if (badDim)
                    break;
            }
            if (badDim && throwException)
                throw new ArgumentOutOfRangeException("All arrays must have the same dimensions!");
            return !badDim;
        }

        /// <summary>
        /// multiplies two arrays  
        /// </summary>
        /// <param name="leftArray">With a rank no greater than 2</param>
        /// <param name="rightArray">With a rank no greater than 2</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public static Array Multiply(Array leftArray, Array rightArray)
        {
            Array outputArray;
            if (leftArray.Rank == 1 && rightArray.Rank == 1)
                outputArray = Array.CreateInstance(leftArray.GetType().GetElementType(), 1);
            else if (leftArray.Rank == 1)
                outputArray = Array.CreateInstance(leftArray.GetType().GetElementType(), leftArray.Length);
            else if (rightArray.Rank == 1)
                outputArray = Array.CreateInstance(leftArray.GetType().GetElementType(), rightArray.Length);
            else
                outputArray = Array.CreateInstance(leftArray.GetType().GetElementType(), 
                    leftArray.GetLength(0), rightArray.GetLength(1));
            Multiply(leftArray, rightArray, outputArray);
            return outputArray;
        }


        /// <summary>
        /// multiplies two arrays  
        /// </summary>
        /// <param name="leftArray">With a rank no greater than 2</param>
        /// <param name="rightArray">With a rank no greater than 2</param>
        /// <param name="outputArray">Dumps results into this array Note: [r1 x c1] * [r2 x c2] = [r1 x c2]</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public static void Multiply(Array leftArray, Array rightArray, Array outputArray)
        {
            if (leftArray.Rank > 2 || rightArray.Rank > 2)
                throw new NotSupportedException("Arrays with a rank greater than 2 are not supported");
            int leftArrRows = leftArray.Rank == 1 ? 1 : leftArray.GetLength(0),
                leftArrCols = leftArray.Rank == 1 ? leftArray.Length : leftArray.GetLength(1),
                rightArrRows = rightArray.Rank == 1 ? rightArray.Length : rightArray.GetLength(0),
                rightArrCols = rightArray.Rank == 1 ? 1 : rightArray.GetLength(1);

            if (leftArrCols != rightArrRows)
            {
                throw new ArgumentOutOfRangeException("leftArray's columns must be equal to rightArray's rows");
            }

            Func<int, int, object>
                getOutputArrEle = (i, j) => { return outputArray.GetValue(i, j); },
                getLeftArrEle = (i, j) => { return leftArray.GetValue(i, j); },
                getRightArrEle = (i, j) => { return rightArray.GetValue(i, j); }; 
            Action<object, int, int> setOutputArrEle = (obj, i, j) => { outputArray.SetValue(obj, i, j); };

            if (leftArray.Rank == 1)
            {
                getLeftArrEle = (i, j) => { return leftArray.GetValue(j); };
                setOutputArrEle = (obj, i, j) => { outputArray.SetValue(obj, j); };
                getOutputArrEle = (i, j) => { return outputArray.GetValue(j); };
            }                
            if (rightArray.Rank == 1)
            {
                getRightArrEle = (i, j) => { return rightArray.GetValue(i); };
                setOutputArrEle = (obj, i, j) => { outputArray.SetValue(obj, i); };
                getOutputArrEle = (i, j) => { return outputArray.GetValue(i); };
            }
                
            Func<object, object, object> adder = DetermineAdder(leftArray.GetType().GetElementType());
            Func<object, object, object> multiplier = DetermineMultiplier(leftArray.GetType().GetElementType());
            for (int r = 0; r < leftArrRows; r++)
            {
                for (int i = 0; i < rightArrCols; i++)
                {
                    for (int j = 0; j < rightArrRows; j++)
                    {
                        setOutputArrEle(
                            adder(getOutputArrEle(r, i),
                            multiplier(getLeftArrEle(r, j), getRightArrEle(j, i))), 
                            r, i);
                    }
                }
            }
        }

    }
}
