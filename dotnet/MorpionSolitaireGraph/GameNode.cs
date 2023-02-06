using MorpionSolitaire;

namespace MorpionSolitaireGraph;

public class GameNode
{
    public int Level { get; }
    public Segment? Root { get; }
    public List<Segment> Branches { get; }

    public GameNode(Game game)
    {
        Root = null;
        Level = game.GetScore();
        Branches = game.FindAllSegments();
    }
    
    public GameNode(Game game, GameNode parent, Segment root, ICollection<GameBranch>? discardedBranches = null)
    {
        Root = root;
        Level = parent.Level + 1;
        Branches = game.FindNewSegments(root.Dot.Pt);
        foreach (var segment in parent.Branches)
        {
            if (segment == root) continue;
            if (game.Image.IsValid(segment))
            {
                Branches.Add(segment);
            }
            else
            {
                discardedBranches?.Add(new GameBranch(parent, segment));
            }
        }
    }
}