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
