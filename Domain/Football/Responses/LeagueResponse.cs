using Newtonsoft.Json;

namespace Domain.Football.Responses;

public class LeagueResponse
{
    [JsonProperty("league")] 
    public League Get { get; set; }
    
    [JsonProperty("country")] 
    public Country Country { get; set; }
    
    [JsonProperty("seasons")] 
    public List<Season> Seasons { get; set; }
}

public class League
{
    [JsonProperty("id")] 
    public int Id { get; set; }
    
    [JsonProperty("name")] 
    public string Name { get; set; }
    
    [JsonProperty("type")] 
    public string Type { get; set; }
}


public class Country
{
    [JsonProperty("name")] 
    public string Name { get; set; }
    
    [JsonProperty("code")] 
    public string? Code { get; set; }
}

public class Season
{
    [JsonProperty("year")] 
    public int Year { get; set; }
    
    [JsonProperty("start")] 
    public string StartDate { get; set; }
    
    [JsonProperty("end")] 
    public string EndDate { get; set; }
    
    [JsonProperty("current")] 
    public bool IsCurrent { get; set; }
}