using DsmSuite.DsmViewer.Application.Sorting;

namespace DsmSuite.DsmViewer.Application.Test.Algorithm
{
    [TestClass]
    public class WeightsMatrixTest
    {
        [TestMethod]
        public void WhenConstructedThenSizeCanBeReRetrieved()
        {
            int size = 5;
            WeightsMatrix matrix = new WeightsMatrix(size);
            Assert.AreEqual(size, matrix.Size);
        }

        [TestMethod]
        public void WhenConstructedThenAllWeightsAreZero()
        {
            int size = 5;
            WeightsMatrix matrix = new WeightsMatrix(size);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Assert.AreEqual(0, matrix.GetWeight(i, j));
                }
            }
        }

        [TestMethod]
        public void WhenWeightIsSetThenRetrievedWeightMatchesSetValue()
        {
            int size = 5;
            WeightsMatrix matrix = new WeightsMatrix(size);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int weight = CalculateWeight(i, j);
                    matrix.SetWeight(i, j, weight);
                    Assert.AreEqual(weight, matrix.GetWeight(i, j));
                }
            }
        }

        [TestMethod]
        public void WhenMatrixClonedThenValuesIdentical()
        {
            int size = 5;
            WeightsMatrix matrix = new WeightsMatrix(size);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int weight = CalculateWeight(i, j);
                    matrix.SetWeight(i, j, weight);
                }
            }

            WeightsMatrix clonedMatrix = matrix.Clone() as WeightsMatrix;
            Assert.IsNotNull(clonedMatrix);

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int weight = CalculateWeight(i, j);
                    Assert.AreEqual(weight, matrix.GetWeight(i, j));
                }
            }
        }

        private int CalculateWeight(int i, int j)
        {
            return i * 10 + j;
        }
    }
}
