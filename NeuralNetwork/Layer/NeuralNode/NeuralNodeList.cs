using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuralNetwork.Layer.NeuralNode
{
    public class NeuralNodeList : List<BaseNode>
    {
        public NeuralNodeList() : base() { }

        /// <summary>
        /// Extract all nodes of type t
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public IEnumerable<BaseNode> FindByType(Type t)
        {
            if (t.Equals(typeof(BaseNode)))
                return this;
            return this.Where((x) => x.GetType().Equals(t)).Select((x) => x);
        }

    }
}
