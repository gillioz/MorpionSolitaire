using MorpionSolitaireGraph;

namespace MorpionSolitaireWeb;

public static class SessionManager
{
    private static Dictionary<string, Session> _sessions = new Dictionary<string, Session>();

    private const string SessionKey = "MorpionSolitaireID";

    public static GameGraph Restore(ISession httpContextSession)
    {
        var id = httpContextSession.GetString(SessionKey) ?? string.Empty;

        if (_sessions.TryGetValue(id, out var existingSession))
        {
            existingSession.Update();
            return existingSession.Game;
        }

        id = Guid.NewGuid().ToString();
        var newSession = new Session();
        httpContextSession.SetString(SessionKey, id);
        _sessions.Add(id, newSession);
        return newSession.Game;
    }

    public static void Assign(ISession httpContextSession, GameGraph game)
    {
        var id = httpContextSession.GetString(SessionKey) ?? string.Empty;
        if (!_sessions.TryGetValue(id, out var existingSession)) return;

        existingSession.Game = game;
        existingSession.Update();
    }

    public static void Clean()
    {
        _sessions = _sessions
            .Where(kv => !kv.Value.Expired())
            .ToDictionary(kv => kv.Key, kv => kv.Value);
    }
}