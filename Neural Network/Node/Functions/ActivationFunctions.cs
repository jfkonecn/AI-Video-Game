using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.Node.Functions
{
    class ActivationFunctions
    {
        internal static double defaultActivationFunction(double n)
        {
            if (n >= 0.5)
            {
                return 1;
            }
            return 0;
        }
    }
}
