﻿
namespace CSparse.Tests.Double.Factorization
{
    using Real = System.Double;
    using CSparse.Double;
    using CSparse.Double.Factorization;
    using NUnit.Framework;

    public class SparseLUTest
    {
        private const Real EPS = (Real)1.0e-6;

        [Test]
        public void TestSolve()
        {
            // Load matrix from a file.
            var A = ResourceLoader.Get<Real, Real>("general-40x40.mat");

            // Create test data.
            var x = Helper.CreateTestVector(A.ColumnCount);
            var b = Helper.Multiply(A, x);
            var r = Vector.Clone(b);

            // Create LU factorization.
            var lu = SparseLU.Create(A, ColumnOrdering.MinimumDegreeAtPlusA, (Real)1.0);

            // Solve Ax = b.
            lu.Solve(b, x);

            // Compute residual r = b - Ax.
            A.Multiply((Real)(-1.0), x, (Real)1.0, r);

            Assert.IsTrue(Vector.Norm(r) < EPS);
        }

        [Test]
        public void TestSolveTranspose()
        {
            // Load matrix from a file.
            var A = ResourceLoader.Get<Real, Real>("general-40x40.mat");

            var AT = A.Transpose();

            // Create test data.
            var x = Helper.CreateTestVector(A.ColumnCount);
            var b = Helper.Multiply(AT, x);
            var r = Vector.Clone(b);

            // Create LU factorization.
            var lu = SparseLU.Create(A, ColumnOrdering.MinimumDegreeAtPlusA, (Real)1.0);

            // Solve A'x = b.
            lu.SolveTranspose(b, x);

            // Compute residual r = b - A'x.
            AT.Multiply((Real)(-1.0), x, (Real)1.0, r);

            Assert.IsTrue(Vector.Norm(r) < EPS);
        }

        [Test]
        public void TestEmptyFactorize()
        {
            var A = new SparseMatrix(0, 0, 0);

            var lu = SparseLU.Create(A, ColumnOrdering.MinimumDegreeAtPlusA, (Real)1.0);

            Assert.NotNull(lu);
            Assert.IsTrue(lu.NonZerosCount == 0);
        }
    }
}
