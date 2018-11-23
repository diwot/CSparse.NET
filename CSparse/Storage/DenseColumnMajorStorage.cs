﻿
namespace CSparse.Storage
{
    using CSparse;
    using CSparse.Properties;
    using System;
    using System.Collections.Generic;
    using System.Numerics;

    ///// <summary>
    ///// Dense column-major matrix storage.
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    //public abstract class DenseColumnMajorStorage<T> : DenseColumnMajorStorage<T, double>
    //   where T : struct, IEquatable<T>, IFormattable
    //{
    //    /// <summary>
    //    /// Initializes a new instance of the DenseColumnMajorStorage class.
    //    /// </summary>
    //    public DenseColumnMajorStorage(int rows, int columns) : base(rows, columns)
    //    {
    //    }

    //    /// <summary>
    //    /// Initializes a new instance of the DenseColumnMajorStorage class.
    //    /// </summary>
    //    public DenseColumnMajorStorage(int rows, int columns, T[] values) : base(rows, columns, values)
    //    {
    //    }
    //}

    /// <summary>
    /// Dense column-major matrix storage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class DenseColumnMajorStorage<T, Scalar> : Matrix<T, Scalar>
        where T : struct, IEquatable<T>, IFormattable
    {
        /// <summary>
        /// Gets the numerical values in column-major order.
        /// </summary>
        public T[] Values;

        /// <summary>
        /// Return the matrix value at position (i, j).
        /// </summary>
        /// <param name="i">The row index.</param>
        /// <param name="j">The column index.</param>
        public T this[int i, int j]
        {
            get { return Values[(j * rowCount) + i]; }
            set { Values[(j * rowCount) + i] = value; }
        }

        /// <summary>
        /// Initializes a new instance of the DenseColumnMajorStorage class.
        /// </summary>
        public DenseColumnMajorStorage(int rows, int columns)
            : this(rows, columns, new T[rows * columns])
        {
        }

        /// <summary>
        /// Initializes a new instance of the DenseColumnMajorStorage class.
        /// </summary>
        public DenseColumnMajorStorage(int rows, int columns, T[] values)
            : base(rows, columns)
        {
            this.Values = values;
        }

        /// <inheritdoc />
        public override T At(int row, int column)
        {
            return Values[(column * rowCount) + row];
        }

        /// <summary>
        /// Sets the element without range checking.
        /// </summary>
        public void At(int row, int column, T value)
        {
            Values[(column * rowCount) + row] = value;
        }

        /// <inheritdoc />
        public override T[] Row(int row)
        {
            var target = new T[columnCount];

            Row(row, target);

            return target;
        }

        /// <inheritdoc />
        public override T[] Column(int column)
        {
            var target = new T[rowCount];

            Column(column, target);

            return target;
        }

        /// <inheritdoc />
        public override void Row(int row, T[] target)
        {
            for (int i = 0; i < columnCount; i++)
            {
                target[i] = Values[(i * rowCount) + row];
            }
        }

        /// <inheritdoc />
        public override void Column(int column, T[] target)
        {
            Array.Copy(Values, column * rowCount, target, 0, rowCount);
        }

        /// <summary>
        /// Copy values from array to matrix row.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="values">The new values.</param>
        public void SetRow(int row, T[] values)
        {
            var target = this.Values;

            for (int i = 0; i < columnCount; i++)
            {
                target[(i * rowCount) + row] = values[i];
            }
        }

        /// <summary>
        /// Copy values from array to matrix column.
        /// </summary>
        /// <param name="column">The column index.</param>
        /// <param name="values">The new values.</param>
        public void SetColumn(int column, T[] values)
        {
            var target = this.Values;

            Array.Copy(values, 0, target, column * rowCount, rowCount);
        }

        #region Linear Algebra (Vector)

        #endregion

        #region Linear Algebra (Matrix)

        /// <summary>
        /// Returns the transpose of this matrix.
        /// </summary>
        public virtual DenseColumnMajorStorage<T, Scalar> Transpose()
        {
            var result = DenseColumnMajorStorage<T, Scalar>.Create(columnCount, rowCount);
            this.Transpose(result);
            return result;
        }

        /// <summary>
        /// Transpose this matrix and store the result in given matrix.
        /// </summary>
        public virtual void Transpose(DenseColumnMajorStorage<T, Scalar> result)
        {
            var target = result.Values;

            for (int j = 0; j < ColumnCount; j++)
            {
                var index = j * RowCount;
                for (int i = 0; i < RowCount; i++)
                {
                    target[(i * ColumnCount) + j] = Values[index + i];
                }
            }
        }

        /// <summary>
        /// Adds two matrices in CSC format, C = A + B, where A is current instance.
        /// </summary>
        public DenseColumnMajorStorage<T, Scalar> Add(DenseColumnMajorStorage<T, Scalar> other)
        {
            int m = this.rowCount;
            int n = this.columnCount;

            // check inputs
            if (m != other.RowCount || n != other.ColumnCount)
            {
                throw new ArgumentException(Resources.MatrixDimensions, "other");
            }

            var result = DenseColumnMajorStorage<T, Scalar>.Create(m, n);

            Add(other, result);

            return result;
        }

        /// <summary>
        /// Adds two matrices, C = A + B, where A is current instance.
        /// </summary>
        /// <param name="other">The matrix added to this instance.</param>
        /// <param name="result">Contains the sum.</param>
        public abstract void Add(DenseColumnMajorStorage<T, Scalar> other, DenseColumnMajorStorage<T, Scalar> result);

        /// <summary>
        /// Dense matrix multiplication, C = A*B
        /// </summary>
        /// <param name="other">Dense matrix</param>
        /// <returns>C = A*B, null on error</returns>
        public DenseColumnMajorStorage<T, Scalar> Multiply(DenseColumnMajorStorage<T, Scalar> other)
        {
            // A = (3x4)
            // B = (4x5)
            // C = (3x5)
            int m = this.rowCount;
            int n = other.columnCount;

            // check inputs
            if (this.columnCount != other.RowCount)
            {
                throw new ArgumentException(Resources.MatrixDimensions, "other");
            }

            var result = DenseColumnMajorStorage<T, Scalar>.Create(m, n);

            Multiply(other, result);

            return result;
        }

        /// <summary>
        /// Dense matrix multiplication, C = A*B
        /// </summary>
        /// <param name="other">The matrix multiplied to this instance.</param>
        /// <param name="result">The product matrix.</param>
        public abstract void Multiply(DenseColumnMajorStorage<T, Scalar> other, DenseColumnMajorStorage<T, Scalar> result);

        /// <summary>
        /// Pointwise multiplies this matrix with another matrix and stores the result into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to pointwise multiply with this one.</param>
        /// <param name="result">The matrix to store the result of the pointwise multiplication.</param>
        public abstract void PointwiseMultiply(DenseColumnMajorStorage<T, Scalar> other, DenseColumnMajorStorage<T, Scalar> result);

        #endregion

        /// <summary>
        /// Returns a clone of this matrix.
        /// </summary>
        public abstract DenseColumnMajorStorage<T, Scalar> Clone();

        /// <summary>
        /// Returns a new matrix containing the upper triangle of this matrix.
        /// </summary>
        /// <returns>The upper triangle of this matrix.</returns>
        public virtual DenseColumnMajorStorage<T, Scalar> UpperTriangle()
        {
            var result = Create(RowCount, ColumnCount);

            for (var row = 0; row < RowCount; row++)
            {
                for (var column = row; column < ColumnCount; column++)
                {
                    result.At(row, column, At(row, column));
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a new matrix containing the lower triangle of this matrix.
        /// </summary>
        /// <returns>The lower triangle of this matrix.</returns>
        public virtual DenseColumnMajorStorage<T, Scalar> LowerTriangle()
        {
            var result = Create(RowCount, ColumnCount);

            for (var row = 0; row < RowCount; row++)
            {
                for (var column = 0; column <= row && column < ColumnCount; column++)
                {
                    result.At(row, column, At(row, column));
                }
            }

            return result;
        }

        /// <summary>
        /// Puts the lower triangle of this matrix into the result matrix.
        /// </summary>
        /// <param name="result">Where to store the lower triangle.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="result"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the result matrix's dimensions are not the same as this matrix.</exception>
        public virtual void LowerTriangle(DenseColumnMajorStorage<T, Scalar> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (result.RowCount != RowCount || result.ColumnCount != ColumnCount)
            {
                throw new ArgumentException(Resources.MatrixDimensions, "result");
            }

            for (var row = 0; row < RowCount; row++)
            {
                for (var column = 0; column < ColumnCount; column++)
                {
                    result.At(row, column, row >= column ? At(row, column) : Zero);
                }
            }
        }

        /// <summary>
        /// Puts the upper triangle of this matrix into the result matrix.
        /// </summary>
        /// <param name="result">Where to store the lower triangle.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="result"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the result matrix's dimensions are not the same as this matrix.</exception>
        public virtual void UpperTriangle(DenseColumnMajorStorage<T, Scalar> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (result.RowCount != RowCount || result.ColumnCount != ColumnCount)
            {
                throw new ArgumentException(Resources.MatrixDimensions, "result");
            }

            for (var row = 0; row < RowCount; row++)
            {
                for (var column = 0; column < ColumnCount; column++)
                {
                    result.At(row, column, row <= column ? At(row, column) : Zero);
                }
            }
        }

        /// <summary>
        /// Returns a sub-matrix with values in given range.
        /// </summary>
        /// <param name="rowIndex">The row to start copying to.</param>
        /// <param name="rowCount">The number of rows to copy. Must be positive.</param>
        /// <param name="columnIndex">The column to start copying to.</param>
        /// <param name="columnCount">The number of columns to copy. Must be positive.</param>
        public DenseColumnMajorStorage<T, Scalar> SubMatrix(int rowIndex, int rowCount, int columnIndex, int columnCount)
        {
            var result = DenseColumnMajorStorage<T, Scalar>.Create(rowCount, columnCount);

            CopySubMatrixTo(result, rowIndex, 0, rowCount, columnIndex, 0, columnCount);

            return result;
        }

        /// <summary>
        /// Copies the values of a given matrix into a region in this matrix.
        /// </summary>
        /// <param name="rowIndex">The row to start copying to.</param>
        /// <param name="columnIndex">The column to start copying to.</param>
        /// <param name="subMatrix">The sub-matrix to copy from.</param>
        public void SetSubMatrix(int rowIndex, int columnIndex, DenseColumnMajorStorage<T, Scalar> subMatrix)
        {
            subMatrix.CopySubMatrixTo(this, 0, rowIndex, subMatrix.RowCount, 0, columnIndex, subMatrix.ColumnCount);
        }

        /// <summary>
        /// Copies the values of a given matrix into a region in this matrix.
        /// </summary>
        /// <param name="rowIndex">The row to start copying to.</param>
        /// <param name="rowCount">The number of rows to copy. Must be positive.</param>
        /// <param name="columnIndex">The column to start copying to.</param>
        /// <param name="columnCount">The number of columns to copy. Must be positive.</param>
        /// <param name="subMatrix">The sub-matrix to copy from.</param>
        public void SetSubMatrix(int rowIndex, int rowCount, int columnIndex, int columnCount, DenseColumnMajorStorage<T, Scalar> subMatrix)
        {
            subMatrix.CopySubMatrixTo(this, 0, rowIndex, rowCount, 0, columnIndex, columnCount);
        }

        /// <inheritdoc />
        public override void Clear()
        {
            Array.Clear(Values, 0, rowCount * columnCount);
        }

        /// <inheritdoc />
        public override IEnumerable<Tuple<int, int, T>> EnumerateIndexed()
        {
            for (int row = 0; row < rowCount; row++)
            {
                for (int column = 0; column < columnCount; column++)
                {
                    yield return new Tuple<int, int, T>(row, column, Values[(column * rowCount) + row]);
                }
            }
        }

        private void CopySubMatrixTo(DenseColumnMajorStorage<T, Scalar> target,
            int sourceRowIndex, int targetRowIndex, int rowCount,
            int sourceColumnIndex, int targetColumnIndex, int columnCount)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (rowCount == 0 || columnCount == 0)
            {
                return;
            }

            if (ReferenceEquals(this, target))
            {
                throw new NotSupportedException();
            }

            // TODO: Validate sub-matrix range.

            for (int j = sourceColumnIndex, jj = targetColumnIndex; j < sourceColumnIndex + columnCount; j++, jj++)
            {
                Array.Copy(Values, j * RowCount + sourceRowIndex, target.Values, jj * target.RowCount + targetRowIndex, rowCount);
            }
        }

        #region Internal methods

        internal static DenseColumnMajorStorage<T, Scalar> Create(int rowCount, int columnCount)
        {
            if (typeof(T) == typeof(double))
            {
                return new CSparse.Double.DenseMatrix(rowCount, columnCount)
                    as DenseColumnMajorStorage<T, Scalar>;
            }

            if (typeof(T) == typeof(Complex))
            {
                return new CSparse.Complex.DenseMatrix(rowCount, columnCount)
                    as DenseColumnMajorStorage<T, Scalar>;
            }

            if (typeof(T) == typeof(float))
            {
                return new CSparse.Single.DenseMatrix(rowCount, columnCount)
                    as DenseColumnMajorStorage<T, Scalar>;
            }

            throw new NotSupportedException();
        }

        #endregion
    }
}
