using System.Text.Json;
using System.Text.Json.Serialization;

namespace KNP.Core.Storage;

public sealed class DistillSettings
{
    [JsonPropertyName("teacher")]     public string Teacher     { get; set; } = "gpt2-large";
    [JsonPropertyName("student")]     public string Student     { get; set; } = "gpt2";
    [JsonPropertyName("temperature")] public double Temperature { get; set; } = 4.0;
    [JsonPropertyName("alpha")]       public double Alpha       { get; set; } = 0.7;
    [JsonPropertyName("lr")]          public double Lr          { get; set; } = 1.0;
    [JsonPropertyName("epochs")]      public int    Epochs      { get; set; } = 25;
}

public sealed class QuantSettings
{
    [JsonPropertyName("model")] public string Model { get; set; } = "llama-7b";
    [JsonPropertyName("bits")]  public int    Bits  { get; set; } = 8;
}

public sealed class SessionData
{
    [JsonPropertyName("mode")]     public string Mode    { get; set; } = "distill";
    [JsonPropertyName("distill")]  public DistillSettings Distill { get; set; } = new();
    [JsonPropertyName("quant")]    public QuantSettings Quant { get; set; } = new();
    [JsonPropertyName("theme")]    public string Theme   { get; set; } = "dark";
    [JsonPropertyName("accent")]   public string Accent  { get; set; } = "purple";
    [JsonPropertyName("dev_mode")] public bool   DevMode { get; set; }
}

public sealed class SessionStore
{
    private readonly string _path;
    private static readonly JsonSerializerOptions Opts = new() { WriteIndented = true };

    public SessionStore(string path) { _path = path; }

    public SessionData Load()
    {
        if (!File.Exists(_path)) return new SessionData();
        try
        {
            var json = File.ReadAllText(_path);
            return JsonSerializer.Deserialize<SessionData>(json) ?? new SessionData();
        }
        catch { return new SessionData(); }
    }

    public void Save(SessionData data)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        File.WriteAllText(_path, JsonSerializer.Serialize(data, Opts));
    }
}