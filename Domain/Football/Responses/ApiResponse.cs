using Newtonsoft.Json;

namespace Domain.Football.Responses;

public class ApiResponse<T>
{
    [JsonProperty("get")] 
    public string Get { get; set; }
    
    [JsonProperty("parameters")] 
    public Dictionary<string, string> Parameters { get; set; }
    
    [JsonProperty("results")] 
    public int Results { get; set; }
    
    [JsonProperty("response")] 
    public List<T> Response { get; set; }
}