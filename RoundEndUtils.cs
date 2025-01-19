using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace RoundEndUtils;

public class RoundEndUtils : BasePlugin, IPluginConfig<PluginConfig>
{
    public PluginConfig Config { get; set; } = new();
    public static RoundEndUtils Instance { get; private set; } = new();
    public PlayersCache Cache { get; } = new();

    public override string ModuleName => "[RoundEndUtils]";
    public override string ModuleAuthor => "Armatura";
    public override string ModuleVersion => "1.2.0";

    public void OnConfigParsed(PluginConfig config)
    {
        Instance = this;
        Config = config;
    }

    public override void Load(bool hotReload)
    {
        Logs.PrintConsole("Plugin loaded");
        RegisterEventHandler<EventRoundStart>(EventRoundStart);
        RegisterEventHandler<EventRoundEnd>(EventRoundEnd);
        Cache.Load();

        if (Config.EndRoundBhop.Active)
        {
            RegisterListener<Listeners.OnTick>(() =>
            {
                foreach (var player in Cache.GetAllPlayers())
                {
                    if (player == null || !player.IsValid || player.IsBot) continue;
                    if (Cache.GetBhopActive(player))
                    {
                        HandleBhop(player);
                    }
                }
            });
        }
    }

    private HookResult EventRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        Logs.PrintConsole("Round started. Resetting settings...");
        Server.ExecuteCommand($"sv_gravity 800");

        Cache.GetAllPlayers().ToList().ForEach(player =>
        {
            var playerPawn = player.PlayerPawn;

            if (playerPawn == null) return;

            var pawn = playerPawn.Value;

            if (pawn == null) return;

            Logs.PrintConsole($"Resetting speed for player {player.PlayerName}");
            pawn.VelocityModifier = 1.0f;
            Utilities.SetStateChanged(player, "CCSPlayerPawn", "m_flVelocityModifier");
            Cache.SetBhopActive(player, false);
        });

        return HookResult.Continue;
    }

    private HookResult EventRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        if (Config.EndRoundGravity.Active)
        {
            Logs.PrintConsole($"Setting gravity to {Config.EndRoundGravity.GravityValue}");
            Server.ExecuteCommand($"sv_gravity {Config.EndRoundGravity.GravityValue}");
        }

        if (Config.EndRoundSpeed.Active)
        {
            Logs.PrintConsole($"Setting speed modifier to {Config.EndRoundSpeed.MaxSpeed}");
            Cache.GetAllPlayers().ToList().ForEach(player =>
            {
                var pawn = player.PlayerPawn?.Value;
                if (pawn == null) return;

                Logs.PrintConsole($"Setting speed for player {player.PlayerName}");
                pawn.VelocityModifier = Config.EndRoundSpeed.MaxSpeed;
                Utilities.SetStateChanged(player, "CCSPlayerPawn", "m_flVelocityModifier");
            });
        }

        if (Config.EndRoundBhop.Active)
        {
            Logs.PrintConsole("Enabling Bhop...");
            Cache.GetAllPlayers().ToList().ForEach(player =>
            {
                var pawn = player.PlayerPawn?.Value;
                if (pawn == null) return;

                Logs.PrintConsole($"Activating Bhop for player {player.PlayerName}");
                Cache.SetBhopActive(player, true);
            });
        }

        return HookResult.Continue;
    }

    private void HandleBhop(CCSPlayerController player)
    {
        var pawn = player.PlayerPawn?.Value;
        if (pawn == null) return;

        var flags = (PlayerFlags) pawn.Flags;
        var buttons = player.Buttons;

        // Обработка прыжка
        if (buttons.HasFlag(PlayerButtons.Jump) &&
            flags.HasFlag(PlayerFlags.FL_ONGROUND) &&
            !pawn.MoveType.HasFlag(MoveType_t.MOVETYPE_LADDER))
        {
            pawn.AbsVelocity.Z = 300;
        }

        // Ограничение максимальной скорости
        var maxSpeed = Config.EndRoundBhop.MaxSpeed;
        if (pawn.AbsVelocity.Length2D() > maxSpeed)
        {
            AdjustVelocity(pawn, maxSpeed);
        }
    }

    private void AdjustVelocity(CCSPlayerPawn pawn, float maxSpeed)
    {
        var velocity = new Vector(pawn.AbsVelocity.X, pawn.AbsVelocity.Y, pawn.AbsVelocity.Z);
        var speed2D = Math.Sqrt(velocity.X * velocity.X + velocity.Y * velocity.Y);

        if (speed2D <= maxSpeed) return;

        var scale = maxSpeed / speed2D;
        pawn.AbsVelocity.X *= (float)scale;
        pawn.AbsVelocity.Y *= (float)scale;
    }
}
