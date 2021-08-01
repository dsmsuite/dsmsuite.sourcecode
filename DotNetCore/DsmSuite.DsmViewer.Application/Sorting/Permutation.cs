
namespace DsmSuite.DsmViewer.Application.Sorting
{
    /// <summary>
    /// Represents a permutation of two values (order not important). 
    /// </summary>
    public class Permutation
    {
        readonly int _first;
        readonly int _second;

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        public Permutation(int value1, int value2)
        {
            // as the order is not important we sort them to make comparing permutations a little easier
            if (value1 < value2)
            {
                _first = value1;
                _second = value2;
            }
            else
            {
                _first = value2;
                _second = value1;
            }
        }

        /// <summary>
        /// Equals override so that permutations can be used as a key on a dictionary
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            Permutation test = obj as Permutation;
            if (test != null)
            {
                return test._first == _first &&
                       test._second == _second;
            }

            return false;
        }

        /// <summary>
        /// GetHashCode override so that permutations can be used as a key in a dictionary
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _first.GetHashCode() ^ _second.GetHashCode();
        }
    }
}
