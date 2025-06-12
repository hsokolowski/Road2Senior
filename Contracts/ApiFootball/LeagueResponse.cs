using System.Text.Json.Serialization;

namespace Contracts.ApiFootball;

public class LeagueResponse
{
    [JsonPropertyName("league")] 
    public League League { get; set; }
    
    [JsonPropertyName("country")] 
    public Country Country { get; set; }
    
    [JsonPropertyName("seasons")] 
    public List<Season> Seasons { get; set; }
}

public class League
{
    [JsonPropertyName("id")] 
    public int Id { get; set; }
    
    [JsonPropertyName("name")] 
    public string Name { get; set; }
    
    [JsonPropertyName("type")] 
    public string Type { get; set; }
}


public class Country
{
    [JsonPropertyName("name")] 
    public string Name { get; set; }
    
    [JsonPropertyName("code")] 
    public string? Code { get; set; }
}

public class Season
{
    [JsonPropertyName("year")] 
    public int Year { get; set; }
    
    [JsonPropertyName("start")] 
    public string StartDate { get; set; }
    
    [JsonPropertyName("end")] 
    public string EndDate { get; set; }
    
    [JsonPropertyName("current")] 
    public bool IsCurrent { get; set; }
}