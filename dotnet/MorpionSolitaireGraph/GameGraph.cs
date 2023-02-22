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
                var dot = dots.Single();
                if (!TryPlay(line.Pt1, line.Pt2, dot.Pt))
                {
                    throw new Exception($"Invalid segment at stage {counter}.");
                }
            }

            counter += 1;
        }
    }

    public int GetNumberOfMoves()
    {
        return Nodes.Peek().Branches.Count;
    }

    public void Play(GameBranch branch)
    {
        ApplySegment(branch.Segment);
        Nodes.Push(new GameNode(this, branch, DiscardedBranches));
    }

    public void Play(int index)
    {
        var segment = Nodes.Peek().Branches[index];
        Play(segment);
    }

    public bool TryPlay(GridCoordinates pt1, GridCoordinates pt2, GridCoordinates? newPt = null)
    {
        var line = new GridLine(pt1, pt2);

        foreach (var branch in Nodes.Peek().Branches)
        {
            if (branch.Segment.Line == line)
            {
                if (newPt != null && newPt != branch.Segment.Dot.Pt)
                {
                    return false;
                }
                Play(branch);
                return true;
            }
        }

        return false;
    }

    public void PlayAtRandom(int n)
    {
        var numberOfMoves = GetNumberOfMoves();
        if (n <= 0 || numberOfMoves == 0) return;
        
        var index = _random.Next(0, numberOfMoves);
        Play(index);
        PlayAtRandom(n - 1);
    }

    public void PlayAtRandom()
    {
        int numberOfMoves;
        while (true)
        {
            numberOfMoves = GetNumberOfMoves();
            if (numberOfMoves == 0) return;

            var index = _random.Next(0, numberOfMoves);
            Play(index);
        }
    }

    private GameBranch UndoNode()
    {
        Grid.Actions.Pop();
        var node = Nodes.Pop();
        if (node.Root is null) throw new Exception("Missing Root in GameNode");
        Image.Apply(node.Root.Segment.ToImageAction(), false);
        return node.Root;
    }

    private void CleanDiscardedBranches()
    {
        var level = Nodes.Peek().Level;
        DiscardedBranches.RemoveAll(branch => branch.Node.Level >= level);
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

    public void RevertToNode(GameNode node, bool removeRevertedBranch = false)
    {
        GameBranch? revertedBranch = null;
        while (Nodes.Peek() != node)
        {
            revertedBranch = UndoNode();
        }

        CleanDiscardedBranches();

        if (removeRevertedBranch && revertedBranch != null)
        {
            node.Branches.Remove(revertedBranch);
        }
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
            var newBranch = DiscardedBranches[index];
            RevertToNode(newBranch.Node);
            Play(newBranch);
        }
    }

    public void RevertAndPlayNextDiscardedBranch()
    {
        if (DiscardedBranches.Count == 0) return;

        // choose one of the top-level branches at random
        var maxLevel = DiscardedBranches
            .Select(branch => branch.Node.Level)
            .Max();
        var maxLevelBranches = DiscardedBranches
            .Where(branch => branch.Node.Level == maxLevel)
            .ToList();
        var index = _random.Next(0, maxLevelBranches.Count);
        var newBranch = maxLevelBranches[index];

        // revert to node and remove reverted branch
        RevertToNode(newBranch.Node, true);

        // play the new branch
        Play(newBranch);
    }
        
    public override void Restart()
    {
        Undo(GetScore());
    }
}