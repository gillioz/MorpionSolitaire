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

    public static bool operator <(GridCoordinates pt1, GridCoordinates pt2)
    {
        return pt1.X < pt2.X || (pt1.X == pt2.X && pt1.Y < pt2.Y);
    }

    public static bool operator >(GridCoordinates pt1, GridCoordinates pt2)
    {
        return pt1.X > pt2.X || (pt1.X == pt2.X && pt1.Y > pt2.Y);
    }

    public static bool operator ==(GridCoordinates pt1, GridCoordinates pt2)
    {
        return pt1.X == pt2.X && pt1.Y == pt2.Y;
    }
    
    public static bool operator !=(GridCoordinates pt1, GridCoordinates pt2)
    {
        return pt1.X != pt2.X || pt1.Y != pt2.Y;
    }
}