using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using melodia_api.Repositories;

namespace melodia_api.Repositories.Implementations
{
    public class QdrantRestClient : IQdrantRestClient
{
    private readonly HttpClient _client;
    private readonly string _collectionName;

    public QdrantRestClient(string baseUrl, string apiKey, string collectionName)
    {
        if (!baseUrl.EndsWith("/")) baseUrl += "/";

        _collectionName = collectionName;
        _client = new HttpClient { BaseAddress = new Uri(baseUrl) };
        _client.DefaultRequestHeaders.Add("api-key", apiKey);
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _client.DefaultRequestHeaders.UserAgent.ParseAdd("QdrantRestClient-CSharp/1.0");
    }

    public async Task CreateCollectionAsync(int vectorSize = 4)
    {
        var payload = new
        {
            vectors = new
            {
                size = vectorSize,
                distance = "cosine" // must be lowercase
            }
        };

        var response = await _client.PutAsync(
            $"collections/{_collectionName}",
            new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
        );

        response.EnsureSuccessStatusCode();
    }
    
    public async Task<bool> CollectionExistsAsync()
    {
        var response = await _client.GetAsync($"collections/{_collectionName}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return false;

        response.EnsureSuccessStatusCode();
        return true;
    }


    public async Task UpsertAsync(int id, float[] vector, object payload = null)
    {
        var request = new
        {
            points = new[]
            {
                new
                {
                    id,
                    vector,
                    payload
                }
            }
        };

        var response = await _client.PutAsync(
            $"collections/{_collectionName}/points",
            new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
        );

        response.EnsureSuccessStatusCode();
    }

    public async Task<List<(int Id, float Score, string PayloadJson)>> SearchAsync(float[] queryVector, int limit = 5)
    {
        var request = new
        {
            vector = queryVector,
            limit,
            with_payload = true
        };

        var response = await _client.PostAsync(
            $"collections/{_collectionName}/points/search",
            new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
        );

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JObject.Parse(json)["result"];

        var matches = new List<(int, float, string)>();
        foreach (var hit in result)
        {
            int id = hit["id"]?.Value<int>() ?? -1;
            float score = hit["score"]?.Value<float>() ?? -1f;
            string payload = hit["payload"]?.ToString(Formatting.None) ?? "{}";

            matches.Add((id, score, payload));
        }

        return matches;
    }
    
    public async Task<int> GetMaxIdAsync()
    {
        int maxId = 0;
        string offset = null;

        do
        {
            var requestBody = new
            {
                limit = 10000,
                with_payload = false,
                with_vector = false,
                offset = offset != null ? new { offset = offset } : null
            };

            // Supprimer les clés nulles (offset si null)
            var jsonSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            var jsonContent = JsonConvert.SerializeObject(requestBody, jsonSettings);

            var response = await _client.PostAsync(
                $"collections/{_collectionName}/points/scroll",
                new StringContent(jsonContent, Encoding.UTF8, "application/json")
            );

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var parsed = JObject.Parse(json);
            var points = parsed["result"]?["points"];

            if (points != null)
            {
                foreach (var point in points)
                {
                    int id = point["id"]?.Value<int>() ?? -1;
                    if (id > maxId) maxId = id;
                }
            }

            // Récupérer offset pour la page suivante
            offset = parsed["result"]?["next_page_offset"]?.ToString();

        } while (!string.IsNullOrEmpty(offset));

        return maxId;
    }
    
    public async Task DeletePointAsync(int id)
    {
        var requestBody = new
        {
            points = new[] { id }
        };

        var json = JsonConvert.SerializeObject(requestBody);
        var response = await _client.PostAsync(
            $"collections/{_collectionName}/points/delete",
            new StringContent(json, Encoding.UTF8, "application/json")
        );

        response.EnsureSuccessStatusCode();
    }
    
    public async Task<float> CompareVectorToPointAsync(float[] queryVector, int targetPointId)
    {
        var request = new
        {
            vector = queryVector,
            limit = 1,
            lookup = new
            {
                collection = _collectionName,
                point_id = targetPointId
            },
            with_payload = false,
            with_vector = false
        };

        var response = await _client.PostAsync(
            $"collections/{_collectionName}/points/search",
            new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
        );

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JObject.Parse(json)["result"];

        if (result != null && result.Any())
        {
            var score = result[0]["score"]?.Value<float>() ?? 0f;
            return score; // La similarité cosinus entre queryVector et le vecteur du point
        }

        return 0f; // Aucun résultat
    }

}

}
