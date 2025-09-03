using System.Text.Json.Serialization;

namespace Contracts.ApiFootball;

public class ApiResponse<T>
{
    [JsonPropertyName("get")] 
    public string Get { get; set; }
    
    [JsonPropertyName("parameters")] 
    public Dictionary<string, string> Parameters { get; set; }
    
    [JsonPropertyName("results")] 
    public int Results { get; set; }
    
    [JsonPropertyName("response")] 
    public List<T> Response { get; set; }
}