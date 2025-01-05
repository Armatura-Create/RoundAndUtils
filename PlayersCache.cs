using CounterStrikeSharp.API.Core;
using static RoundEndUtils.RoundEndUtils;

namespace RoundEndUtils;

public class PlayersCache {

    public List<CCSPlayerController?> _players = [];

    public void Load()
    {
        Instance.RegisterEventHandler<EventPlayerSpawn>(OnPlayerSpawn);
        Instance.RegisterEventHandler<EventPlayerDeath>(OnPlayerDeadth);
        Logs.PrintConsole("PlayersCache loaded");
    }

    public HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        if (@event == null || @event.Userid == null || @event.Userid.IsBot || !@event.Userid.IsValid)
        {
             return HookResult.Continue;
        }

        Logs.PrintConsole($"Player Spawned: [{@event.Userid}]");

        if (!_players.Contains(@event.Userid) && !@event.Userid.IsBot)
        {
            _players.Add(@event.Userid);
        }
        return HookResult.Continue;
    }

    private HookResult OnPlayerDeadth(EventPlayerDeath @event, GameEventInfo info)
    {
        if (@event == null || @event.Userid == null || @event.Userid.IsBot || !@event.Userid.IsValid)
        {
             return HookResult.Continue;
        }

        Logs.PrintConsole($"Player death: [{@event.Userid}]");
        if (_players.Contains(@event.Userid))
        {
            _players.Remove(@event.Userid);
        }
        return HookResult.Continue;
    }
}