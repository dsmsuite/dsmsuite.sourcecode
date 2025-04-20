namespace DsmSuite.DsmViewer.Application.Sorting
{
    public class WeightsMatrix : ICloneable
    {
        readonly int[,] _weights;
        readonly int _size;

        public WeightsMatrix(int size)
        {
            _size = size;
            _weights = new int[_size, _size];
        }

        public object Clone()
        {
            WeightsMatrix sm = new WeightsMatrix(Size);

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    sm.SetWeight(i, j, GetWeight(i, j));
                }
            }

            return sm;
        }

        public int Size => _size;

        public void SetWeight(int i, int j, int weight)
        {
            CheckIndex(i);
            CheckIndex(i);

            _weights[i, j] = weight;
        }

        public int GetWeight(int i, int j)
        {
            CheckIndex(i);
            CheckIndex(i);

            return _weights[i, j];
        }

        private void CheckIndex(int index)
        {
            if ((index < 0) || (index >= _size))
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
    }
}
