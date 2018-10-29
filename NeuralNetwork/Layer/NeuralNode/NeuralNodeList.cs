using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuralNetwork.Layer.NeuralNode
{
    public class NeuralNodeList : IList<BaseNode>
    {
        protected readonly List<BaseNode> _List = new List<BaseNode>();
        public NeuralNodeList() : base() { }

        public BaseNode this[int index] { get => _List[index]; set => _List[index] = value; }

        public int Count => _List.Count;

        public bool IsReadOnly => false;

        public void Add(BaseNode item)
        {
            _List.Add(item);
        }

        public void Clear()
        {
            _List.Clear();
        }

        public bool Contains(BaseNode item)
        {
            foreach(BaseNode node in _List)
            {
                if ((item == null && node == null) || 
                    (item != null && item.Equals(node)) ||
                    (node is BaseLayer layer && layer.Nodes.Contains(item)))
                    return true;
            }
            return false;
        }

        public void CopyTo(BaseNode[] array, int arrayIndex)
        {
            _List.CopyTo(array, arrayIndex);
        }

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

        public IEnumerator<BaseNode> GetEnumerator()
        {
            return _List.GetEnumerator();
        }

        public int IndexOf(BaseNode item)
        {
            return _List.IndexOf(item);
        }

        public void Insert(int index, BaseNode item)
        {
            _List.Insert(index, item);
        }

        public bool Remove(BaseNode item)
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
