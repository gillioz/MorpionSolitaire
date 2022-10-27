using MorpionSolitaire;

namespace MorpionSolitaireGraph;

public class GameNode
{
    public GameNode? Parent { get; }
    public Segment? Root { get; }
    public List<Segment> Branches { get; }

    public GameNode(Game game)
    {
        Parent = null;
        Root = null;
        Branches = game.FindAllSegments();
    }
    
    public GameNode(Game game, GameNode parent, Segment root)
    {
        Parent = parent;
        Root = root;
        Branches = game.FindNewSegments(root.Dot.Pt);
        foreach (var segment in parent.Branches)
        {
            if (game.Image.IsValid(segment))
            {
                Branches.Add(segment);
            }
        }
    }
}