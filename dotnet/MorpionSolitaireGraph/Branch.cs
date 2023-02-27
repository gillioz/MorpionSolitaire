using MorpionSolitaire;

namespace MorpionSolitaireGraph;

public class Branch
{
    public Node Node;
    public Segment Segment;

    public Branch(Node node, Segment segment)
    {
        Node = node;
        Segment = segment;
    }
}