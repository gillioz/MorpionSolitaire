namespace MorpionSolitaire;

public class GameAction
{
    public GridAction GridAction { get; }
    public ImageAction ImageAction { get; }

    public GameAction()
    {
        GridAction = new GridAction();
        ImageAction = new ImageAction();
    }
}