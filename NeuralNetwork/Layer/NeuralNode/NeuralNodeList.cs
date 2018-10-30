using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuralNetwork.Layer.NeuralNode
{
    public class NeuralNodeList : IList<INeuralNode>
    {
        protected readonly List<INeuralNode> _List = new List<INeuralNode>();
        public NeuralNodeList() : base() { }

        public INeuralNode this[int index] { get => _List[index]; set => _List[index] = value; }

        public int Count => _List.Count;

        public bool IsReadOnly => false;

        public void Add(INeuralNode item)
        {
            _List.Add(item);
        }

        public void Clear()
        {
            _List.Clear();
        }

        public bool Contains(INeuralNode item)
        {
            foreach(INeuralNode node in _List)
            {
                if ((item == null && node == null) || 
                    (item != null && item.Equals(node)) ||
                    (node is BaseLayer layer && layer.Nodes.Contains(item)))
                    return true;
            }
            return false;
        }

        public void CopyTo(INeuralNode[] array, int arrayIndex)
        {
            _List.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Extract all nodes of type t
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public IEnumerable<INeuralNode> FindByType(Type t)
        {
            if (t.Equals(typeof(INeuralNode)))
                return this;
            return this.Where((x) => x.GetType().Equals(t)).Select((x) => x);
        }

        public IEnumerator<INeuralNode> GetEnumerator()
        {
            return _List.GetEnumerator();
        }

        public int IndexOf(INeuralNode item)
        {
            return _List.IndexOf(item);
        }

        public void Insert(int index, INeuralNode item)
        {
            _List.Insert(index, item);
        }

        public bool Remove(INeuralNode item)
        {
            return _List.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _List.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_List).GetEnumerator();
        }
    }
}
