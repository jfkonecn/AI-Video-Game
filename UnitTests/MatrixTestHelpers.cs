using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    public static class MatrixTestHelpers
    {
        public static bool ArraysAreEqual(Array A, Array B)
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
                        if (i == indices.Length - 1)
                            return true;
                        indices[i] = 0;
                    }
                    else
                        break;
                }
            }

        }

        public static void AssertArraysAreEqual(Array actual, Array expected, string msg = "")
        {
            Assert.IsTrue(MatrixTestHelpers.ArraysAreEqual(actual, expected), msg);
        }
    }
}
