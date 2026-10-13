namespace KNP.Core.Distillation;

public sealed class Distiller
{
    private readonly Random _rng;

    public double[] TeacherLogits { get; }
    public double[] StudentLogits { get; private set; }
    public int Epoch { get; private set; }

    public List<double> TotalLosses { get; } = new();
    public List<double> SoftLosses { get; } = new();
    public List<double> HardLosses { get; } = new();
    public List<double> Accuracies { get; } = new();

    public Distiller(int vocabSize = 10, int seed = 42)
    {
        _rng = new Random(seed);
        TeacherLogits = Enumerable.Range(0, vocabSize)
            .Select(_ => _rng.NextDouble() * 6 - 3).ToArray();
        StudentLogits = Enumerable.Range(0, vocabSize)
            .Select(_ => _rng.NextDouble() * 6 - 3).ToArray();
    }

    public StepResult Step(double temperature, double alpha, double lr)
    {
        var teacherHard = Softmax.Compute(TeacherLogits, 1.0);
        var teacherSoft = Softmax.Compute(TeacherLogits, temperature);
        var studentHard = Softmax.Compute(StudentLogits, 1.0);
        var studentSoft = Softmax.Compute(StudentLogits, temperature);

        double softLoss = Divergence.Kl(teacherSoft, studentSoft) * temperature * temperature;
        double hardLoss = Divergence.Kl(teacherHard, studentHard);
        double total = (1 - alpha) * hardLoss + alpha * softLoss;

        int tTop = Array.IndexOf(teacherHard, teacherHard.Max());
        int sTop = Array.IndexOf(studentHard, studentHard.Max());
        double acc = tTop == sTop ? 1.0 : 0.0;

        double effectiveLr = lr * 0.06;
        for (int i = 0; i < StudentLogits.Length; i++)
        {
            double noise = (_rng.NextDouble() - 0.5) * 0.1;
            StudentLogits[i] += effectiveLr * (TeacherLogits[i] - StudentLogits[i]) + noise;
        }

        Epoch++;
        TotalLosses.Add(total);
        SoftLosses.Add(softLoss);
        HardLosses.Add(hardLoss);
        Accuracies.Add(acc);

        return new StepResult(total, softLoss, hardLoss, acc,
            teacherSoft, studentSoft, teacherHard, studentHard, Epoch);
    }

    public void Reset()
    {
        Epoch = 0;
        TotalLosses.Clear();
        SoftLosses.Clear();
        HardLosses.Clear();
        Accuracies.Clear();
        for (int i = 0; i < StudentLogits.Length; i++)
            StudentLogits[i] = _rng.NextDouble() * 6 - 3;
    }
}

public record StepResult(
    double TotalLoss, double SoftLoss, double HardLoss, double Accuracy,
    double[] TeacherSoft, double[] StudentSoft,
    double[] TeacherHard, double[] StudentHard,
    int Epoch);