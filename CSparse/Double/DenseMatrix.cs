
namespace CSparse.Double
{
    using Real = System.Double;
    using CSparse.Properties;
    using CSparse.Storage;
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Dense matrix stored in column major order.
    /// </summary>
    [DebuggerDisplay("DenseMatrix {RowCount}x{ColumnCount}")]
    [Serializable]
    public class DenseMatrix : DenseColumnMajorStorage<Real, Real>
    {
        /// <summary>
        /// Initializes a new instance of the DenseMatrix class.
        /// </summary>
        public DenseMatrix(int rows, int columns)
            : this(rows, columns, new Real[rows * columns])
        {
        }

        /// <summary>
        /// Initializes a new instance of the DenseMatrix class.
        /// </summary>
        public DenseMatrix(int rows, int columns, Real[] values)
            : base(rows, columns, values)
        {
        }

        /// <inheritdoc />
        public override Real L1Norm()
        {
            Real sum, norm = (Real)0.0;

            for (var j = 0; j < columnCount; j++)
            {
                sum = (Real)0.0;
                for (var i = 0; i < rowCount; i++)
                {
                    sum += Math.Abs(Values[(j * rowCount) + i]);
                }
                norm = Math.Max(norm, sum);
            }

            return norm;
        }

        /// <inheritdoc />
        public override Real InfinityNorm()
        {
            var r = new Real[rowCount];

            for (var j = 0; j < columnCount; j++)
            {
                for (var i = 0; i < rowCount; i++)
                {
                    r[i] += Math.Abs(Values[(j * rowCount) + i]);
                }
            }

            Real norm = r[0];

            for (int i = 1; i < rowCount; i++)
            {
                if (r[i] > norm)
                {
                    norm = r[i];
                }
            }

            return norm;
        }

        /// <inheritdoc />
        public override Real FrobeniusNorm()
        {
            Real sum = (Real)0.0, norm = (Real)0.0;

            int length = rowCount * columnCount;

            for (int i = 0; i < length; i++)
            {
                sum = Math.Abs(Values[i]);
                norm += sum * sum;
            }

            return (Real)Math.Sqrt(norm);
        }

        /// <inheritdoc />
        public override DenseColumnMajorStorage<Real, Real> Clone()
        {
            var values = (Real[])this.Values.Clone();

            return new DenseMatrix(rowCount, columnCount, values);
        }

        /// <inheritdoc />
        public override void Multiply(Real[] x, Real[] y)
        {
            var A = Values;

            int rows = rowCount;
            int cols = columnCount;

            for (int i = 0; i < rows; i++)
            {
                Real sum = (Real)0.0;

                for (int j = 0; j < cols; j++)
                {
                    sum += A[(j * rows) + i] * x[j];
                }

                y[i] = sum;
            }
        }

        /// <inheritdoc />
        public override void Multiply(Real alpha, Real[] x, Real beta, Real[] y)
        {
            var A = Values;

            int rows = rowCount;
            int cols = columnCount;

            for (int i = 0; i < rows; i++)
            {
                Real sum = (Real)0.0;

                for (int j = 0; j < cols; j++)
                {
                    sum += A[(j * rows) + i] * x[j];
                }

                y[i] = beta * y[i] + alpha * sum;
            }
        }

        /// <inheritdoc />
        public override void TransposeMultiply(Real[] x, Real[] y)
        {
            var A = Values;

            int rows = rowCount;
            int cols = columnCount;

            for (int j = 0; j < cols; j++)
            {
                int col = j * rows;

                Real sum = (Real)0.0;

                for (int i = 0; i < rows; i++)
                {
                    sum += A[col + i] * x[i];
                }

                y[j] = sum;
            }
        }

        /// <inheritdoc />
        public override void TransposeMultiply(Real alpha, Real[] x, Real beta, Real[] y)
        {
            var A = Values;

            int rows = rowCount;
            int cols = columnCount;

            for (int j = 0; j < cols; j++)
            {
                y[j] = beta * y[j];
            }

            for (int j = 0; j < cols; j++)
            {
                int col = j * rows;

                Real sum = (Real)0.0;

                for (int i = 0; i < rows; i++)
                {
                    sum += A[col + i] * x[i];
                }

                y[j] = beta * y[j] + alpha * sum;
            }
        }

        /// <inheritdoc />
        public override void Add(DenseColumnMajorStorage<Real, Real> other, DenseColumnMajorStorage<Real, Real> result)
        {
            int rows = this.rowCount;
            int columns = this.columnCount;

            // check inputs
            if (rows != other.RowCount || columns != other.ColumnCount)
            {
                throw new ArgumentException();
            }

            var target = result.Values;

            var a = this.Values;
            var b = other.Values;

            int length = rows * columns;

            for (int i = 0; i < length; i++)
            {
                target[i] = a[i] + b[i];
            }
        }

        /// <inheritdoc />
        public override void Multiply(DenseColumnMajorStorage<Real, Real> other, DenseColumnMajorStorage<Real, Real> result)
        {
            var A = Values;
            var B = other.Values;
            var C = result.Values;

            int m = rowCount; // rows of matrix A
            int n = other.ColumnCount;
            int o = columnCount;

            const Real zero = (Real)0.0;

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Real sum = zero;

                    for (int k = 0; k < o; ++k)
                    {
                        sum += A[(k * m) + i] * B[(j * o) + k];
                    }

                    C[(j * m) + i] += sum;
                }
            }
        }

        /// <inheritdoc />
        public override void PointwiseMultiply(DenseColumnMajorStorage<Real, Real> other, DenseColumnMajorStorage<Real, Real> result)
        {
            if (RowCount != other.RowCount || ColumnCount != other.ColumnCount)
            {
                throw new ArgumentException(Resources.MatrixDimensions);
            }

            if (RowCount != result.RowCount || ColumnCount != result.ColumnCount)
            {
                throw new ArgumentException(Resources.MatrixDimensions);
            }

            var x = Values;
            var y = other.Values;

            var target = result.Values;

            int length = target.Length;

            for (int i = 0; i < length; i++)
            {
                target[i] = x[i] * y[i];
            }
        }

        /// <inheritdoc />
        public override bool Equals(Matrix<Real, Real> other, Real tolerance)
        {
            if (rowCount != other.RowCount || columnCount != other.ColumnCount)
            {
                return false;
            }

            var dense = other as DenseColumnMajorStorage<Real, Real>;

            if (dense == null)
            {
                return false;
            }

            int length = rowCount * columnCount;

            var otherValues = dense.Values;

            for (int i = 0; i < length; i++)
            {
                if (Math.Abs(Values[i] - otherValues[i]) > tolerance)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
