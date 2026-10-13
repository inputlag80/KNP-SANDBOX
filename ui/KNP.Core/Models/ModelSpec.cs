using System.Text.Json.Serialization;

namespace KNP.Core.Models;

public sealed class ModelSpec
{
    [JsonPropertyName("params")]   public long   Params  { get; set; }
    [JsonPropertyName("base_ppl")] public double BasePpl { get; set; }
    [JsonPropertyName("fp32_mb")]  public int    Fp32Mb  { get; set; }
}

public sealed class ModelSpecsFile
{
    [JsonPropertyName("specs")]
    public Dictionary<string, ModelSpec> Specs { get; set; } = new();
}