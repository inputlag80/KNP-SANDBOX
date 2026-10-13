using KNP.Server;
using MudBlazor.Services;
using KNP.Core.Storage;
using KNP.Server.Services;

// Жёсткое завершение процесса при Ctrl+C — не ждём Blazor-WebSocket'ы
Console.CancelKeyPress += (s, e) =>
{
    e.Cancel = false;
    Environment.Exit(0);
};

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<HostOptions>(opts =>
{
    opts.ShutdownTimeout = TimeSpan.FromSeconds(2);
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
var dataRoot = Path.Combine(root, "data");

builder.Services.AddSingleton(new ModelCatalog(Path.Combine(dataRoot, "info")));
builder.Services.AddSingleton(new SessionStore(Path.Combine(dataRoot, "state", "session.json")));
builder.Services.AddSingleton(new HistoryStore(Path.Combine(dataRoot, "state", "history.json")));

builder.Services.AddSingleton<StorageService>();
builder.Services.AddSingleton<KnpSession>();
builder.Services.AddSingleton<TrainingLoop>();

var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapGet("/health", () => Results.Ok(new { status = "ok", time = DateTime.UtcNow }));
app.MapGet("/api/ping", () => Results.Ok(new { pong = true }));

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();