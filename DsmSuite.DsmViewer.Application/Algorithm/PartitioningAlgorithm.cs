using System.Collections.Generic;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Algorithm
{
    /// <summary>
    /// This class is responsable for the partitioning calculation which for a given square matrix orders the rows
    /// as close as possible to the lower block triangular form.  The idea being that as many empty cells are pushed
    /// into the upper triangle as possible.  It is probable that some cells will be non-zero - in this
    /// case they are pushed towards the bottom right corner
    /// </summary>
    /// <remarks>
    /// <para>
    /// The alogorithm first pushes empty rows to the top and complete rows to the bottom.  Partially complete rows
    /// are then processed using a distance score to push them below but as close as possible to the diagonal</para>
    /// <para>The result of the calculation is contained in the vector class which specifies how the new order of row indexes</para>
    /// </remarks>
    class PartitioningAlgorithm
    {
        readonly SquareMatrix _sm;

        /// <summary>
        /// Constructor of calcualtion on a given n * n matrix
        /// </summary>
        /// <param name="matrix"></param>
        public PartitioningAlgorithm(SquareMatrix matrix)
        {
            _sm = matrix;
        }

        /// <summary>
        /// Run the partition calculation
        /// </summary>
        /// <returns>The result in the form of a vector</returns>
        public Vector Partition()
        {
            Vector vector = new Vector(_sm.Size);

            DoPartitioning(vector);

            return vector;
        }

        /// <summary>
        /// the main partitioning algo.
        /// </summary>
        /// <param name="vector"></param>
        void DoPartitioning(Vector vector)
        {
            // Move all empty rows to the top - an save the index of the first non empty row (start)
            int start = MoveZeroRows(vector);

            // Move all the complete rows to the bottom - save the index of the first full row (end)
            int end = MoveFullRows(vector, start);

            // For the remaining rows between start and end move to the right empty columns
            end = MoveZeroColumns(vector, start, end);

            // Sort the remaining partially complete rows
            ToBlockTriangular(vector, start, end);
        }

        int MoveZeroRows(Vector vector)
        {
            int nextSwapRow = 0;

            // bubble zero rows to the top - starting from the bottom - to leave any rows already moved
            // by the user stay in place

            int i = vector.Size() - 1;
            while (i >= nextSwapRow)
            {
                var allZero = true;
                for (int j = 0; j < vector.Size() && allZero; j++)
                {
                    if (i != j)  // the diagonal must obviously be ignored
                    {
                        if (TrueMatrixValue(vector, i, j) != 0)
                        {
                            allZero = false;
                        }
                    }
                }

                if (allZero) // swap indexes
                {
                    vector.Swap(nextSwapRow, i);
                    nextSwapRow++; // points to next swap position

                    // don't decrement i as it has not yet been tested - we've chnaged the order remember !!
                }
                else
                {
                    i--; // next row
                }
            }

            return nextSwapRow;
        }

        int MoveFullRows(Vector vector, int start)
        {
            int nextSwapRow = vector.Size() - 1;

            // bubble complete rows to the bottom starting 'start'
            int i = start;

            while (i <= nextSwapRow)
            {
                var allNonZero = true;
                for (int j = 0; j < vector.Size() && allNonZero; j++)
                {
                    if (i != j)
                    {
                        if (TrueMatrixValue(vector, i, j) == 0)
                        {
                            allNonZero = false;
                        }
                    }
                }

                if (allNonZero) // swap indexes
                {
                    vector.Swap(nextSwapRow, i);
                    nextSwapRow--;
                }
                else
                {
                    i++;
                }
            }

            return nextSwapRow;
        }

        int MoveZeroColumns(Vector vector, int start, int end)
        {
            int j = start;
            int nextSwap = end; //vector.Size - 1;

            while (j <= nextSwap)
            {
                var allZeros = true;
                for (int i = 0; i < vector.Size() && allZeros; i++)
                {
                    if (i != j)
                    {
                        if (TrueMatrixValue(vector, i, j) != 0)
                        {
                            allZeros = false;
                        }
                    }
                }

                if (allZeros)  // swap indexes
                {
                    vector.Swap(nextSwap, j);
                    nextSwap--;

                    // stay on new column in position j
                }
                else
                {
                    j++;
                }
            }

            return nextSwap;
        }

        /// <summary>
        /// Get the dependency weight from the original sqaure matrix given the current vector and i,j indexes
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        int TrueMatrixValue(Vector vector, int i, int j)
        {
            return _sm.Get(vector.Get(i), vector.Get(j));
        }

        void ToBlockTriangular(Vector vector, int start, int end)
        {
            bool doLoop;

            long currentScore = Score(vector);

            // For holding Permutations already examined during one iteration of the outer while loop
            IDictionary<Permutation, object> permMap =
                new Dictionary<Permutation, object>(vector.Size() * vector.Size() / 2);
            do
            {
                doLoop = false;

                for (int i = start; i <= end; i++) // i is index on rows
                {
                    for (int j = end; j > i; j--) // cols in upper triangle
                    {
                        if (TrueMatrixValue(vector, i, j) != 0) // not zero so we want to possibley move it
                        {
                            // now find first zero from the left hand side

                            for (int x = start; x <= end; x++) // here x represents the line index
                            {
                                for (int y = start; y <= end; y++)
                                {
                                    if (x != y)  // ignore the diagonal
                                    {
                                        if (TrueMatrixValue(vector, x, y) == 0)
                                        {
                                            Permutation p = new Permutation(j, y);

                                            if (!permMap.ContainsKey(p))
                                            {
                                                permMap.Add(p, null);

                                                //check score of potential new vector
                                                vector.Swap(j, y);

                                                var newScore = Score(vector);

                                                if (newScore > currentScore)
                                                {
                                                    currentScore = newScore;
                                                    doLoop = true; // increasing score so continue
                                                }
                                                else
                                                {
                                                    vector.Swap(j, y); //swap back to original 
                                                }
                                            }
                                            // else permutation already used
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                permMap.Clear();
            }
            while (doLoop);
        }

        long Score(Vector vector)
        {
            // We're trying to maximise the number of empty cells in the upper triangle
            // filled in cells are pushed down - for a set of two orderings the one with the higher
            // score should be the most preferable


            // calculate score for cells in upper triangle (TODO possible optimisation between start and end)
            long score = 0;

            for (int i = 0; i < vector.Size() - 1; i++)
            {
                for (int j = i + 1; j < vector.Size(); j++)
                {
                    if (TrueMatrixValue(vector, i, j) == 0)
                    {
                        score += CellScore(i, j, vector.Size());
                    }
                }
            }

            return score;
        }

        static long CellScore(int i, int j, int size)
        {
            // a measure of distance from bottom right
            int a = (size - i);
            int b = j + 1;

            return (a * a * b * b);
        }
    }
}
