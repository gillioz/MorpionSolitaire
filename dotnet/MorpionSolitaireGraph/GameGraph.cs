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
        Nodes = new List<GameNode>() { new GameNode(Game) };
    }

    public int GetNumberOfMoves()
    {
        return Nodes.Last().Branches.Count;
    }
    
    public void Play(int index)
    {
        throw new NotImplementedException();
    }
    
    public void PlayAtRandom()
    {
        throw new NotImplementedException();
    }
}