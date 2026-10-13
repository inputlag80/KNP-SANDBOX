namespace KNP.Core.Distillation;

public static class Divergence
{
    public static double Kl(double[] p, double[] q)
    {
        if (p.Length != q.Length) throw new ArgumentException("Размерности не совпадают");
        double sum = 0;
        for (int i = 0; i < p.Length; i++)
        {
            if (p[i] <= 0) continue;
            sum += p[i] * Math.Log(p[i] / Math.Max(q[i], 1e-9));
        }
        return sum;
    }
}