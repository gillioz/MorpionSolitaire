namespace MorpionSolitaire;

public struct GridCoordinates
{
    public int X { get; }
    public int Y { get; }

    public GridCoordinates(int x, int y)
    {
        X = x;
        Y = y;
    }

    public bool Equals(GridCoordinates other)
    {
        return (X == other.X) && (Y == other.Y);
    }
}