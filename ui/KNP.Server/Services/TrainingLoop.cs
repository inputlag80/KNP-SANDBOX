using KNP.Core.Distillation;
using KNP.Core.Quantization;

namespace KNP.Server.Services;

public sealed class TrainingLoop : IDisposable
{
    private readonly KnpSession _session;
    private readonly object _gate = new();

    private readonly string _logDir = Path.Combine(
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..")),
        "data", "logs");

    private Timer? _timer;
    private Distiller? _distiller;
    private bool _quantInitialized;
    private int _targetEpochs = 25;

    public bool IsRunning { get; private set; }
    public Distiller? Distiller => _distiller;
    public StepResult? LastStep { get; private set; }

    public double[]? OriginalWeights { get; private set; }
    public double[]? QuantizedWeights { get; private set; }

    public event Action? OnTick;
    public event Action<string>? OnLog;

    public event Action? OnFinished;

    public List<string> Log { get; } = new();

    public bool QuantInitialized
    {
        get { lock (_gate) return _quantInitialized; }
    }

    public int TargetEpochs
    {
        get { lock (_gate) return _targetEpochs; }
    }

    public TrainingLoop(KnpSession session)
    {
        _session = session;
    }

    public void Start()
    {
        lock (_gate)
        {
            if (IsRunning) return;
            if (_session.Mode == "distill")
            {
                _distiller ??= new Distiller();
                _targetEpochs = _session.Distill.Epochs;
            }
            IsRunning = true;
        }
        PushLog($"[{_session.Mode}] старт");
        _timer = new Timer(_ => Tick(), null, 0, 200);
        OnTick?.Invoke();
    }

    public void Pause()
    {
        lock (_gate)
        {
            if (!IsRunning) return;
            IsRunning = false;
        }
        _timer?.Dispose();
        _timer = null;
        PushLog("пауза");
        OnTick?.Invoke();
    }

    public void Reset()
    {
        _timer?.Dispose();
        _timer = null;
        lock (_gate)
        {
            IsRunning = false;
            _distiller = new Distiller();
            LastStep = null;
            OriginalWeights = null;
            QuantizedWeights = null;
            _quantInitialized = false;
            _targetEpochs = _session.Distill.Epochs;
        }
        PushLog("сброс");
        OnTick?.Invoke();
    }

    public void RecomputeQuant()
    {
        lock (_gate)
        {
            OriginalWeights ??= WeightSampler.Gaussian(2000);
            QuantizedWeights = Quantizer.Quantize(OriginalWeights, _session.Quant.Bits);
            _quantInitialized = true;
        }
        PushLog($"quantize {_session.Quant.Model} → {_session.Quant.Bits} bit");
        OnTick?.Invoke();
    }

    private void Tick()
    {
        if (_session.Mode != "distill") return;

        StepResult? result = null;
        bool finished = false;

        lock (_gate)
        {
            if (_distiller is null) return;
            var s = _session.Distill;
            result = _distiller.Step(s.Temperature, s.Alpha, s.Lr);
            LastStep = result;
            if (result.Epoch >= _targetEpochs) finished = true;
        }

        if (finished)
        {
            PushLog($"эпоха {result!.Epoch} — обучение завершено");
            Pause();
            OnFinished?.Invoke();
            return;
        }

        OnTick?.Invoke();
    }

    private void PushLog(string msg)
    {
        var line = $"[{DateTime.Now:HH:mm:ss}] {msg}";

        lock (_gate)
        {
            if (Log.Count > 0 && Log[0].EndsWith(msg)) return;
            Log.Insert(0, line);
            if (Log.Count > 50) Log.RemoveAt(Log.Count - 1);
        }

        try
        {
            Directory.CreateDirectory(_logDir);
            var path = Path.Combine(_logDir, $"{DateTime.Now:yyyy-MM-dd}.log");
            File.AppendAllText(path, line + Environment.NewLine);
        }
        catch { }

        OnLog?.Invoke(line);
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}