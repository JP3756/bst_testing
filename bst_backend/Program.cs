using System.Text.Json;
using System.IO;
using bst_backend.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        // Configure Serilog to log to both console and file
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
            .WriteTo.File("backend.log", 
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}",
                retainedFileCountLimit: 7)
            .CreateLogger();

        try
        {
            Log.Information("Starting BST Backend API");

            var builder = WebApplication.CreateBuilder(args);

            // Use Serilog for logging
            builder.Host.UseSerilog();

        builder.Services.AddSingleton<BstService>();
        builder.Services.AddCors(options => options.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();
        app.UseCors();

        // Register endpoints
        MapEndpoints(app);

        // Configure listen URLs
        ConfigureUrls(app);

        app.UseSwagger();
        app.UseSwaggerUI();

        try
        {
            await app.RunAsync();
            Log.Information("Application shut down gracefully");
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
            return 1;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Application failed to start");
        return 1;
    }
    finally
    {
        await Log.CloseAndFlushAsync();
    }
}

    private static void ConfigureUrls(WebApplication app)
    {
        var urlsEnv = Environment.GetEnvironmentVariable("BST_BACKEND_URLS");
        var singleUrl = Environment.GetEnvironmentVariable("BST_BACKEND_URL");
        app.Urls.Clear();
        if (!string.IsNullOrWhiteSpace(urlsEnv))
        {
            foreach (var u in urlsEnv.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var trimmed = u.Trim();
                if (!string.IsNullOrEmpty(trimmed)) app.Urls.Add(trimmed);
            }
            app.Logger.LogInformation("Using BST_BACKEND_URLS from environment: {urls}", urlsEnv);
        }
        else if (!string.IsNullOrWhiteSpace(singleUrl))
        {
            app.Urls.Add(singleUrl);
            app.Logger.LogInformation("Using BST_BACKEND_URL from environment: {url}", singleUrl);
        }
        else
        {
            app.Urls.Add("http://localhost:5000");
            app.Urls.Add("https://localhost:5001");
            app.Logger.LogInformation("Using default listen URLs: http://localhost:5000 and https://localhost:5001");
        }
    }

    private static void MapEndpoints(WebApplication app)
    {
        // Helper to read an integer from query or request body (plain integer or JSON { "value": n })
        static async Task<int?> ReadIntFromRequestAsync(HttpRequest req)
        {
            if (req.Query.TryGetValue("value", out var qv) && int.TryParse(qv, out var qint))
                return qint;

            req.EnableBuffering();
            req.Body.Position = 0;
            using var sr = new StreamReader(req.Body, leaveOpen: true);
            var bodyText = await sr.ReadToEndAsync();
            req.Body.Position = 0;

            if (string.IsNullOrWhiteSpace(bodyText)) return null;

            if (int.TryParse(bodyText.Trim(), out var plainInt)) return plainInt;

            try
            {
                using var doc = JsonDocument.Parse(bodyText);
                var root = doc.RootElement;
                if (root.ValueKind == JsonValueKind.Number && root.TryGetInt32(out var num)) return num;
                if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("value", out var ve) && ve.TryGetInt32(out var v2)) return v2;
            }
            catch { }

            return null;
        }

        // Helper to read multiple integers from the request body (JSON array or comma-separated text)
        static async Task<int[]?> ReadIntsFromRequestAsync(HttpRequest req)
        {
            if (req.Query.TryGetValue("value", out var values) && values.Count > 0)
            {
                var list = new List<int>();
                foreach (var v in values)
                {
                    if (int.TryParse(v, out var iv)) list.Add(iv);
                }
                if (list.Count > 0) return list.ToArray();
            }

            req.EnableBuffering();
            req.Body.Position = 0;
            using var sr = new StreamReader(req.Body, leaveOpen: true);
            var bodyText = await sr.ReadToEndAsync();
            req.Body.Position = 0;

            if (string.IsNullOrWhiteSpace(bodyText)) return null;

            try
            {
                using var doc = JsonDocument.Parse(bodyText);
                var root = doc.RootElement;
                if (root.ValueKind == JsonValueKind.Array)
                {
                    var list = new List<int>();
                    foreach (var el in root.EnumerateArray())
                    {
                        if (el.ValueKind == JsonValueKind.Number && el.TryGetInt32(out var n)) list.Add(n);
                    }
                    if (list.Count > 0) return list.ToArray();
                }
            }
            catch { }

            var pieces = bodyText.Split(new[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var outList = new List<int>();
            foreach (var p in pieces)
            {
                if (int.TryParse(p.Trim(), out var iv)) outList.Add(iv);
            }
            return outList.Count > 0 ? outList.ToArray() : null;
        }

        // POST /api/bst/insert
        app.MapPost("/api/bst/insert", async (HttpRequest req, BstService svc) =>
        {
            var val = await ReadIntFromRequestAsync(req);
            if (val is int v)
            {
                svc.Insert(v);
                app.Logger.LogInformation("Inserted value {Value} from {RemoteIp}", v, req.HttpContext.Connection.RemoteIpAddress?.ToString());
                return Results.Ok();
            }

            req.EnableBuffering();
            req.Body.Position = 0;
            using var sr = new StreamReader(req.Body, leaveOpen: true);
            var snippet = (await sr.ReadToEndAsync() ?? string.Empty).Replace("\n", " ").Replace("\r", " ");
            if (snippet.Length > 200) snippet = snippet.Substring(0, 200) + "...";
            req.Body.Position = 0;

            app.Logger.LogWarning("Bad insert request to /api/bst/insert from {RemoteIp}. Body snippet: {Snippet}", req.HttpContext.Connection.RemoteIpAddress?.ToString(), snippet);
            return Results.BadRequest("Provide integer 'value' as query parameter or in the request body (plain integer or JSON). See API docs.");
        });

        // Bulk insert endpoint
        app.MapPost("/api/bst/insert-bulk", async (HttpRequest req, BstService svc) =>
        {
            var vals = await ReadIntsFromRequestAsync(req);
            if (vals != null && vals.Length > 0)
            {
                foreach (var x in vals) svc.Insert(x);
                app.Logger.LogInformation("Bulk inserted {Count} values from {RemoteIp}", vals.Length, req.HttpContext.Connection.RemoteIpAddress?.ToString());
                return Results.Ok(new { inserted = vals.Length });
            }
            return Results.BadRequest("Provide an array of integers in the request body (JSON array) or a comma-separated list.");
        });

        // GET endpoints
        app.MapGet("/api/bst/tree", (BstService svc) => Results.Json(svc.GetTree()));
        app.MapGet("/api/bst/inorder", (BstService svc) => Results.Text(svc.Inorder()));
        app.MapGet("/api/bst/preorder", (BstService svc) => Results.Text(svc.Preorder()));
        app.MapGet("/api/bst/postorder", (BstService svc) => Results.Text(svc.Postorder()));
        app.MapGet("/api/bst/levelorder", (BstService svc) => Results.Text(svc.LevelOrder()));
        app.MapGet("/api/bst/min", (BstService svc) =>
        {
            var m = svc.GetMinimum();
            return m.HasValue ? Results.Text(m.Value.ToString()) : Results.NoContent();
        });

        app.MapGet("/api/bst/max", (BstService svc) =>
        {
            var m = svc.GetMaximum();
            return m.HasValue ? Results.Text(m.Value.ToString()) : Results.NoContent();
        });
        app.MapGet("/api/bst/totalnodes", (BstService svc) => Results.Text(svc.GetTotalNodes().ToString()));
        app.MapGet("/api/bst/leafnodes", (BstService svc) => Results.Text(svc.GetLeafNodes().ToString()));
        app.MapGet("/api/bst/height", (BstService svc) => Results.Text(svc.GetTreeHeight().ToString()));

        app.MapGet("/health", () => Results.Text("OK"));

        app.MapPost("/api/bst/reset", (BstService svc) =>
        {
            svc.Reset();
            return Results.Ok();
        });

        app.MapGet("/", () => Results.Text("BST backend API is running. Use /api/bst/... endpoints."));

        var lifetime = app.Lifetime;
        lifetime.ApplicationStarted.Register(() => app.Logger.LogInformation("Application started and listening."));
        lifetime.ApplicationStopping.Register(() => app.Logger.LogWarning("Application is stopping."));
        lifetime.ApplicationStopped.Register(() => app.Logger.LogInformation("Application stopped."));

        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            Log.Fatal(e.ExceptionObject as Exception, "Unhandled AppDomain exception");
        };
    }
}