
namespace CSparse.Tests
{
    using CSparse.Storage;
    using System;

    class SparseTestData<T, Scalar>
        where T : struct, IEquatable<T>, IFormattable
    {
        public int RowCount;

        public int ColumnCount;

        public CompressedColumnStorage<T, Scalar> A;

        public CompressedColumnStorage<T, Scalar> B;

        public T[] x;

        public T[] y;

        public CompressedColumnStorage<T, Scalar> AT;

        public CompressedColumnStorage<T, Scalar> BT;

        public CompressedColumnStorage<T, Scalar> ApB;

        public CompressedColumnStorage<T, Scalar> AmBT;

        public CompressedColumnStorage<T, Scalar> ATmB;

        public T[] Ax;

        public T[] ATy;

        public T[] xTBT;
    }
}
