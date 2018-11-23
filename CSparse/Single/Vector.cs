// -----------------------------------------------------------------------
// <copyright file="Vector.cs">
// Copyright (c) 2012-2016, Christian Woltering
// </copyright>
// -----------------------------------------------------------------------

namespace CSparse.Single
{
    using Real = System.Single;
    using System;

    /// <summary>
    /// Vector helper methods.
    /// </summary>
    public static class Vector
    {
        /// <summary>
        /// Copy one vector to another.
        /// </summary>
        /// <param name="src">The source array.</param>
        /// <param name="dst">The destination array.</param>
        public static void Copy(Real[] src, Real[] dst)
        {
            Buffer.BlockCopy(src, 0, dst, 0, src.Length * sizeof(Real));
        }

        /// <summary>
        /// Copy one vector to another.
        /// </summary>
        /// <param name="src">The source array.</param>
        /// <param name="dst">The destination array.</param>
        /// <param name="n">Number of values to copy.</param>
        public static void Copy(Real[] src, Real[] dst, int n)
        {
            Buffer.BlockCopy(src, 0, dst, 0, n * sizeof(Real));
        }

        /// <summary>
        /// Create a new vector.
        /// </summary>
        public static Real[] Create(int length, Real value)
        {
            Real[] result = new Real[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = value;
            }

            return result;
        }

        /// <summary>
        /// Clone the given vector.
        /// </summary>
        public static Real[] Clone(Real[] src)
        {
            Real[] result = new Real[src.Length];

            Buffer.BlockCopy(src, 0, result, 0, src.Length * sizeof(Real));

            return result;
        }

        /// <summary>
        /// Set vector values to zero.
        /// </summary>
        public static void Clear(Real[] x)
        {
            Array.Clear(x, 0, x.Length);
        }

        /// <summary>
        /// Computes the dot product of two vectors.
        /// </summary>
        public static Real DotProduct(Real[] x, Real[] y)
        {
            int length = x.Length;

            Real result = (Real)0.0;

            for (int i = 0; i < length; i++)
            {
                result += x[i] * y[i];
            }

            return result;
        }

        /// <summary>
        /// Computes the pointwise product of two vectors.
        /// </summary>
        public static void PointwiseMultiply(Real[] x, Real[] y, Real[] z)
        {
            int length = x.Length;

            for (int i = 0; i < length; i++)
            {
                z[i] = x[i] * y[i];
            }
        }

        /// <summary>
        /// Computes the norm of a vector, sqrt( x' * x ).
        /// </summary>
        public static Real Norm(Real[] x)
        {
            int length = x.Length;

            Real result = (Real)0.0;

            for (int i = 0; i < length; ++i)
            {
                result += x[i] * x[i];
            }

            return (Real)Math.Sqrt(result);
        }

        /// <summary>
        /// Computes the norm of a vector avoiding overflow, sqrt( x' * x ).
        /// </summary>
        public static Real NormRobust(Real[] x)
        {
            int length = x.Length;

            Real scale = (Real)0.0, ssq = (Real)1.0;
            const Real one = (Real)1.0;

            for (int i = 0; i < length; ++i)
            {
                if (x[i] != 0.0)
                {
                    Real absxi = Math.Abs(x[i]);
                    if (scale < absxi)
                    {
                        ssq = one + ssq * (scale / absxi) * (scale / absxi);
                        scale = absxi;
                    }
                    else
                    {
                        ssq += (absxi / scale) * (absxi / scale);
                    }
                }
            }

            return scale * (Real)Math.Sqrt(ssq);
        }

        /// <summary>
        /// Scales a vector by a given factor, x = alpha * x.
        /// </summary>
        public static void Scale(Real alpha, Real[] x)
        {
            int length = x.Length;

            for (int i = 0; i < length; i++)
            {
                x[i] *= alpha;
            }
        }

        /// <summary>
        /// Add a scaled vector to another vector, y = y + alpha * x.
        /// </summary>
        public static void Axpy(Real alpha, Real[] x, Real[] y)
        {
            int length = x.Length;

            for (int i = 0; i < length; i++)
            {
                y[i] += alpha * x[i];
            }
        }

        /// <summary>
        /// Add two scaled vectors, z = alpha * x + beta * y.
        /// </summary>
        public static void Add(Real alpha, Real[] x, Real beta, Real[] y, Real[] z)
        {
            int length = x.Length;

            for (int i = 0; i < length; i++)
            {
                z[i] = alpha * x[i] + beta * y[i];
            }
        }
    }
}
