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
            Debug.WriteLine("hello world");
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
