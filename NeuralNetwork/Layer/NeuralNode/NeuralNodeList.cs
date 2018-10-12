using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuralNetwork.Layer.NeuralNode
{
    internal class NeuralNodeList : List<NeuralNode>
    {
        public NeuralNodeList() : base() { }


        /// <summary>
        /// Extract all nodes of type t
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public IEnumerable<NeuralNode> FindByType(Type t)
        {
            if (t.Equals(typeof(NeuralNode)))
                return this;
            return this.Where((x) => x.GetType().Equals(t)).Select((x) => x);
        }

    }
}
