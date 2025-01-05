using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;

public class PluginConfig : BasePluginConfig
{
    [JsonPropertyName("end_round_gravity")]
    public EndRoundGravityConfig EndRoundGravity { get; set; } = new EndRoundGravityConfig();

    [JsonPropertyName("end_round_speed")]
    public EndRoundSpeedConfig EndRoundSpeed { get; set; } = new EndRoundSpeedConfig();

    [JsonPropertyName("end_round_bhop")]
    public EndRoundBhopConfig EndRoundBhop { get; set; } = new EndRoundBhopConfig();

    [JsonPropertyName("debug")]
    public bool Debug { get; set; } = false;
}

public class EndRoundSpeedConfig
{
    [JsonPropertyName("active")]
    public bool Active { get; set; } = true;
    
    [JsonPropertyName("speed_value")]
    public float MaxSpeed { get; set; } = (float) 1.5;
}

public class EndRoundGravityConfig
{
    [JsonPropertyName("active")]
    public bool Active { get; set; } = true;
    
    [JsonPropertyName("gravity_value")]
    public int GravityValue { get; set; } = 100;
}


public class EndRoundBhopConfig
{
    [JsonPropertyName("active")]
    public bool Active { get; set; } = true;

    [JsonPropertyName("max_speed")]
    public int MaxSpeed { get; set; } = 850;
}
