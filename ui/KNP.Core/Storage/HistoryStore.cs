using System.Text.Json;
using System.Text.Json.Serialization;

namespace KNP.Core.Storage;

public sealed class HistoryEntry
{
    [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    [JsonPropertyName("mode")] public string Mode { get; set; } = "";
    [JsonPropertyName("model")] public string Model { get; set; } = "";
    [JsonPropertyName("summary")] public string Summary { get; set; } = "";
    [JsonPropertyName("metrics")] public Dictionary<string, double> Metrics { get; set; } = new();
}

public sealed class HistoryFile
{
    [JsonPropertyName("runs")] public List<HistoryEntry> Runs { get; set; } = new();
}

public sealed class HistoryStore
{
    private readonly string _path;
    private const int MaxEntries = 10;
    private static readonly JsonSerializerOptions Opts = new() { WriteIndented = true };

    public HistoryStore(string path) { _path = path; }

    public HistoryFile Load()
    {
        if (!File.Exists(_path)) return new HistoryFile();
        try { return JsonSerializer.Deserialize<HistoryFile>(File.ReadAllText(_path)) ?? new HistoryFile(); }
        catch { return new HistoryFile(); }
    }

    public void Append(HistoryEntry entry)
    {
        var file = Load();
        file.Runs.Insert(0, entry);
        if (file.Runs.Count > MaxEntries) file.Runs = file.Runs.Take(MaxEntries).ToList();
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        File.WriteAllText(_path, JsonSerializer.Serialize(file, Opts));
    }

    public void Clear()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        File.WriteAllText(_path, JsonSerializer.Serialize(new HistoryFile(), Opts));
    }
}