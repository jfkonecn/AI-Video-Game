using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using NeuralNetwork.NeuralMath;
using System.Diagnostics;

namespace UnitTests
{
    [TestClass]
    public class MatrixMath
    {
        /// <summary>
        ///  Gets or sets the test context which provides
        ///  information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Add()
        {
            double[,] A = {  { 5, 6, 1 },
                            { 5, 7, 8 },
                            { 9, 1, 3 }    };
            double[,] B = {  { 5, 6, 2 },
                            { 8, 9, 1 },
                            { 3, 5, 6 } };
            double[,] Expected = {{ 10, 12, 3 },
                                { 13, 16, 9 },
                                { 12, 6, 9 } };
            Assert.IsTrue(ArraysAreEqual(Matrix.Add(A, B), Expected), "ThreeXThree Add");
            Assert.IsTrue(ArraysAreEqual(Matrix.Add(new double[] { 3, 4 }, new double[] { 1, 1 }), new double[] { 4, 5 }), "vector Add");
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Matrix.Add(new double[] { 1, 2 }, new double[] { 1 }));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Matrix.Add(new double[,] { { 1, 2 } }, new double[,] { { 1 } }));
        }

        [TestMethod]
        public void AddSpeedTest()
        {
            Stopwatch sw = new Stopwatch();
            int size = 1000;
            double[,] A = new double[size, size];
            sw.Start();
            Matrix.Add(A, A);
            sw.Stop();
            TestContext.WriteLine($"Elapsed={sw.Elapsed}");
        }
        [TestMethod]
        public void Multiply()
        {
            Array A = new double[,] {  { 5, 6, 1 },
                            { 5, 7, 8 },
                            { 9, 1, 3 }    };
            Array B = new double[,]  {  { 5, 6, 2 },
                            { 8, 9, 1 },
                            { 3, 5, 6 } };
            Array Expected = new double[,]{ { 76, 89, 22 },
                                    { 105, 133, 65 },
                                    { 62, 78, 37 }    };
            Assert.IsTrue(ArraysAreEqual(Matrix.Multiply(A, B), Expected));
            A = new double[] { 5, 6, 1 };
            Expected = new double[]{ 76, 89, 22 };
            Assert.IsTrue(ArraysAreEqual(Matrix.Multiply(A, B), Expected));
            Expected = new double[]{ 63, 95, 51 };
            Assert.IsTrue(ArraysAreEqual(Matrix.Multiply(B, A), Expected));
            B = new double[] { 5, 8, 3 };
            Expected = new double[] { 76 };
            Assert.IsTrue(ArraysAreEqual(Matrix.Multiply(A, B), Expected));
            A = new double[,,] { { { 1 } } };
            Assert.ThrowsException<NotSupportedException>(() => Matrix.Multiply(A, B));
            Assert.ThrowsException<NotSupportedException>(() => Matrix.Multiply(B, A));
        }
        [TestMethod]
        public void Scalar()
        {
            double[,] A = {  { 5, 6, 1 },
                            { 5, 7, 8 },
                            { 9, 1, 3 }    };

            double[,] Expected = new double[,]{ { 50, 60, 10 },
                                    { 50, 70, 80 },
                                    { 90, 10, 30 }    };
            Assert.IsTrue(ArraysAreEqual(Matrix.ScalarMultiplication(10, A), Expected), "ThreeXThree Scalar");
        }
        [TestMethod]
        public void Mutate()
        {
            double[,] A = {  { 5, 6, 1 },
                            { 5, 7, 8 },
                            { 9, 1, 3 }    };
            double[,] B = {  { 5, 6, 1 },
                            { 5, 7, 8 },
                            { 9, 1, 3 }    };
            double low = -1, high = 1;
            Matrix.MutateMatrix(A, low, high, 0.2);
            for(int i = 0; i < A.GetLength(0); i++)
            {
                for(int j = 0; j < A.GetLength(1); j++)
                {
                    if (A[i, j] == B[i, j])
                        Assert.Fail();
                }
            }
            Assert.ThrowsException<ArgumentException>(() => Matrix.MutateMatrix(A, high, low, 1));
            Assert.ThrowsException<ArgumentException>(() => Matrix.MutateMatrix(A, low, high, -1));
        }

        private static void AssertArraysAreEqual(Array actual, Array expected, string msg = "")
        {
            Assert.IsTrue(ArraysAreEqual(actual, expected), msg);
        }

        private static bool ArraysAreEqual(Array A, Array B)
        {
            if (A.Rank != B.Rank)
                return false;
            int[] indices = new int[A.Rank];
            for (int i = 0; i < indices.Length; i++)
                indices[i] = 0;

            while (true)
            {
                if (!A.GetValue(indices).Equals(B.GetValue(indices)))
                    return false;



                for (int i = 0; i < indices.Length; i++)
                {
                    indices[i]++;
                    if (indices[i] == A.GetLength(i))
                    {
                        if(i == indices.Length - 1)
                            return true;
                        indices[i] = 0;
                    }                        
                    else
                        break;
                }
            }
            
        }
    }
}
