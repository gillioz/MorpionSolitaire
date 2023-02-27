using MorpionSolitaire;

namespace MorpionSolitaireGraph;

public class Node
{
    public int Level { get; }
    public Branch? Root { get; }
    public List<Branch> Branches { get; }

    public Node(Game game)
    {
        Root = null;
        Level = game.GetScore();
        Branches = game.FindAllSegments()
            .Select(segment => new Branch(this, segment))
            .ToList();
    }
    
    public Node(Game game, Branch root, ICollection<Branch>? discardedBranches = null)
    {
        Root = root;
        Level = root.Node.Level + 1;
        Branches = game.FindNewSegments(root.Segment.Dot.Pt)
            .Select(segment => new Branch(this, segment))
            .ToList();;
        foreach (var branch in root.Node.Branches)
        {
            if (branch == root) continue;
            if (game.Image.IsValid(branch.Segment))
            {
                Branches.Add(new Branch(this, branch.Segment));
            }
            else
            {
                discardedBranches?.Add(branch);
            }
        }
    }
}