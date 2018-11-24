
namespace CSparse.Tests.Single
{
    using Real = System.Single;
    using CSparse.Storage;
    using System.Collections.Generic;

    class MatrixHelper
    {
        private static Dictionary<string, DenseTestData<Real, Real>> dense = new Dictionary<string, DenseTestData<Real, Real>>();

        private static Dictionary<string, SparseTestData<Real, Real>> sparse = new Dictionary<string, SparseTestData<Real, Real>>();

        public static SparseTestData<Real, Real> LoadSparse(int rows, int columns)
        {
            string resource = string.Format("test-data-dense-{0}x{1}.txt", rows, columns);

            SparseTestData<Real, Real> data;

            if (!sparse.TryGetValue(resource, out data))
            {
                var dense = LoadDense(rows, columns);

                data = DenseToSparse(dense);

                sparse.Add(resource, data);
            }

            return data;
        }

        public static DenseTestData<Real, Real> LoadDense(int rows, int columns)
        {
            string resource = string.Format("test-data-dense-{0}x{1}.txt", rows, columns);

            DenseTestData<Real, Real> data;

            if (!dense.TryGetValue(resource, out data))
            {
                var stream = ResourceLoader.GetStream(resource, "Double");

                data = DenseTestDataReader.Read(stream);

                dense.Add(resource, data);
            }

            return data;
        }

        private static SparseTestData<Real, Real> DenseToSparse(DenseTestData<Real, Real> dense)
        {
            var data = new SparseTestData<Real, Real>()
            {
                RowCount = dense.RowCount,
                ColumnCount = dense.ColumnCount
            };

            data.A = DenseToSparse(dense.A);
            data.B = DenseToSparse(dense.B);
            data.x = dense.x;
            data.y = dense.y;
            data.AT = DenseToSparse(dense.AT);
            data.BT = DenseToSparse(dense.BT);
            data.ApB = DenseToSparse(dense.ApB);
            data.AmBT = DenseToSparse(dense.AmBT);
            data.ATmB = DenseToSparse(dense.ATmB);
            data.Ax = dense.Ax;
            data.ATy = dense.ATy;
            data.xTBT = dense.xTBT;

            return data;
        }

        private static CompressedColumnStorage<Real, Real> DenseToSparse(DenseColumnMajorStorage<Real, Real> dense)
        {
            var cs = Converter.FromColumnMajorArray(dense.Values, dense.RowCount, dense.ColumnCount);

            return Converter.ToCompressedColumnStorage<Real, Real>(cs);
        }
    }
}
