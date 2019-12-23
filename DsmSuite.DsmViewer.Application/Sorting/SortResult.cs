using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System;

namespace DsmSuite.DsmViewer.Application.Algorithm
{
    public class SortResult : ISortResult
    {
        private List<int> _list = new List<int>();

        public SortResult(string data)
        {
            string[] items = data.Split(',');

            _list.Clear();

            for (int i = 0; i < items.Length; i++)
            {
                int value = 0;
                if (int.TryParse(items[i], out value))
                {
                    _list.Add(value);
                }
            }
        }

        public SortResult(int numberOfElements)
        {
            _list.Clear();

            for (int i = 0; i < numberOfElements; i++)
            {
                _list.Add(i);
            }
        }

        public int GetNumberOfElements()
        {
            return _list.Count;
        }

        public void InvertOrder()
        {
            List<KeyValuePair<int, int>> order = new List<KeyValuePair<int, int>>();
            for (int i = 0; i < _list.Count; i++)
            {
                order.Add(new KeyValuePair<int, int>(i, _list[i]));
            }

            foreach(var v in order)
            {
                _list[v.Value] = v.Key;
            }
        }

        public void Swap(int index1, int index2)
        {
            CheckIndex(index1);
            CheckIndex(index2);
            int temp = _list[index1];
            _list[index1] = _list[index2];
            _list[index2] = temp;
        }

        public int GetIndex(int index)
        {
            CheckIndex(index);
            return _list[index];
        }

        public string Data
        {
            get
            {
                string data = "";
                for (int i = 0; i < _list.Count; i++)
                {
                    data += _list[i].ToString();

                    if (i < _list.Count - 1)
                    {
                        data += ",";
                    }
                }
                return data;
            }
        }

        public bool IsValid
        {
            get
            {
                HashSet<int> set = new HashSet<int>();
                for (int i = 0; i < _list.Count; i++)
                {
                    set.Add(_list[i]);
                }

                bool valid = true;
                for (int i = 0; i < _list.Count; i++)
                {
                    if (!set.Contains(i))
                    {
                        valid = false;
                    }
                }
                return (_list.Count > 0) && valid;
            }
        }
        
        private void CheckIndex(int index)
        {
            if ((index < 0) || (index >= _list.Count))
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}
