using MorpionSolitaire;

namespace MorpionSolitaireGraph;

public class GameLink
{
    public GameNode Node;
    public Segment Segment;

    public GameLink(GameNode node, Segment segment)
    {
        Node = node;
        Segment = segment;
    }
}