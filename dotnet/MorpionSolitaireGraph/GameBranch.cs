using MorpionSolitaire;

namespace MorpionSolitaireGraph;

public class GameBranch
{
    public GameNode Node;
    public Segment Segment;

    public GameBranch(GameNode node, Segment segment)
    {
        Node = node;
        Segment = segment;
    }
}