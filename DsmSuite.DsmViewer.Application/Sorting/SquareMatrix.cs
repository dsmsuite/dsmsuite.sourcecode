using System;

namespace DsmSuite.DsmViewer.Application.Algorithm
{
    public class SquareMatrix : ICloneable
    {
        readonly int[,] _matrix;
        readonly int _size;

        public SquareMatrix(int width)
        {
            _size = width;

            _matrix = new int[_size, _size];
        }

        public object Clone()
        {
            SquareMatrix sm = new SquareMatrix(Size);

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    sm.Set(i, j, Get(i, j));
                }
            }

            return sm;
        }

        public int Size => _size;

        public void Set(int i, int j, int value)
        {
            if (i >= _size || i < 0)
                throw new ArgumentOutOfRangeException(nameof(i));

            if (j >= _size || j < 0)
                throw new ArgumentOutOfRangeException(nameof(j));

            _matrix[i, j] = value;
        }



        public int Get(int i, int j)
        {
            if (i >= _size || i < 0)
                throw new ArgumentOutOfRangeException(nameof(i));

            if (j >= _size || j < 0)
                throw new ArgumentOutOfRangeException(nameof(j));

            return _matrix[i, j];
        }
    }
}
