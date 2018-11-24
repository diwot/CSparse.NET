
namespace CSparse.Tests.Single
{
    using Real = System.Single;
    using CSparse.Single;
    using NUnit.Framework;
    using System;

    public class DenseMatrixTest
    {
        [OneTimeSetUp]
        public void Initialize()
        {
            GlobalSettings.DefaultFloatingPointTolerance = 1e-6;
        }

        [TestCase(2, 2)]
        [TestCase(2, 3)]
        public void TestGetRow(int rows, int columns)
        {
            var data = MatrixHelper.LoadDense(rows, columns);

            var A = data.A;

            for (int i = 0; i < rows; i++)
            {
                var y = A.Row(i);

                for (int j = 0; j < columns; j++)
                {
                    Assert.AreEqual(A.At(i, j), y[j]);
                }
            }
        }

        [TestCase(2, 2)]
        [TestCase(2, 3)]
        public void TestGetColumn(int rows, int columns)
        {
            var data = MatrixHelper.LoadDense(rows, columns);

            var A = data.A;

            for (int j = 0; j < columns; j++)
            {
                var y = A.Column(j);

                for (int i = 0; i < rows; i++)
                {
                    Assert.AreEqual(A.At(i, j), y[i]);
                }
            }
        }

        [TestCase(2, 2)]
        [TestCase(2, 3)]
        public void TestSetRow(int rows, int columns)
        {
            var data = MatrixHelper.LoadDense(rows, columns);

            var A = (DenseMatrix)data.A.Clone();

            var z = Vector.Create(columns, (Real)(-1.1));

            for (int i = 0; i < rows; i++)
            {
                A.SetRow(i, z);

                for (int j = 0; j < columns; j++)
                {
                    Assert.AreEqual(-1.1, A.At(i, j));
                }
            }
        }

        [TestCase(2, 2)]
        [TestCase(2, 3)]
        public void TestSetColumn(int rows, int columns)
        {
            var data = MatrixHelper.LoadDense(rows, columns);

            var A = (DenseMatrix)data.A.Clone();

            var z = Vector.Create(rows, (Real)(-1.1));

            for (int j = 0; j < columns; j++)
            {
                A.SetColumn(j, z);

                for (int i = 0; i < rows; i++)
                {
                    Assert.AreEqual(-1.1, A.At(i, j));
                }
            }
        }

        [TestCase(2, 2)]
        [TestCase(2, 3)]
        public void TestMatrixVectorMultiply(int rows, int columns)
        {
            var data = MatrixHelper.LoadDense(rows, columns);

            var A = data.A;
            var x = data.x;

            var actual = Vector.Create(A.RowCount, (Real)0.0);

            A.Multiply(x, actual);

            CollectionAssert.AreEqual(data.Ax, actual);
        }

        [TestCase(2, 2)]
        [TestCase(2, 3)]
        public void TestMatrixVectorTransposeMultiply(int rows, int columns)
        {
            var data = MatrixHelper.LoadDense(rows, columns);

            var A = data.A;
            var y = data.y;

            var actual = Vector.Create(A.ColumnCount, (Real)0.0);

            A.TransposeMultiply(y, actual);

            CollectionAssert.AreEqual(data.ATy, actual);

            var AT = data.AT;

            AT.Multiply(y, actual);

            CollectionAssert.AreEqual(data.ATy, actual);
        }

        [TestCase(2, 2)]
        [TestCase(2, 3)]
        public void TestMatrixTranspose(int rows, int columns)
        {
            var data = MatrixHelper.LoadDense(rows, columns);

            var A = data.A;
            var B = data.B;

            var actualA = A.Transpose();
            var actualB = B.Transpose();

            CollectionAssert.AreEqual(data.AT.Values, actualA.Values);
            CollectionAssert.AreEqual(data.BT.Values, actualB.Values);
        }

        [TestCase(2, 2)]
        [TestCase(2, 3)]
        public void TestMatrixSum(int rows, int columns)
        {
            var data = MatrixHelper.LoadDense(rows, columns);

            var A = data.A;
            var B = data.B;

            var actual = A.Add(B);

            CollectionAssert.AreEqual(data.ApB.Values, actual.Values);
        }

        [TestCase(2, 2)]
        [TestCase(2, 3)]
        public void TestMatrixMultiply(int rows, int columns)
        {
            var data = MatrixHelper.LoadDense(rows, columns);

            var A = data.A;
            var B = data.B;

            var AT = data.AT;
            var BT = data.BT;

            var actual = AT.Multiply(B);

            CollectionAssert.AreEqual(data.ATmB.Values, actual.Values);

            actual = A.Multiply(BT);

            CollectionAssert.AreEqual(data.AmBT.Values, actual.Values);
        }
    }
}
