using System.Text.Json;
using KNP.Core.Models;

namespace KNP.Core.Storage;

public sealed class ModelCatalog
{
    private readonly string _infoDir;
    private static readonly JsonSerializerOptions Opts = new() { PropertyNameCaseInsensitive = true };

    public ModelCatalog(string infoDir) { _infoDir = infoDir; }

    public IReadOnlyList<ModelInfo> GetModels()
    {
        var path = Path.Combine(_infoDir, "models.json");
        if (!File.Exists(path)) return Array.Empty<ModelInfo>();
        var file = JsonSerializer.Deserialize<ModelCatalogFile>(File.ReadAllText(path), Opts);
        return file?.Models ?? new List<ModelInfo>();
    }

    public ModelSpec? GetSpec(string modelId)
    {
        var all = GetAllSpecs();
        return all.TryGetValue(modelId, out var spec) ? spec : null;
    }

    public IReadOnlyDictionary<string, ModelSpec> GetAllSpecs()
    {
        var path = Path.Combine(_infoDir, "specs.json");
        if (!File.Exists(path)) return new Dictionary<string, ModelSpec>();
        var file = JsonSerializer.Deserialize<ModelSpecsFile>(File.ReadAllText(path), Opts);
        return file?.Specs ?? new Dictionary<string, ModelSpec>();
    }
}