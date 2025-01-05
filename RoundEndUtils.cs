using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Serilog;


namespace RoundEndUtils;

public class RoundEndUtils : BasePlugin, IPluginConfig<PluginConfig>
{
    public PluginConfig Config { get; set; } = new();
    public static RoundEndUtils Instance { get; set; } = new();
    public PlayersCache Cache { get; } = new();

    public override string ModuleName => "[RoundEndUtils]";
    public override string ModuleAuthor => "Armatura";
    public override string ModuleVersion => "1.0.0";

    public void OnConfigParsed(PluginConfig config)
    {
        Instance = this;
        Config = config;
    }

    public override void Load(bool hotReload)
    {
        Console.WriteLine($"{ModuleName} [{ModuleVersion}] loaded");

        RegisterEventHandler<EventRoundStart>(EventRoundStart);
        RegisterEventHandler<EventRoundEnd>(EventRoundEnd);

        Cache.Load();
    }
		
	private HookResult EventRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        Logs.PrintConsole("Round started set gravity to 800");
        Server.ExecuteCommand($"sv_gravity 800");

        Cache._players.ForEach(player =>
            {
                if (player != null)
                {
                    Logs.PrintConsole($"Round started set speed to 1.0 for {player.PlayerName}");
                    var speedModifierValue = 1.0;
                    var playerPawn = player.PlayerPawn.Value;
                    if (playerPawn != null)
                    {
                        playerPawn.VelocityModifier = (float) speedModifierValue;
                        Utilities.SetStateChanged(player, "CCSPlayerPawn", "m_flVelocityModifier");
                    }
                } else {
                    Logs.PrintConsole("Player is null");
                }
            }
        );

        return HookResult.Continue;
    }

	private HookResult EventRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
		if (Config.EndRoundGravity.Active)
        {
            Logs.PrintConsole($"Round ended set gravity to {Config.EndRoundGravity.GravityValue}");
            Server.ExecuteCommand($"sv_gravity {Config.EndRoundGravity.GravityValue}");
        }

        if (Config.EndRoundSpeed.Active)
        {
            Logs.PrintConsole($"Round ended set speed to {Config.EndRoundSpeed.MaxSpeed}");
            Cache._players.ForEach(player =>
                {
                    if (player != null)
                    {
                        var speedModifierValue = Config.EndRoundSpeed.MaxSpeed;
                        var playerPawn = player.PlayerPawn.Value;
                        Logs.PrintConsole($"Round ended set speed to {speedModifierValue} for all player {player.PlayerName}");
                        if (playerPawn != null)
                        {
                            playerPawn.VelocityModifier = (float) speedModifierValue;
                            Utilities.SetStateChanged(player, "CCSPlayerPawn", "m_flVelocityModifier");
                        }
                    } else {
                        Logs.PrintConsole("Player is null");
                    }
                }
            );
        }
        if (Config.EndRoundBhop.Active)
        {
            
        }
        return HookResult.Continue;
    }
}
