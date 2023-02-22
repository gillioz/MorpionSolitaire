using MorpionSolitaire;

namespace MorpionSolitaireGraph;

public class GameNode
{
    public int Level { get; }
    public GameBranch? Root { get; }
    public List<GameBranch> Branches { get; }

    public GameNode(Game game)
    {
        Root = null;
        Level = game.GetScore();
        Branches = game.FindAllSegments()
            .Select(segment => new GameBranch(this, segment))
            .ToList();
    }
    
    public GameNode(Game game, GameBranch root, ICollection<GameBranch>? discardedBranches = null)
    {
        Root = root;
        Level = root.Node.Level + 1;
        Branches = game.FindNewSegments(root.Segment.Dot.Pt)
            .Select(segment => new GameBranch(this, segment))
            .ToList();;
        foreach (var branch in root.Node.Branches)
        {
            if (branch == root) continue;
            if (game.Image.IsValid(branch.Segment))
            {
                Branches.Add(new GameBranch(this, branch.Segment));
            }
            else
            {
                discardedBranches?.Add(branch);
            }
        }
    }
}