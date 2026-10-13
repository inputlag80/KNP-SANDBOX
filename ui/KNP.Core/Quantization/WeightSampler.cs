namespace KNP.Core.Quantization;

public static class WeightSampler
{
    public static double[] Gaussian(int count, double sigma = 0.5, int seed = 13)
    {
        var rng = new Random(seed);
        var result = new double[count];
        for (int i = 0; i < count; i++)
        {
            double u1 = 1.0 - rng.NextDouble();
            double u2 = 1.0 - rng.NextDouble();
            double z = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2 * Math.PI * u2);
            result[i] = z * sigma;
        }
        return result;
    }
}