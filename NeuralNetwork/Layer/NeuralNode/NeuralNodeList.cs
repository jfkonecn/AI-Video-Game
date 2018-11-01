using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuralNetwork.Layer.NeuralNode
{
    public class NeuralNodeList<T> : IList<T>
        where T : INeuralComponent
    {
        protected readonly List<T> _List = new List<T>();
        public NeuralNodeList() : base() { }

        public T this[int index] { get => _List[index]; set => _List[index] = value; }

        public int Count => _List.Count;

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            _List.Add(item);
        }

        public void Clear()
        {
            _List.Clear();
        }

        public bool Contains(T item)
        {
            foreach(T node in _List)
            {
                if ((item == null && node == null) || 
                    (item != null && item.Equals(node)) ||
                    (node is ILayer layer && layer.Nodes.Contains(item)))
                    return true;
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _List.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Extract all nodes of type t
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public IEnumerable<T> FindByType(Type t)
        {
            if (t.Equals(typeof(INeuralComponent)))
                return this;
            return this.Where((x) => x.GetType().Equals(t)).Select((x) => x);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _List.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return _List.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _List.Insert(index, item);
        }

        public bool Remove(T item)
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
