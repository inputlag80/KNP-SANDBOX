using KNP.Core.Storage;

namespace KNP.Server.Services;

public sealed class KnpSession
{
    private readonly StorageService _storage;

    public string Mode { get; set; } = "distill";
    public DistillSettings Distill { get; set; }
    public QuantSettings Quant { get; set; }

    public string Theme { get; set; } = "dark";
    public string Accent { get; set; } = "purple";
    public bool DevMode { get; set; }

    public event Action? OnChanged;

    public KnpSession(StorageService storage)
    {
        _storage = storage;
        var data = storage.LoadSession();
        Mode = data.Mode;
        Distill = data.Distill;
        Quant = data.Quant;
        Theme = data.Theme;
        Accent = data.Accent;
        DevMode = data.DevMode;
    }

    public void Persist()
    {
        _storage.SaveSession(new SessionData
        {
            Mode = Mode,
            Distill = Distill,
            Quant = Quant,
            Theme = Theme,
            Accent = Accent,
            DevMode = DevMode,
        });
        OnChanged?.Invoke();
    }

    public void NotifyChanged() => OnChanged?.Invoke();

    public void Reset()
    {
        Mode = "distill";
        Distill = new DistillSettings();
        Quant = new QuantSettings();
        Persist();
    }
}