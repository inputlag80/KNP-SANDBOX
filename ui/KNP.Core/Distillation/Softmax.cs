namespace KNP.Core.Distillation;

public static class Softmax
{
    public static double[] Compute(double[] logits, double temperature = 1.0)
    {
        if (temperature <= 0) throw new ArgumentOutOfRangeException(nameof(temperature));
        double max = logits.Max();
        var exps = new double[logits.Length];
        double sum = 0;
        for (int i = 0; i < logits.Length; i++)
        {
            exps[i] = Math.Exp((logits[i] - max) / temperature);
            sum += exps[i];
        }
        for (int i = 0; i < logits.Length; i++) exps[i] /= sum;
        return exps;
    }
}