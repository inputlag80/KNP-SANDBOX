namespace KNP.Core.Quantization;

public static class Quantizer
{
    public static double[] Quantize(double[] weights, int bits)
    {
        if (bits < 1 || bits > 32) throw new ArgumentOutOfRangeException(nameof(bits));
        int levels = 1 << bits;
        double min = weights.Min();
        double max = weights.Max();
        double step = (max - min) / (levels - 1);
        if (step == 0) return weights.ToArray();

        var result = new double[weights.Length];
        for (int i = 0; i < weights.Length; i++)
        {
            double q = Math.Round((weights[i] - min) / step);
            result[i] = q * step + min;
        }
        return result;
    }

    public static int SizeMb(long parameters, int bits) =>
        (int)Math.Round(parameters * bits / 8.0 / 1_000_000);
}