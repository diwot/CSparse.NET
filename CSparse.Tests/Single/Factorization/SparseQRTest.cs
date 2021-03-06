﻿
namespace CSparse.Tests.Single.Factorization
{
    using Real = System.Single;
    using CSparse.Single;
    using CSparse.Single.Factorization;
    using NUnit.Framework;

    public class SparseQRTest
    {
        private const double EPS = (Real)1.0e-4;

        [Test]
        public void TestSolve()
        {
            // Load matrix from a file.
            var A = ResourceLoader.Get<Real, Real>("general-40x40.mat");

            Assert.AreEqual(A.RowCount, A.ColumnCount);

            // Create test data.
            var x = Helper.CreateTestVector(A.ColumnCount);
            var b = Helper.Multiply(A, x);
            var r = Vector.Clone(b);

            var qr = SparseQR.Create(A, ColumnOrdering.MinimumDegreeAtA);

            // Solve Ax = b.
            qr.Solve(b, x);

            // Compute residual r = b - Ax.
            A.Multiply((Real)(-1.0), x, (Real)1.0, r);

            Assert.IsTrue(Vector.Norm(r) < EPS);
        }

        [Test]
        public void TestSolveOverdetermined()
        {
            // Load matrix from a file.
            var A = ResourceLoader.Get<Real, Real>("general-40x20.mat");

            int m = A.RowCount;
            int n = A.ColumnCount;

            Assert.IsTrue(m > n);

            // Create test data.
            var x = Helper.CreateTestVector(n);
            var b = Helper.Multiply(A, x);
            var r = Vector.Clone(b);

            var qr = SparseQR.Create(A, ColumnOrdering.MinimumDegreeAtA);

            // Compute min norm(Ax - b).
            qr.Solve(b, x);

            // Compute residual r = b - Ax.
            A.Multiply((Real)(-1.0), x, (Real)1.0, r);

            Assert.IsTrue(Vector.Norm(r) < EPS);
        }

        [Test]
        public void TestSolveUnderdetermined()
        {
            // Load matrix from a file.
            var A = ResourceLoader.Get<Real, Real>("general-20x40.mat");

            int m = A.RowCount;
            int n = A.ColumnCount;

            Assert.IsTrue(m < n);

            // Create test data.
            var x = Helper.CreateTestVector(n);
            var b = Helper.Multiply(A, x);
            var r = Vector.Clone(b);

            var qr = SparseQR.Create(A, ColumnOrdering.MinimumDegreeAtA);

            // Assuming A has full rank m, we have N(A) = n - m degrees of freedom.
            // Compute solution x with min norm(Ax - b).
            qr.Solve(b, x);

            // Compute residuals.
            A.Multiply((Real)(-1.0), x, (Real)1.0, r);

            Assert.IsTrue(Vector.Norm(r) < EPS);
        }

        [TestCase(0, 0)]
        [TestCase(0, 5)]
        [TestCase(5, 0)]
        public void TestEmptyFactorize(int rows, int columns)
        {
            var A = new SparseMatrix(rows, columns, 0);

            var qr = SparseQR.Create(A, ColumnOrdering.MinimumDegreeAtA);

            Assert.NotNull(qr);
            Assert.IsTrue(qr.NonZerosCount == -rows);
        }
    }
}
