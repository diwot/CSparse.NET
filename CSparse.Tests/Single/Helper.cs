
namespace CSparse.Tests.Single
{
    static class Helper
    {
        public static float[] CreateTestVector(int n)
        {
            var x = new float[n];

            for (int i = 0; i < n; i++)
            {
                x[i] = 1.0f + i / (n - 1.0f);
            }

            return x;
        }

        public static float[] Multiply(ILinearOperator<float> A, float[] x)
        {
            var b = new float[A.RowCount];

            A.Multiply(1.0f, x, 0.0f, b);

            return b;
        }
    }
}
