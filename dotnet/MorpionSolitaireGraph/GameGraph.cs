using MorpionSolitaire;

namespace MorpionSolitaireGraph;

public class GameGraph : Game
{
    private readonly Random _random = new ();

    public Stack<GameNode> Nodes { get; }
    public List<GameBranch> DiscardedBranches { get; set; }

    public GameGraph(Grid grid) : base(grid.SegmentLength, grid.NoTouchingRule)
    {
        Grid = grid;
        Image = new Image(dimensions: new GridCoordinates(20, 20),
            origin: new GridCoordinates(5, 5));
        Nodes = new Stack<GameNode>();
        DiscardedBranches = new List<GameBranch>();

        if (grid.Actions.Count == 0)
        {
            throw new Exception("Attempt to create a game with an invalid grid");
        }

        var counter = 0;
        var actions = Grid.Actions.Reverse();
        foreach (var action in actions)
        {
            var dots = action.Elements.OfType<GridDot>().ToList();
            var lines = action.Elements.OfType<GridLine>().ToList();

            if (counter == 0)
            {
                if (lines.Count > 0) 
                    throw new Exception("Line elements are not supported at the initial stage");

                foreach (var dot in dots)
                {
                    Image.Set(new ImageCoordinates(dot.Pt), true);
                }

                Nodes.Push(new GameNode(this));
            }
            else
            {
                if (lines.Count != 1 || dots.Count != 1)
                    throw new Exception($"Invalid grid element at stage {counter}.");

                var line = lines.Single();
                var newPoint = dots.Single().Pt;
                var segment = NewSegment(line.Pt1, line.Pt2, newPoint);
                if (segment is null)
                {
                    throw new Exception($"Invalid segment at stage {counter}.");
                }
                Image.Apply(segment.ToImageAction());
                Nodes.Push(new GameNode(this, Nodes.Peek(), segment));
            }

            counter += 1;
        }
    }

    public int GetNumberOfMoves()
    {
        return Nodes.Peek().Branches.Count;
    }

    public void Play(Segment segment)
    {
        ApplySegment(segment);
        Nodes.Push(new GameNode(this, Nodes.Peek(), segment, DiscardedBranches));
    }

    public void Play(int index)
    {
        var segment = Nodes.Peek().Branches[index];
        Play(segment);
    }

    public bool TryPlay(GridCoordinates pt1, GridCoordinates pt2)
    {
        var line = new GridLine(pt1, pt2);

        foreach (var segment in Nodes.Peek().Branches)
        {
            if (segment.Line == line)
            {
                Play(segment);
                return true;
            }
        }

        return false;
    }

    public void PlayAtRandom(int n)
    {
        if (n <= 0 || GetNumberOfMoves() == 0) return;
        
        var rand = new Random();
        var index = rand.Next(0, GetNumberOfMoves());
        Play(index);
        PlayAtRandom(n - 1);
    }

    public void PlayAtRandom()
    {
        if (GetNumberOfMoves() == 0) return;
        
        var index = _random.Next(0, GetNumberOfMoves());
        Play(index);
        PlayAtRandom();
    }

    private void UndoNode()
    {
        Grid.Actions.Pop();
        var node = Nodes.Pop();
        if (node.Root is null) throw new Exception("Missing Root in GameNode");
        Image.Apply(node.Root.ToImageAction(), false);
    }

    private void CleanDiscardedBranches()
    {
        var level = Nodes.Peek().Level;
        DiscardedBranches = DiscardedBranches.Where(branch => branch.Node.Level < level).ToList();
    }

    public override void Undo(int steps = 1)
    {
        while (steps != 0)
        {
            if (Nodes.Count == 1 || Grid.Actions.Count == 1) break;

            UndoNode();

            steps -= 1;
        }

        CleanDiscardedBranches();
    }

    public void RevertToNode(GameNode node)
    {
        while (Nodes.Peek() != node)
        {
            UndoNode();
        }

        CleanDiscardedBranches();
    }

    private int WeightedRandomIndex(IList<double> weights)
    {
        var total = weights.Sum();
        var value = _random.NextDouble() * total;
        for (var i = 0; i < weights.Count; i++)
        {
            value -= weights[i];
            if (value < 0)
            {
                return i;
            }
        }

        return weights.Count - 1;
    }

    public void RevertToRandomNode(Func<int, double>? func = null)
    {
        if (Nodes.Count <= 1)
        {
            return;
        }

        int index;
        if (func == null)
        {
            index = _random.Next(1, Nodes.Count);
        }
        else
        {
            var weights = Nodes
                .Select(x => func(x.Level))
                .ToList();
            weights.RemoveAt(0);
            index = WeightedRandomIndex(weights) + 1;
        }

        var randomNode = Nodes.ElementAt(index);
        RevertToNode(randomNode);
    }

    public void RevertAndPlayBranch(GameBranch branch)
    {
        RevertToNode(branch.Node);
        Play(branch.Segment);
    }

    public void RevertAndPlayRandomDiscardedBranch(Func<int, double>? func = null)
    {
        int index;
        if (func == null)
        {
            index = _random.Next(0, DiscardedBranches.Count + 1);
        }
        else
        {
            var weights = DiscardedBranches
                .Select(x => func(x.Node.Level + 1))
                .ToList();
            weights.Add(func(0));
            index = WeightedRandomIndex(weights);
        }

        if (index == DiscardedBranches.Count)
        {
            Restart();
        }
        else
        {
            RevertAndPlayBranch(DiscardedBranches[index]);
        }
    }

    public override void Restart()
    {
        Undo(GetScore());
    }
}