using MorpionSolitaire;

namespace MorpionSolitaireGraph;

public class GameNode
{
    public List<GameLink> GameLinks { get; set; }

    public GameNode()
    {
        GameLinks = new List<GameLink>();
    }
    
    public GameNode(Game game, GameLink link)
    {
        GameLinks = new List<GameLink>(); // to be implemented
    }
}