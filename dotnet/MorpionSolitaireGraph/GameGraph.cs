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

        var node = new GameNode();
        var segments = Game.FindAllSegments();
        foreach (var segment in segments)
        {
            node.GameLinks.Add(new GameLink(node, segment));
        }
        
        Nodes = new List<GameNode>() { node };
    }

    public int GetNumberOfMoves()
    {
        return Nodes.Last().GameLinks.Count;
    }
    
    public void Play(GameLink link)
    {
        throw new NotImplementedException();
    }
}