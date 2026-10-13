namespace KNP.Core.Quantization;

public static class PerplexityModel
{
    public static double Estimate(double basePpl, int bits)
    {
        if (bits >= 32) return basePpl;
        double penalty = 0.04 * Math.Pow(32 - bits, 1.35);
        return basePpl * (1 + penalty);
    }
}