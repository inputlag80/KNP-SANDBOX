namespace KNP.Core.Models;

public sealed class ModelInfo
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Note { get; set; } = "";
}

public sealed class ModelCatalogFile
{
    public List<ModelInfo> Models { get; set; } = new();
}