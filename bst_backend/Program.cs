using System.Text.Json;
using System.IO;
using System.Threading;
using bst_backend.Services;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

Console.WriteLine("[bst_backend] Program starting - pid=" + Environment.ProcessId);

var builder = WebApplication.CreateBuilder(args);

// increase console logging so we can see startup/shutdown reasons
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options => { options.SingleLine = true; options.TimestampFormat = "hh:mm:ss "; });
builder.Logging.SetMinimumLevel(LogLevel.Debug);

builder.Services.AddSingleton<BstService>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors();

// POST /api/bst/insert?value=123
app.MapPost("/api/bst/insert", async (HttpRequest req, BstService svc) =>
{
    // Accept value via query (?value=5) or as a raw JSON body (5) or { "value": 5 }
    if (req.Query.TryGetValue("value", out var qv) && int.TryParse(qv, out var qint))
    {
        svc.Insert(qint);
        return Results.Ok();
    }

    try
    {
        // Ensure the request body can be read multiple times (buffers to file if needed)
        req.EnableBuffering();

        // Read the entire body as text (works for raw integers or JSON)
        req.Body.Position = 0;
        using var sr = new StreamReader(req.Body, leaveOpen: true);
        var bodyText = await sr.ReadToEndAsync();
        // Reset position so other middleware (if any) can read it
        req.Body.Position = 0;

        if (!string.IsNullOrWhiteSpace(bodyText))
        {
            // Try plain integer body first (e.g. "5")
            if (int.TryParse(bodyText.Trim(), out var plainInt))
            {
                svc.Insert(plainInt);
                return Results.Ok();
            }

            // Try parse JSON body: either a bare number or an object with a 'value' property
            try
            {
                using var doc = JsonDocument.Parse(bodyText);
                var root = doc.RootElement;
                if (root.ValueKind == JsonValueKind.Number && root.TryGetInt32(out var num))
                {
                    svc.Insert(num);
                    return Results.Ok();
                }

                if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("value", out var ve) && ve.TryGetInt32(out var v2))
                {
                    svc.Insert(v2);
                    return Results.Ok();
                }
            }
            catch
            {
                // ignore JSON parse errors and fall through to BadRequest below
            }
        }
    }
    catch
    {
        // fallthrough to bad request
    }

    return Results.BadRequest("Provide integer 'value' as query or JSON body");
});

// GET endpoints
app.MapGet("/api/bst/tree", (BstService svc) => Results.Json(svc.GetTree()));
app.MapGet("/api/bst/inorder", (BstService svc) => Results.Text(svc.Inorder()));
app.MapGet("/api/bst/preorder", (BstService svc) => Results.Text(svc.Preorder()));
app.MapGet("/api/bst/postorder", (BstService svc) => Results.Text(svc.Postorder()));
app.MapGet("/api/bst/levelorder", (BstService svc) => Results.Text(svc.LevelOrder()));
app.MapGet("/api/bst/min", (BstService svc) => Results.Text(svc.GetMinimum().ToString()));
app.MapGet("/api/bst/max", (BstService svc) => Results.Text(svc.GetMaximum().ToString()));
app.MapGet("/api/bst/totalnodes", (BstService svc) => Results.Text(svc.GetTotalNodes().ToString()));
app.MapGet("/api/bst/leafnodes", (BstService svc) => Results.Text(svc.GetLeafNodes().ToString()));
app.MapGet("/api/bst/height", (BstService svc) => Results.Text(svc.GetTreeHeight().ToString()));

// simple health endpoint
app.MapGet("/health", () => Results.Text("OK"));

// POST /api/bst/reset - clears the tree
app.MapPost("/api/bst/reset", (BstService svc) =>
{
    svc.Reset();
    return Results.Ok();
});

// Force listen on port 5000 so frontend's absolute URLs work.
// Keep this HTTP-only to avoid requiring a developer HTTPS certificate.
app.Urls.Clear();
app.Urls.Add("http://localhost:5000");

// simple root endpoint to make http://localhost:5000/ useful in a browser
app.MapGet("/", () => Results.Text("BST backend API is running. Use /api/bst/... endpoints."));

// log lifetime events so shutdown reasons are visible in the console
var lifetime = app.Lifetime;
lifetime.ApplicationStarted.Register(() => app.Logger.LogInformation("Application started and listening."));
lifetime.ApplicationStopping.Register(() =>
{
    app.Logger.LogWarning("Application is stopping.");
    try
    {
        var st = new System.Diagnostics.StackTrace(true);
        app.Logger.LogWarning("Stack trace at stopping:\n{trace}", st.ToString());
    }
    catch { }
});

lifetime.ApplicationStopped.Register(() => app.Logger.LogInformation("Application stopped."));

// global exception / process handlers to help debug unexpected shutdowns
AppDomain.CurrentDomain.UnhandledException += (s, e) =>
{
    try { app.Logger.LogCritical((Exception?)e.ExceptionObject, "Unhandled exception"); } catch { }
};

// log console signals (Ctrl+C / Ctrl+Break) and process exit so we can see what triggered shutdown
Console.CancelKeyPress += (s, e) =>
{
    try
    {
        app.Logger.LogWarning("Console.CancelKeyPress: SpecialKey={key}, CancelBefore={cancel}", e.SpecialKey, e.Cancel);
    }
    catch { }
};

AppDomain.CurrentDomain.ProcessExit += (s, e) =>
{
    try { app.Logger.LogWarning("ProcessExit event fired"); } catch { }
};

// keep default signal behavior (Ctrl+C / SIGTERM will stop the app). Use a dedicated terminal to run the backend.

// run inside try/catch to surface exceptions to the console
try
{
    // enable swagger UI in Development and Production for local testing
    app.UseSwagger();
    app.UseSwaggerUI();

    // no special posix signal handling - allow default shutdown behavior

    app.Run();
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Host terminated unexpectedly");
    throw;
}