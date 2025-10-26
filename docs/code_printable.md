# BST Frontend + Backend — Printable Source Bundle

This document collects the key project files and scripts from the workspace in a single Markdown file for printing or PDF export. Each section shows the filename and the source content.

---

## backend/Program.cs

```csharp
using System.Text.Json;
using System.IO;
using bst_backend.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

// Console logging for local development
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options => { options.SingleLine = true; options.TimestampFormat = "hh:mm:ss "; });
builder.Logging.SetMinimumLevel(LogLevel.Debug);

builder.Services.AddSingleton<BstService>();
builder.Services.AddCors(options => options.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors();

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

// POST /api/bst/insert
app.MapPost("/api/bst/insert", async (HttpRequest req, BstService svc) =>
{
    var val = await ReadIntFromRequestAsync(req);
    if (val is int v)
    {
        svc.Insert(v);
        return Results.Ok();
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

app.MapGet("/health", () => Results.Text("OK"));

app.MapPost("/api/bst/reset", (BstService svc) =>
{
    svc.Reset();
    return Results.Ok();
});

// Force listen on port 5000 (frontend expects this)
app.Urls.Clear();
app.Urls.Add("http://localhost:5000");

app.MapGet("/", () => Results.Text("BST backend API is running. Use /api/bst/... endpoints."));

var lifetime = app.Lifetime;
lifetime.ApplicationStarted.Register(() => app.Logger.LogInformation("Application started and listening."));
lifetime.ApplicationStopping.Register(() => app.Logger.LogWarning("Application is stopping."));
lifetime.ApplicationStopped.Register(() => app.Logger.LogInformation("Application stopped."));

AppDomain.CurrentDomain.UnhandledException += (s, e) =>
{
    try { app.Logger.LogCritical(e.ExceptionObject as Exception, "Unhandled exception"); } catch { }
};

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
```

---

## backend/Services/BstService.cs

```csharp
namespace bst_backend.Services
{
    public class BstNodeModel
    {
        public int Value { get; set; }
        public BstNodeModel? Left { get; set; }
        public BstNodeModel? Right { get; set; }
        // Height used for AVL balancing
        public int Height { get; set; } = 1;
    }

    public class BstService
    {
        private BstNodeModel? _root;
        private readonly object _lock = new();

        public void Insert(int value)
        {
            lock (_lock)
            {
                _root = InsertInternal(_root, value);
            }
        }

        public void Reset()
        {
            lock (_lock) { _root = null; }
        }

        private BstNodeModel InsertInternal(BstNodeModel? node, int value)
        {
            if (node is null) return new BstNodeModel { Value = value };

            if (value < node.Value)
                node.Left = InsertInternal(node.Left, value);
            else if (value > node.Value)
                node.Right = InsertInternal(node.Right, value);
            else
                return node; // duplicate, ignore

            node.Height = 1 + Math.Max(Height(node.Left), Height(node.Right));

            var balance = GetBalance(node);

            // Left Left
            if (balance > 1 && value < node.Left!.Value) return RightRotate(node);
            // Right Right
            if (balance < -1 && value > node.Right!.Value) return LeftRotate(node);
            // Left Right
            if (balance > 1 && value > node.Left!.Value)
            {
                node.Left = LeftRotate(node.Left!);
                return RightRotate(node);
            }
            // Right Left
            if (balance < -1 && value < node.Right!.Value)
            {
                node.Right = RightRotate(node.Right!);
                return LeftRotate(node);
            }

            return node;
        }

        private int Height(BstNodeModel? node) => node?.Height ?? 0;

        private int GetBalance(BstNodeModel? node) => node == null ? 0 : Height(node.Left) - Height(node.Right);

        private BstNodeModel RightRotate(BstNodeModel y)
        {
            var x = y.Left!;
            var t2 = x.Right;

            x.Right = y;
            y.Left = t2;

            y.Height = 1 + Math.Max(Height(y.Left), Height(y.Right));
            x.Height = 1 + Math.Max(Height(x.Left), Height(x.Right));

            return x;
        }

        private BstNodeModel LeftRotate(BstNodeModel x)
        {
            var y = x.Right!;
            var t2 = y.Left;

            y.Left = x;
            x.Right = t2;

            x.Height = 1 + Math.Max(Height(x.Left), Height(x.Right));
            y.Height = 1 + Math.Max(Height(y.Left), Height(y.Right));

            return y;
        }

        public BstNodeModel? GetTree()
        {
            lock (_lock) { return _root; }
        }

        private void TraverseInorder(BstNodeModel? node, List<int> outList)
        {
            if (node is null) return;
            TraverseInorder(node.Left, outList);
            outList.Add(node.Value);
            TraverseInorder(node.Right, outList);
        }

        private void TraversePreorder(BstNodeModel? node, List<int> outList)
        {
            if (node is null) return;
            outList.Add(node.Value);
            TraversePreorder(node.Left, outList);
            TraversePreorder(node.Right, outList);
        }

        private void TraversePostorder(BstNodeModel? node, List<int> outList)
        {
            if (node is null) return;
            TraversePostorder(node.Left, outList);
            TraversePostorder(node.Right, outList);
            outList.Add(node.Value);
        }

        public string Inorder()
        {
            lock (_lock) { var l = new List<int>(); TraverseInorder(_root, l); return string.Join(",", l); }
        }

        public string Preorder()
        {
            lock (_lock) { var l = new List<int>(); TraversePreorder(_root, l); return string.Join(",", l); }
        }

        public string Postorder()
        {
            lock (_lock) { var l = new List<int>(); TraversePostorder(_root, l); return string.Join(",", l); }
        }

        public string LevelOrder()
        {
            lock (_lock)
            {
                if (_root == null) return string.Empty;
                var q = new Queue<BstNodeModel>();
                var outList = new List<int>();
                q.Enqueue(_root);
                while (q.Count > 0)
                {
                    var n = q.Dequeue();
                    outList.Add(n.Value);
                    if (n.Left != null) q.Enqueue(n.Left);
                    if (n.Right != null) q.Enqueue(n.Right);
                }
                return string.Join(",", outList);
            }
        }

        public int GetMinimum()
        {
            lock (_lock)
            {
                if (_root == null) return 0;
                var cur = _root;
                while (cur.Left != null) cur = cur.Left;
                return cur.Value;
            }
        }

        public int GetMaximum()
        {
            lock (_lock)
            {
                if (_root == null) return 0;
                var cur = _root;
                while (cur.Right != null) cur = cur.Right;
                return cur.Value;
            }
        }

        public int GetTotalNodes() { lock (_lock) { return CountNodes(_root); } }
        private int CountNodes(BstNodeModel? node) { if (node == null) return 0; return 1 + CountNodes(node.Left) + CountNodes(node.Right); }

        public int GetLeafNodes() { lock (_lock) { return CountLeaves(_root); } }
        private int CountLeaves(BstNodeModel? node) { if (node == null) return 0; if (node.Left==null && node.Right==null) return 1; return CountLeaves(node.Left) + CountLeaves(node.Right); }

        public int GetTreeHeight() { lock (_lock) { return Height(_root); } }
    }
}
```

---

## backend/bst_backend.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <RootNamespace>bst_backend</RootNamespace>
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
  </ItemGroup>
  <ItemGroup>
    <!-- Make logging types available even in environments where implicit references are not resolved -->
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="8.0.0" />
  </ItemGroup>
  <ItemGroup>
    <!-- Keep logging console provider for local development logs -->
    <PackageReference Include="Microsoft.Extensions.Logging.Console" Version="8.0.0" />
  </ItemGroup>
  <!-- Exclude test and auxiliary project folders that live inside the repo root
       so the web project doesn't accidentally compile test sources. -->
  <ItemGroup>
    <Compile Remove="Tests\**\*.cs" />
    <Compile Remove="unit-tests\**\*.cs" />
    <Compile Remove="unit-tests-mstest\**\*.cs" />
    <Compile Remove="tests-runner\**\*.cs" />
  </ItemGroup>
</Project>
```

---

## frontend/bst_frontend.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk.BlazorWebAssembly">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" Version="8.0.0" />
    <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.DevServer" Version="8.0.0" PrivateAssets="all" />
  </ItemGroup>

  <ItemGroup>
    <RazorComponent Include="**/*.razor" />
  </ItemGroup>

  <!-- Exclude the backend project files (added into the repo) from frontend compilation
       to avoid duplicate top-level statements and duplicate assembly attributes. -->
  <ItemGroup>
    <Compile Remove="bst_backend\**\*.cs" />
    <None Remove="bst_backend\**\*.cs" />
    <None Remove="bst_backend\**\obj\**\*" />
    <Content Remove="bst_backend\**\*" />
  </ItemGroup>

</Project>
```

---

## frontend/Services/BstApiService.cs

```csharp
using System.Net.Http.Json;
using System.Text.Json;

namespace bst_frontend.Services
{
    public class BstNodeModel
    {
        public int Value { get; set; }
        public BstNodeModel? Left { get; set; }
        public BstNodeModel? Right { get; set; }
    }

    public class BstApiService
    {
        private readonly HttpClient _httpClient;

        public BstApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task Insert(int value)
        {
            var response = await _httpClient.PostAsync($"/api/bst/insert?value={value}", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task Reset()
        {
            var response = await _httpClient.PostAsync($"/api/bst/reset", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task<BstNodeModel?> GetTreeAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/bst/tree");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return JsonSerializer.Deserialize<BstNodeModel>(json, options);
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> GetInorderTraversal()
        {
            var response = await _httpClient.GetAsync("/api/bst/inorder");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetPreorderTraversal()
        {
            var response = await _httpClient.GetAsync("/api/bst/preorder");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetPostorderTraversal()
        {
            var response = await _httpClient.GetAsync("/api/bst/postorder");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetLevelOrderTraversal()
        {
            var response = await _httpClient.GetAsync("/api/bst/levelorder");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<int> GetMinimum()
        {
            var response = await _httpClient.GetAsync("/api/bst/min");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return int.Parse(content);
        }

        public async Task<int> GetMaximum()
        {
            var response = await _httpClient.GetAsync("/api/bst/max");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return int.Parse(content);
        }

        public async Task<int> GetTotalNodes()
        {
            var response = await _httpClient.GetAsync("/api/bst/totalnodes");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return int.Parse(content);
        }

        public async Task<int> GetLeafNodes()
        {
            var response = await _httpClient.GetAsync("/api/bst/leafnodes");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return int.Parse(content);
        }

        public async Task<int> GetTreeHeight()
        {
            var response = await _httpClient.GetAsync("/api/bst/height");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return int.Parse(content);
        }
    }
}
```

---

## frontend/Pages/Index.razor
```razor
@page "/"
@using bst_frontend.Services

@inject BstApiService ApiService

<PageTitle>Binary Search Tree Builder</PageTitle>

<div class="container mt-4">
    <h1>Binary Search Tree Builder and Analyzer</h1>

    <div class="row">
        <div class="col-md-6">
            <h3>Insert Value</h3>
            <div class="mb-3">
                <input type="text" class="form-control" @bind="numberInput" placeholder="Enter a number" />
            </div>
            <div class="d-flex gap-2">
                <button class="btn btn-primary" @onclick="InsertValue" disabled="@isInserting">
                @if (isInserting)
                {
                    <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                    @:Inserting...
                }
                else
                {
                    @:Insert
                }
                </button>
                <button class="btn btn-warning" @onclick="RestartTree">Restart</button>
            </div>
            @if (!string.IsNullOrEmpty(message))
            {
                <div class="mt-3 alert @alertClass">@message</div>
            }

            <h4 class="mt-4">Tree Traversals</h4>
            <div class="mb-3 traversal-buttons">
                <button class="btn btn-outline-primary" @onclick="GetInorder">Inorder</button>
                <button class="btn btn-outline-primary" @onclick="GetPreorder">Preorder</button>
                <button class="btn btn-outline-primary" @onclick="GetPostorder">Postorder</button>
                <button class="btn btn-outline-primary" @onclick="GetLevelOrder">Level Order</button>
            </div>
            @if (!string.IsNullOrEmpty(traversalResult))
            {
                <div class="mt-3 traversal-result">
                    <h5>Traversal Result</h5>
                    <p>@traversalResult</p>
                </div>
            }

            <h4 class="mt-4">Tree Analytics</h4>
            <div class="row tree-analytics">
                <div class="col-sm-6 col-md-4 mb-3">
                    <div class="card">
                        <div class="card-body">
                            <h6 class="card-title">Minimum</h6>
                            <p class="card-text">@((minValue.HasValue) ? minValue.Value.ToString() : "-")</p>
                        </div>
                    </div>
                </div>
                <div class="col-sm-6 col-md-4 mb-3">
                    <div class="card">
                        <div class="card-body">
                            <h6 class="card-title">Maximum</h6>
                            <p class="card-text">@((maxValue.HasValue) ? maxValue.Value.ToString() : "-")</p>
                        </div>
                    </div>
                </div>
                <div class="col-sm-6 col-md-4 mb-3">
                    <div class="card">
                        <div class="card-body">
                            <h6 class="card-title">Total Nodes</h6>
                            <p class="card-text">@totalNodes</p>
                        </div>
                    </div>
                </div>
                <div class="col-sm-6 col-md-4 mb-3">
                    <div class="card">
                        <div class="card-body">
                            <h6 class="card-title">Leaf Nodes</h6>
                            <p class="card-text">@leafNodes</p>
                        </div>
                    </div>
                </div>
                <div class="col-sm-6 col-md-4 mb-3">
                    <div class="card">
                        <div class="card-body">
                            <h6 class="card-title">Tree Height</h6>
                            <p class="card-text">@treeHeight</p>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-md-6">
            <TreeVisualizer Root="treeData" />
        </div>

    </div>
</div>

@code {
    private string numberInput = "";
    private BstNodeModel? treeData = null;
    private string message = "";
    private string alertClass = "";
    private bool isInserting = false;
    private string traversalResult = "";
    private int? minValue = null;
    private int? maxValue = null;
    private int totalNodes = 0;
    private int leafNodes = 0;
    private int treeHeight = 0;

    private async Task InsertValue()
    {
        if (int.TryParse(numberInput, out int value))
        {
            isInserting = true;
            message = "";
            try
            {
                await ApiService.Insert(value);
                treeData = await ApiService.GetTreeAsync();
                await LoadAnalytics();
                message = $"Successfully inserted {value}.";
                alertClass = "alert-success";
                numberInput = "";
            }
            catch (Exception ex)
            {
                message = $"Error inserting {value}: {ex.Message}";
                alertClass = "alert-danger";
            }
            finally
            {
                isInserting = false;
            }
        }
        else
        {
            message = "Please enter a valid integer.";
            alertClass = "alert-warning";
        }
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            treeData = await ApiService.GetTreeAsync();
            await LoadAnalytics();
        }
        catch
        {
            // Ignore errors on initial load
        }
    }

    private async Task GetInorder()
    {
        try
        {
            traversalResult = await ApiService.GetInorderTraversal();
        }
        catch (Exception ex)
        {
            traversalResult = $"Error: {ex.Message}";
        }
    }

    private async Task GetPreorder()
    {
        try
        {
            traversalResult = await ApiService.GetPreorderTraversal();
        }
        catch (Exception ex)
        {
            traversalResult = $"Error: {ex.Message}";
        }
    }

    private async Task GetPostorder()
    {
        try
        {
            traversalResult = await ApiService.GetPostorderTraversal();
        }
        catch (Exception ex)
        {   
            traversalResult = $"Error: {ex.Message}";
        }
    }

    private async Task GetLevelOrder()
    {
        try
        {
            traversalResult = await ApiService.GetLevelOrderTraversal();
        }
        catch (Exception ex)
        {
            traversalResult = $"Error: {ex.Message}";
        }
    }

    private async Task LoadAnalytics()
    {
        try
        {
            minValue = await ApiService.GetMinimum();
            maxValue = await ApiService.GetMaximum();
            totalNodes = await ApiService.GetTotalNodes();
            leafNodes = await ApiService.GetLeafNodes();
            treeHeight = await ApiService.GetTreeHeight();
        }
        catch
        {
            // Ignore errors for analytics
        }
    }

    private async Task RestartTree()
    {
        try
        {
            await ApiService.Reset();
            treeData = await ApiService.GetTreeAsync();
            await LoadAnalytics();
            message = "Tree restarted.";
            alertClass = "alert-info";
        }
        catch (Exception ex)
        {
            message = $"Error restarting tree: {ex.Message}";
            alertClass = "alert-danger";
        }
    }
}
```

---

## frontend/Components/TreeVisualizer.razor

```razor
@using bst_frontend.Services

<div class="tree-visualizer border rounded p-3">
    @if (Root == null)
    {
        <p class="text-muted">Tree visualization will appear here.</p>
    }
    else
    {
        <div class="tree-container">
            <TreeNode Node="Root" />
        </div>
    }
</div>

@code {
    [Parameter]
    public BstNodeModel? Root { get; set; }
}
```

---

## frontend/Components/TreeNode.razor

```razor
@using bst_frontend.Services

<div class="tree-node">
    <div class="node-circle">@Node.Value</div>
    @if (Node.Left != null || Node.Right != null)
    {
        <div class="children">
            @if (Node.Left != null)
            {
                <div class="child left">
                    <div class="connector"></div>
                    <TreeNode Node="Node.Left" />
                </div>
            }
            @if (Node.Right != null)
            {
                <div class="child right">
                    <div class="connector"></div>
                    <TreeNode Node="Node.Right" />
                </div>
            }
        </div>
    }
</div>

@code {
    [Parameter]
    public BstNodeModel Node { get; set; } = new();
}
```

---

## scripts/start-backend.ps1

```powershell
# Start the backend (detached) with optional log redirection and pid file.
<#
Usage:
	.\start-backend.ps1 [-ProjectPath <path>] [-WorkingDir <path>] [-OutLog <path>] [-ErrLog <path>] [-PidFile <path>]

Defaults:
	ProjectPath = ..\bst_backend.csproj
	WorkingDir  = parent folder of this script
	OutLog      = ./backend.log
	ErrLog      = ./backend.err.log
	PidFile     = ./backend.pid
#>

[CmdletBinding()]
param(
		[string]$ProjectPath = (Join-Path $PSScriptRoot "..\bst_backend.csproj"),
		[string]$WorkingDir = (Resolve-Path (Join-Path $PSScriptRoot "..")),
		[string]$OutLog = (Join-Path (Resolve-Path $PSScriptRoot) 'backend.log'),
		[string]$ErrLog = (Join-Path (Resolve-Path $PSScriptRoot) 'backend.err.log'),
		[string]$PidFile = (Join-Path (Resolve-Path $PSScriptRoot) 'backend.pid')
)

Write-Host "Starting backend from project: $ProjectPath"
if (Test-Path $OutLog) { Remove-Item $OutLog -Force -ErrorAction SilentlyContinue }
if (Test-Path $ErrLog) { Remove-Item $ErrLog -Force -ErrorAction SilentlyContinue }

$argString = "run --project `"$ProjectPath`""
$proc = Start-Process -FilePath "dotnet" -ArgumentList $argString -WorkingDirectory $WorkingDir -RedirectStandardOutput $OutLog -RedirectStandardError $ErrLog -PassThru

if ($proc -and $proc.Id) {
		Write-Host "Started backend with PID: $($proc.Id)"
		try { $proc.Id | Out-File -FilePath $PidFile -Encoding ascii -Force } catch {}
		Write-Host "Logs: $OutLog and $ErrLog"
		Write-Host "To stop: .\stop-backend.ps1 or Stop-Process -Id $($proc.Id)"
} else {
		Write-Error "Failed to start backend process. See $ErrLog for details."
}
```

---

## scripts/stop-backend.ps1

```powershell
<#
Stop backend by PID file or by scanning running dotnet processes for the backend project.
Usage: .\stop-backend.ps1 [-PidFile <path>]
#>

[CmdletBinding()]
param(
	[string]$PidFile = (Join-Path (Resolve-Path $PSScriptRoot) 'backend.pid')
)

if (Test-Path $PidFile) {
	try {
		$pidContent = Get-Content $PidFile -ErrorAction Stop | Select-Object -First 1
		$pidValue = 0
		if ($pidContent -and [int]::TryParse($pidContent, [ref]$pidValue)) {
			Write-Host "Stopping backend PID: $pidValue"
			Stop-Process -Id $pidValue -Force -ErrorAction SilentlyContinue
			Remove-Item $PidFile -ErrorAction SilentlyContinue
			return
		}
	} catch {
		$errMsg = $_.Exception.Message
		Write-Warning "Failed to read pid file ${PidFile}: $errMsg"
	}
}

Write-Host "Pid file not found or invalid. Scanning dotnet processes for the backend project name."
Get-Process -Name dotnet -ErrorAction SilentlyContinue | ForEach-Object {
	try {
		$procInfo = Get-CimInstance Win32_Process -Filter "ProcessId=$($_.Id)"
		$cmd = $procInfo.CommandLine
		if ($cmd -and $cmd -match 'bst_backend') {
			Write-Host "Stopping process $($_.Id) -> $cmd"
			Stop-Process -Id $_.Id -Force -ErrorAction SilentlyContinue
		}
	} catch { }
}
```

---

## scripts/start-all.ps1

```powershell
(See full script in project. The script starts backend and frontend, waits for ports, redirects logs to scripts/logs, and runs a short smoke test.)
```

---

## scripts/smoke-test.ps1

```powershell
# Simple smoke test script for bst backend
# Usage: powershell -ExecutionPolicy Bypass -File ./smoke-test.ps1

$base = 'http://localhost:5000/api/bst'
Write-Host "Calling $base/reset"
Invoke-RestMethod -Method Post -Uri "$base/reset"

Write-Host "Inserting 10"
Invoke-RestMethod -Method Post -Uri "$base/insert?value=10" -Body 10 -ContentType 'text/plain'
Write-Host "Inserting 20"
Invoke-RestMethod -Method Post -Uri "$base/insert?value=20" -Body 20 -ContentType 'text/plain'
Write-Host "Inserting 5"
Invoke-RestMethod -Method Post -Uri "$base/insert?value=5" -Body 5 -ContentType 'text/plain'

Write-Host "Getting inorder"
$tree = Invoke-RestMethod -Method Get -Uri "$base/inorder"
Write-Host "Inorder after inserts: $tree"

Write-Host "Resetting"
Invoke-RestMethod -Method Post -Uri "$base/reset"
$after = Invoke-RestMethod -Method Get -Uri "$base/inorder"
Write-Host "Inorder after reset: $after"
```

---

## Tests/BstService.Tests/BstServiceTests.cs

```csharp
using Xunit;
using bst_backend.Services;

namespace BstService.Tests
{
    public class BstServiceTests
    {
        [Fact]
        public void Insert_MultipleValues_InorderIsSorted()
        {
            var svc = new bst_backend.Services.BstService();
            svc.Insert(5);
            svc.Insert(3);
            svc.Insert(7);
            svc.Insert(4);
            svc.Insert(6);

            var inorder = svc.Inorder();

            Assert.Equal("3,4,5,6,7", inorder);
        }

        [Fact]
        public void Insert_Balances_HeightIsReasonable()
        {
            var svc = new bst_backend.Services.BstService();
            // insert ascending to force rotations if AVL not implemented
            for (int i = 1; i <= 7; i++) svc.Insert(i);

            var height = svc.GetTreeHeight();

            // For AVL tree height of 7 nodes should be <= 4
            Assert.InRange(height, 1, 4);
        }
    }
}
```

---

## Notes

- The backend listens on http://localhost:5000 by default (see `Program.cs`). The frontend uses http://localhost:5000 as the API base address in `BstApiService`.
- The backend `insert` endpoint accepts integer values via query string (`?value=5`), raw body containing `5` (text/plain), or JSON (`5` or `{ "value": 5 }`).
- Use `scripts/start-all.ps1` to start backend + frontend and run the smoke test. Logs and PID files are in `bst_backend/scripts/logs/`.

---

End of printable bundle.
