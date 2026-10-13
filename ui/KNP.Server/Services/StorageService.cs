using KNP.Core.Storage;
using KNP.Core.Models;

namespace KNP.Server.Services;

public sealed class StorageService
{
    private readonly SessionStore _session;
    private readonly HistoryStore _history;
    private readonly ModelCatalog _catalog;

    public StorageService(SessionStore session, HistoryStore history, ModelCatalog catalog)
    {
        _session = session;
        _history = history;
        _catalog = catalog;
    }

    public SessionData LoadSession() => _session.Load();
    public void SaveSession(SessionData data) => _session.Save(data);

    public void AppendHistory(HistoryEntry entry) => _history.Append(entry);
    public HistoryFile LoadHistory() => _history.Load();
    public void ClearHistory() => _history.Clear();

    public IReadOnlyList<ModelInfo> GetModels() => _catalog.GetModels();
    public ModelSpec? GetSpec(string id) => _catalog.GetSpec(id);
    public IReadOnlyDictionary<string, ModelSpec> GetAllSpecs() => _catalog.GetAllSpecs();
}