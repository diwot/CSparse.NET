
namespace CSparse.Tests
{
    using CSparse.Storage;
    using System;

    class DenseTestData<T, Scalar>
        where T : struct, IEquatable<T>, IFormattable
    {
        public int RowCount;

        public int ColumnCount;

        public DenseColumnMajorStorage<T, Scalar> A;

        public DenseColumnMajorStorage<T, Scalar> B;

        public T[] x;

        public T[] y;

        public DenseColumnMajorStorage<T, Scalar> AT;

        public DenseColumnMajorStorage<T, Scalar> BT;

        public DenseColumnMajorStorage<T, Scalar> ApB;

        public DenseColumnMajorStorage<T, Scalar> AmBT;

        public DenseColumnMajorStorage<T, Scalar> ATmB;

        public T[] Ax;

        public T[] ATy;

        public T[] xTBT;
    }
}
