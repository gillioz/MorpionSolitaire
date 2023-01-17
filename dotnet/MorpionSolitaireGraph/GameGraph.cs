using MorpionSolitaire;

namespace MorpionSolitaireGraph;

public class GameGraph
{
    public GameNode Node { get; set; }
    public Game Game { get; set; }

    public GameGraph(int segmentLength = 4, bool noTouchingRule = false,
        Grid? grid = null)
    {
        Game = new Game(segmentLength, noTouchingRule, grid);
        Node = new GameNode(Game);
    }

    public GameGraph(Game game)
    {
        var nodes = new Stack<GameNode>();
        nodes.Push(new GameNode(game));
        while (game.GetScore() > 0)
        {
            game.Undo();
        }
        
        // TODO: implement proper chain of nodes
    }

    public int GetNumberOfMoves()
    {
        return Node.Branches.Count;
    }

    public void Play(Segment segment)
    {
        Game.ApplySegment(segment);
        Node = new GameNode(Game, Node, segment);
    }

    public void Play(int index)
    {
        var segment = Node.Branches[index];
        Play(segment);
    }

    public bool TryPlay(GridCoordinates pt1, GridCoordinates pt2)
    {
        var line = new GridLine(pt1, pt2);

        foreach (var segment in Node.Branches)
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
        if (n <= 0 || Node.Branches.Count <= 0) return;
        
        var rand = new Random();
        var index = rand.Next(0, Node.Branches.Count);
        Play(index);
        PlayAtRandom(n - 1);
    }

    public void PlayAtRandom()
    {
        if (Node.Branches.Count <= 0) return;
        
        var rand = new Random();
        var index = rand.Next(0, Node.Branches.Count);
        Play(index);
        PlayAtRandom();
    }

    public void Undo(int steps = 1)
    {
        while (steps > 0)
        {
            if (Node.Parent is null || Node.Root is null)
            {
                return;
            }

            Game.Grid.Actions.RemoveAt(Game.Grid.Actions.Count - 1);
            Game.Image.Apply(Node.Root.ToImageAction(), false);
            Node = Node.Parent;

            steps -= 1;
        }
    }

    public int GetScore()
    {
        return Game.GetScore();
    }

    public void Restart()
    {
        Undo(Game.GetScore());
    }
}