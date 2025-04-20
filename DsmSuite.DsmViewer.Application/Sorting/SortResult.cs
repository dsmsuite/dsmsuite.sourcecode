using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Sorting
{
    public class SortResult : ISortResult
    {
        private readonly List<int> _list = new List<int>();

        public SortResult(string data)
        {
            string[] items = data.Split(',');

            _list.Clear();

            foreach (string item in items)
            {
                int value;
                if (int.TryParse(item, out value))
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

            foreach (var v in order)
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

        public void SetIndex(int index, int value)
        {
            CheckIndex(index);
            _list[index] = value;
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
                foreach (int value in _list)
                {
                    set.Add(value);
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
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
    }
}
