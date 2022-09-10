namespace MorpionSolitaire;

public class Game
{
    public Grid Grid { get; }
    public Image Image { get; }
    
    public int SegmentLength { get; }
    public bool NoTouchingRule { get; }

    public Game()
    {
        Grid = new Grid();
        Image = new Image(dimensions: new GridCoordinates(24, 24),
            origin: new GridCoordinates(8, 8));
        SegmentLength = 4;
        NoTouchingRule = false;
        
        Apply(new InitialCross());
    }

    public void Apply(GameAction action)
    {
        Grid.Apply(action.GridAction);
        Image.Apply(action.ImageAction);
    }

    public void TrySegment(GridCoordinates pt1, GridCoordinates pt2)
    {
        try
        {
            var segment = new Segment(Image, pt1, pt2, SegmentLength, NoTouchingRule);
            Apply(segment);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}