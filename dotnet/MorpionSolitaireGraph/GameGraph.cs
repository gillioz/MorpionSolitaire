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

    public int GetNumberOfMoves()
    {
        return Node.Branches.Count;
    }
    
    public void Play(int index)
    {
        var segment = Node.Branches[index];
        Game.ApplySegment(segment);
        Node = new GameNode(Game, Node, segment);
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

    public void Restart()
    {
        Undo(Game.GetScore());
    }
}