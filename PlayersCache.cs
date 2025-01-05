using CounterStrikeSharp.API.Core;
using static RoundEndUtils.RoundEndUtils;

namespace RoundEndUtils;

public class PlayersCache
{
    private readonly HashSet<CCSPlayerController> _players = new();
    private readonly Dictionary<int, bool> _bhopActive = new(); // Slot, IsActive

    public void Load()
    {
        Instance.RegisterEventHandler<EventPlayerSpawn>(OnPlayerSpawn);
        Instance.RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
        Logs.PrintConsole("PlayersCache loaded");
    }

    public IEnumerable<CCSPlayerController> GetAllPlayers() => _players;
    public void SetBhopActive(CCSPlayerController player, bool isActive) => _bhopActive[player.Slot] = isActive;
    public bool GetBhopActive(CCSPlayerController player) => _bhopActive.TryGetValue(player.Slot, out var isActive) && isActive;

    private HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        var player = @event?.Userid;
        if (player is not { IsBot: false, IsValid: true }) return HookResult.Continue;

        Logs.PrintConsole($"Player Spawned: {player.PlayerName}");
        _players.Add(player);
        return HookResult.Continue;
    }

    private HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        var player = @event?.Userid;
        if (player is not { IsBot: false, IsValid: true }) return HookResult.Continue;

        Logs.PrintConsole($"Player Death: {player.PlayerName}");
        _players.Remove(player);
        return HookResult.Continue;
    }
}
