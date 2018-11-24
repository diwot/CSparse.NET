
namespace CSparse.Tests.Single.Factorization
{
    using Real = System.Single;
    using CSparse.Single;
    using CSparse.Single.Factorization;
    using NUnit.Framework;
    using System;

    public class SparseCholeskyTest
    {
        private const Real EPS = (Real)1.0e-4;

        [Test]
        public void TestSolve()
        {
            // Load matrix from a file.
            var A = ResourceLoader.Get<Real, Real>("symmetric-40-spd.mat");

            // Create test data.
            var x = Helper.CreateTestVector(A.ColumnCount);
            var b = Helper.Multiply(A, x);
            var r = Vector.Clone(b);

            var chol = SparseCholesky.Create(A, ColumnOrdering.MinimumDegreeAtPlusA);

            // Solve Ax = b.
            chol.Solve(b, x);

            // Compute residual r = b - Ax.
            A.Multiply((Real)(-1.0), x, (Real)1.0, r);

            Assert.IsTrue(Vector.Norm(r) < EPS);
        }

        [Test]
        public void TestConstructorThrowsOnNonSpd()
        {
            // Load matrix from a file.
            var A = ResourceLoader.Get<Real, Real>("symmetric-40.mat");

            Assert.Throws<Exception>(() =>
            {
                var chol = SparseCholesky.Create(A, ColumnOrdering.MinimumDegreeAtPlusA);
            });
        }

        [Test]
        public void TestEmptyFactorize()
        {
            var A = new SparseMatrix(0, 0, 0);

            var chol = SparseCholesky.Create(A, ColumnOrdering.MinimumDegreeAtPlusA);

            Assert.NotNull(chol);
            Assert.IsTrue(chol.NonZerosCount == 0);
        }
    }
}
