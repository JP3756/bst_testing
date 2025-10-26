using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;

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
        private readonly ILogger<BstApiService> _logger;

        public BstApiService(HttpClient httpClient, ILogger<BstApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task Insert(int value)
        {
            _logger.LogInformation("Inserting value {Value} to backend API", value);
            var response = await _httpClient.PostAsync($"/api/bst/insert?value={value}", null);
            response.EnsureSuccessStatusCode();
            _logger.LogInformation("Successfully inserted value {Value}", value);
        }

        public async Task Reset()
        {
            _logger.LogInformation("Resetting BST tree");
            var response = await _httpClient.PostAsync($"/api/bst/reset", null);
            response.EnsureSuccessStatusCode();
            _logger.LogInformation("BST tree reset completed");
        }

        public async Task<BstNodeModel?> GetTreeAsync()
        {
            try
            {
                _logger.LogDebug("Fetching tree structure from backend");
                var response = await _httpClient.GetAsync("/api/bst/tree");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var tree = JsonSerializer.Deserialize<BstNodeModel>(json, options);
                _logger.LogDebug("Tree structure retrieved successfully");
                return tree;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching tree structure from backend");
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

        public async Task<int?> GetMinimum()
        {
            var response = await _httpClient.GetAsync("/api/bst/min");
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent) return null;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content)) return null;
            return int.Parse(content);
        }

        public async Task<int?> GetMaximum()
        {
            var response = await _httpClient.GetAsync("/api/bst/max");
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent) return null;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content)) return null;
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

        public async Task<int> InsertBulk(int[] values)
        {
            // Send JSON array to the bulk insert endpoint
            var response = await _httpClient.PostAsJsonAsync("/api/bst/insert-bulk", values);
            response.EnsureSuccessStatusCode();
            try
            {
                var json = await response.Content.ReadFromJsonAsync<JsonElement>();
                if (json.ValueKind == JsonValueKind.Object && json.TryGetProperty("inserted", out var p) && p.TryGetInt32(out var n))
                    return n;
            }
            catch { }
            // If response body doesn't include count, return requested length as best-effort
            return values?.Length ?? 0;
        }
    }
}
