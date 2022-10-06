using System.Dynamic;
using MorpionSolitaire;

namespace MorpionSolitaireGraph;

public class GameGraph
{
    public List<GameNode> Nodes { get; set; }
    public Game Game { get; set; }

    public GameGraph(int segmentLength = 4, bool noTouchingRule = false,
        Grid? grid = null)
    {
        Game = new Game(segmentLength, noTouchingRule, grid);
        Nodes = new List<GameNode>(); // initialize Nodes
    }

    public void Play(GameLink link)
    {
        throw new NotImplementedException();
    }
}