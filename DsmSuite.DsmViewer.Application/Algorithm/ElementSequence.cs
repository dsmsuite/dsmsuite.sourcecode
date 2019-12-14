using DsmSuite.DsmViewer.Model.Interfaces;
using System;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Application.Algorithm
{
    public class ElementSequence : IElementSequence
    {
        readonly int _numberOfElements;
        private readonly int[] _vector;

        public ElementSequence(int numberOfElements)
        {
            _numberOfElements = numberOfElements;
            _vector = new int[_numberOfElements];

            for (int i = 0; i < _numberOfElements; i++)
            {
                _vector[i] = i;
            }
        }

        public int GetNumberOfElements()
        {
            return _numberOfElements;
        }

        public void SetIndex(int currentIndex, int newIndex)
        {
            if (currentIndex >= _numberOfElements || currentIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(currentIndex));

            _vector[currentIndex] = newIndex;
        }

        public int GetIndex(int currentIndex)
        {
            if (currentIndex >= _numberOfElements || currentIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(currentIndex));

            return _vector[currentIndex];
        }

        public void Swap(int index1, int index2)
        {
            if (index1 >= _numberOfElements || index1 < 0)
                throw new ArgumentOutOfRangeException(nameof(index1));

            if (index2 >= _numberOfElements || index2 < 0)
                throw new ArgumentOutOfRangeException(nameof(index2));

            int temp = _vector[index1];
            _vector[index1] = _vector[index2];
            _vector[index2] = temp;
        }

        public bool IsValid()
        {
            HashSet<int> set = new HashSet<int>();
            for (int i = 0; i < _numberOfElements; i++)
            {
                set.Add(_vector[i]);
            }
            return set.Count == _numberOfElements;
        }
    }
}
