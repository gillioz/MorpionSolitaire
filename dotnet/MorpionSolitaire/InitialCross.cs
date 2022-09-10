namespace MorpionSolitaire;

public class InitialCross: GameAction
{
    public InitialCross()
    {
        var dots = new List<GridCoordinates>()
        {
            new (3, 0), new (4, 0), new (5, 0), new (6, 0),
            new (3, 1), new (6, 1), new (3, 2), new (6, 2),
            new (0, 3), new (1, 3), new (2, 3), new (3, 3),
            new (6, 3), new (7, 3), new (8, 3), new (9, 3),
            new (0, 4), new (9, 4), new (0, 5), new (9, 5),
            new (0, 6), new (1, 6), new (2, 6), new (3, 6),
            new (6, 6), new (7, 6), new (8, 6), new (9, 6),
            new (3, 7), new (6, 7), new (3, 8), new (6, 8),
            new (3, 9), new (4, 9), new (5, 9), new (6, 9)
        };
        foreach (GridCoordinates dot in dots)
        {
            GridAction.Add(new GridDot(dot));
            ImageAction.Pixels.Add(new ImageCoordinates(dot));
        }
    }
}