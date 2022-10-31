using MorpionSolitaire;

namespace MorpionSolitaireGraph;

public class GameBranch
{
    public GameNode Node;
    public Segment Segment;
    public double Weight;

    public GameBranch(GameNode node, Segment segment)
    {
        Node = node;
        Segment = segment;
        Weight = 1.0;
    }
}