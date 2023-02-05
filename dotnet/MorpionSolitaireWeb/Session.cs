using MorpionSolitaire;
using MorpionSolitaireGraph;

namespace MorpionSolitaireWeb;

public class Session
{
    public GameGraph Game { get; set; }

    private DateTime _lastUsed;

    public Session()
    {
        Game = new GameGraph(Grid.Cross());
        _lastUsed = DateTime.UtcNow;
    }

    public void Update()
    {
        _lastUsed = DateTime.UtcNow;
    }

    public bool Expired()
    {
        return _lastUsed.AddHours(1) < DateTime.UtcNow;
    }
}